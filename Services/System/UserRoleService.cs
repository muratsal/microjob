using NanoGo.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NanoGo.Services.System
{
    public class UserRoleService
    {
        public PageList<UserRoleDTO> GetUserRoleList(UserRoleFilter filter, NanoGo.Shared.QueryInfo queryInfo, bool isExport)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var dbQuery = dbContext.UserRoles.OrderByDescending(x => x.UserRoleId).Where(x => 1 == 1);
                int totalCount = 0;

                if (filter != null)
                {
                    if (filter.UserRoleId.HasValue) dbQuery = dbQuery.Where(x => x.UserRoleId == filter.UserRoleId.Value);
                    if (filter.UserId.HasValue) dbQuery = dbQuery.Where(x => x.UserId == filter.UserId.Value);
                    if (filter.RoleId.HasValue) dbQuery = dbQuery.Where(x => x.RoleId == filter.RoleId.Value);
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
                        string orderByName = typeof(Models.UserRole).GetProperties().Where(x => x.Name.ToUpper() == clientOrderByName.ToUpper()).First().Name;

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
                     .Select(x => new UserRoleDTO
                     {
                         UserRoleId = x.UserRoleId,
                         UserId = x.UserId,
                         RoleId = x.RoleId,
                         CreatedDate = x.CreatedDate,
                         CreatedUser = x.CreatedUser,
                         UpdatedDate = x.UpdatedDate,
                         UpdatedUser = x.UpdatedUser,
                         CreatedUserText = dbContext.Users.First(y => y.UserId == x.CreatedUser).UserName,
                         UpdatedUserText = dbContext.Users.First(y => y.UserId == x.UpdatedUser).UserName
                     }).ToList();

                return new PageList<UserRoleDTO> { Data = data, TotalCount = totalCount };

            }
        }

        public UserRoleDTO SaveUserRole(UserRoleDTO userRoleDTO, UserInfo userInfo)
        {
            Models.UserRole userRole = new Models.UserRole();
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                bool isNewRecord = userRoleDTO.UserRoleId == 0;
                if (isNewRecord)
                {
                    userRole.CreatedDate = DateTime.Now;
                    userRole.CreatedUser = userInfo.UserId;
                }
                else
                {
                    userRole = dbContext.UserRoles.First(x => x.UserRoleId == userRoleDTO.UserRoleId);

                }

                userRole.UpdatedDate = DateTime.Now;
                userRole.UpdatedUser = userInfo.UserId;


                userRole.UserId = userRoleDTO.UserId;
                userRole.RoleId = userRoleDTO.RoleId;





                if (isNewRecord)
                {
                    dbContext.UserRoles.Add(userRole);
                }

                dbContext.SaveChanges();

                userRoleDTO.UserRoleId = userRole.UserRoleId;
            }

            return userRoleDTO;
        }


        public void DeleteUserRole(UserRoleDTO userRoleDTO)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var userRole = dbContext.UserRoles.FirstOrDefault(x => x.UserRoleId == userRoleDTO.UserRoleId);
                if (userRole != null)
                {
                    dbContext.UserRoles.Remove(userRole);
                    dbContext.SaveChanges();
                }
            }
        }
    }


    public class UserRoleDTO
    {
        public Int32 UserRoleId { get; set; }
        public Int32 UserId { get; set; }
        public Int32 RoleId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Int32 CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Int32 UpdatedUser { get; set; }
        public String CreatedUserText { get; set; }
        public String UpdatedUserText { get; set; }
    }

    public class UserRoleFilter
    {
        public Int32? UserRoleId { get; set; }
        public Int32? UserId { get; set; }
        public Int32? RoleId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? CreatedDate2 { get; set; }
        public Int32? CreatedUser { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? UpdatedDate2 { get; set; }
        public Int32? UpdatedUser { get; set; }
    }
}
