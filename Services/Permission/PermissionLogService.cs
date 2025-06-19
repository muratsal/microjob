using NanoGo.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NanoGo.Services.System
{
    public class PermissionLogService
    {

        public List<ItemForCombo> GetListForCombo(string search, int? value)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                List<ItemForCombo> itemComboList = new List<ItemForCombo>();
                if (!string.IsNullOrEmpty(search))
                {
                    itemComboList = dbContext.PermissionLogs.
                       Where(x => x.PermissionLogId.ToString().Contains(search)).
                       Take(20).
                       ToList().
                       Select(x => new ItemForCombo
                       {
                           DisplayText = x.PermissionLogId.ToString(),
                           Value = x.PermissionLogId,
                           FreeText = x.PermissionLogId.ToString()

                       }).ToList();
                }
                else if (value.HasValue && value.Value != 0)
                {
                    itemComboList = dbContext.PermissionLogs.
                      Where(x => x.PermissionLogId == value).
                      ToList().
                      Select(x => new ItemForCombo
                      {
                          DisplayText = x.PermissionLogId.ToString(),
                          Value = x.PermissionLogId,
                          FreeText = x.PermissionLogId.ToString()

                      }).ToList();
                }

                return itemComboList;
            }
        }

        public PageList<PermissionLogDTO> GetPermissionLogList(PermissionLogFilter filter, NanoGo.Shared.QueryInfo queryInfo, bool isExport)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var dbQuery = dbContext.PermissionLogs.Include(x=> x.Employee).OrderByDescending(x => x.PermissionLogId).Where(x => 1 == 1);
                int totalCount = 0;

                if (filter != null)
                {
                    if (filter.PermissionLogId.HasValue) dbQuery = dbQuery.Where(x => x.PermissionLogId == filter.PermissionLogId.Value);
                    if (filter.EmployeeId.HasValue) dbQuery = dbQuery.Where(x => x.EmployeeId == filter.EmployeeId.Value);
                    if (filter.Status.HasValue) dbQuery = dbQuery.Where(x => x.Status == filter.Status.Value);
                    if (filter.PermissionId.HasValue) dbQuery = dbQuery.Where(x => x.PermissionId == filter.PermissionId.Value);
                    if (filter.CreatedDate.HasValue) dbQuery = dbQuery.Where(x => x.CreatedDate >= filter.CreatedDate.Value);
                    if (filter.CreatedDate2.HasValue) dbQuery = dbQuery.Where(x => x.CreatedDate <= filter.CreatedDate2.Value);
                    if (filter.CreatedUser.HasValue) dbQuery = dbQuery.Where(x => x.CreatedUser == filter.CreatedUser.Value);
                    if (filter.UpdatedDate.HasValue) dbQuery = dbQuery.Where(x => x.UpdatedDate >= filter.UpdatedDate.Value);
                    if (filter.UpdatedDate2.HasValue) dbQuery = dbQuery.Where(x => x.UpdatedDate <= filter.UpdatedDate2.Value);
                    if (filter.UpdatedUser.HasValue) dbQuery = dbQuery.Where(x => x.UpdatedUser == filter.UpdatedUser.Value);
                }

                totalCount = dbQuery.Count();

                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.OrderBy))
                    {
                        string clientOrderByName = queryInfo.OrderBy.StartsWith("-") ? queryInfo.OrderBy.Substring(1) : queryInfo.OrderBy;
                        string orderByName = typeof(Models.PermissionLog).GetProperties().Where(x => x.Name.ToUpper() == clientOrderByName.ToUpper()).First().Name;

                        if (queryInfo.OrderBy.StartsWith("-"))
                        {
                            dbQuery = dbQuery.OrderByDescending(p => EF.Property<object>(p, orderByName));
                        }
                        else
                        {
                            dbQuery = dbQuery.OrderBy(p => EF.Property<object>(p, orderByName));
                        }
                    }
                }

                if (!isExport)
                {
                    if (queryInfo != null && queryInfo.Pager != null)
                    {
                        dbQuery = dbQuery.Skip((queryInfo.Pager.CurrentPage) * queryInfo.Pager.PageSize).Take(queryInfo.Pager.PageSize);
                    }
                }

                var data = dbQuery.ToList()
                     .Select(x => new PermissionLogDTO
                     {
                         PermissionLogId = x.PermissionLogId,
                         EmployeeId = x.EmployeeId,
                         EmployeeName = x.Employee.FullName,
                         Status = x.Status,
                         PermissionId = x.PermissionId,
                         CreatedDate = x.CreatedDate,
                         CreatedUser = x.CreatedUser,
                         UpdatedDate = x.UpdatedDate,
                         UpdatedUser = x.UpdatedUser,
                         CreatedUserText = dbContext.Users.First(y => y.UserId == x.CreatedUser).UserName,
                         UpdatedUserText = dbContext.Users.First(y => y.UserId == x.UpdatedUser).UserName
                     }).ToList();

                return new PageList<PermissionLogDTO> { Data = data, TotalCount = totalCount };

            }
        }

        public PermissionLogDTO SavePermissionLog(PermissionLogDTO permissionLogDTO, UserInfo userInfo)
        {
            Models.PermissionLog permissionLog = new Models.PermissionLog();
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                bool isNewRecord = permissionLogDTO.PermissionLogId == 0;
                if (isNewRecord)
                {
                    permissionLog.CreatedDate = DateTime.Now;
                    permissionLog.CreatedUser = userInfo.UserId;
                }
                else
                {
                    permissionLog = dbContext.PermissionLogs.First(x => x.PermissionLogId == permissionLogDTO.PermissionLogId);

                }

                permissionLog.UpdatedDate = DateTime.Now;
                permissionLog.UpdatedUser = userInfo.UserId;


                permissionLog.EmployeeId = permissionLogDTO.EmployeeId;
                permissionLog.Status = permissionLogDTO.Status;
                permissionLog.PermissionId = permissionLogDTO.PermissionId;





                if (isNewRecord)
                {
                    dbContext.PermissionLogs.Add(permissionLog);
                }

                dbContext.SaveChanges();

                permissionLogDTO.PermissionLogId = permissionLog.PermissionLogId;
            }

            return permissionLogDTO;
        }


        public void DeletePermissionLog(PermissionLogDTO permissionLogDTO)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var permissionLog = dbContext.PermissionLogs.FirstOrDefault(x => x.PermissionLogId == permissionLogDTO.PermissionLogId);
                if (permissionLog != null)
                {
                    dbContext.PermissionLogs.Remove(permissionLog);
                    dbContext.SaveChanges();
                }
            }
        }
    }


    public class PermissionLogDTO
    {
        public Int32 PermissionLogId { get; set; }
        public Int32 EmployeeId { get; set; }
        public String EmployeeName { get; set; }
        public Int32 Status { get; set; }
        public Int32 PermissionId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Int32 CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Int32 UpdatedUser { get; set; }
        public String CreatedUserText { get; set; }
        public String UpdatedUserText { get; set; }
    }

    public class PermissionLogFilter
    {
        public Int32? PermissionLogId { get; set; }
        public Int32? EmployeeId { get; set; }
        public Int32? Status { get; set; }
        public Int32? PermissionId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? CreatedDate2 { get; set; }
        public Int32? CreatedUser { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? UpdatedDate2 { get; set; }
        public Int32? UpdatedUser { get; set; }
    }
}
