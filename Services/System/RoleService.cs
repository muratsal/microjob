using NanoGo.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NanoGo.Services.System
{
    public class RoleService
    {
        public PageList<RoleDTO> GetRoleList(RoleFilter filter, NanoGo.Shared.QueryInfo queryInfo, bool isExport)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var dbQuery = dbContext.Roles.OrderByDescending(x => x.RoleId).Where(x => 1 == 1);
                int totalCount = 0;

                if (filter != null)
                {
                    if (filter.RoleId.HasValue) dbQuery = dbQuery.Where(x => x.RoleId == filter.RoleId.Value);
                    if (!string.IsNullOrEmpty(filter.RoleName)) dbQuery = dbQuery.Where(x => x.RoleName.Contains(filter.RoleName));
                    if (!string.IsNullOrEmpty(filter.RoleDesc)) dbQuery = dbQuery.Where(x => x.RoleDesc.Contains(filter.RoleDesc));
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
                        string orderByName = typeof(Models.Role).GetProperties().Where(x => x.Name.ToUpper() == clientOrderByName.ToUpper()).First().Name;

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
                     .Select(x => new RoleDTO
                     {
                         RoleId = x.RoleId,
                         RoleName = x.RoleName,
                         RoleDesc = x.RoleDesc,
                         CreatedDate = x.CreatedDate,
                         CreatedUser = x.CreatedUser,
                         UpdatedDate = x.UpdatedDate,
                         UpdatedUser = x.UpdatedUser,
                         CreatedUserText = dbContext.Users.First(y => y.UserId == x.CreatedUser).UserName,
                         UpdatedUserText = dbContext.Users.First(y => y.UserId == x.UpdatedUser).UserName
                     }).ToList();

                return new PageList<RoleDTO> { Data = data, TotalCount = totalCount };

            }
        }

        public RoleDTO SaveRole(RoleDTO roleDTO, UserInfo userInfo)
        {
            Models.Role role = new Models.Role();
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                bool isNewRecord = roleDTO.RoleId == 0;
                if (isNewRecord)
                {
                    role.CreatedDate = DateTime.Now;
                    role.CreatedUser = userInfo.UserId;
                }
                else
                {
                    role = dbContext.Roles.First(x => x.RoleId == roleDTO.RoleId);

                }

                role.UpdatedDate = DateTime.Now;
                role.UpdatedUser = userInfo.UserId;


                role.RoleName = roleDTO.RoleName;
                role.RoleDesc = roleDTO.RoleDesc;





                if (isNewRecord)
                {
                    dbContext.Roles.Add(role);
                }

                dbContext.SaveChanges();

                roleDTO.RoleId = role.RoleId;
            }

            return roleDTO;
        }


        public void DeleteRole(RoleDTO roleDTO)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var role = dbContext.Roles.FirstOrDefault(x => x.RoleId == roleDTO.RoleId);
                if (role != null)
                {
                    dbContext.Roles.Remove(role);
                    dbContext.SaveChanges();
                }
            }
        }


        public List<RoleAuthInfo> GetAuthInfo(int roleId)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var roleAuths = dbContext.RoleAuths.Where(x => x.RoleId == roleId).Select(x => x.AuthId).ToList();

                var auths = dbContext.Auths.ToList();

                List<RoleAuthInfo> roleAuthInfos = new List<RoleAuthInfo>();

                auths.ForEach(x =>
                {
                    roleAuthInfos.Add(new RoleAuthInfo()
                    {
                        RoleId = roleId,
                        AuthId = x.AuthId,
                        AuthCode = x.AuthCode,
                        AuthDesc = x.AuthDesc,
                        IsRoleAuth = roleAuths.Any(y => y == x.AuthId)
                    });
                });

                return roleAuthInfos;
            }
        }

    }


    public class RoleDTO
    {
        public Int32 RoleId { get; set; }
        public String RoleName { get; set; }
        public String RoleDesc { get; set; }
        public DateTime CreatedDate { get; set; }
        public Int32 CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Int32 UpdatedUser { get; set; }
        public String CreatedUserText { get; set; }
        public String UpdatedUserText { get; set; }
    }

    public class RoleFilter
    {
        public Int32? RoleId { get; set; }
        public String RoleName { get; set; }
        public String RoleDesc { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? CreatedDate2 { get; set; }
        public Int32? CreatedUser { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? UpdatedDate2 { get; set; }
        public Int32? UpdatedUser { get; set; }
    }

    public class RoleAuthInfo
    {
        public int RoleId { get; set; }
        public int AuthId { get; set; }
        public String AuthCode { get; set; }
        public String AuthDesc { get; set; }
        public Boolean IsRoleAuth { get; set; }

    }
}
