using NanoGo.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NanoGo.Services.System
{
    public class RoleAuthService
    {
        public PageList<RoleAuthDTO> GetRoleAuthList(RoleAuthFilter filter, NanoGo.Shared.QueryInfo queryInfo, bool isExport)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var dbQuery = dbContext.RoleAuths.OrderByDescending(x => x.RoleAuthId).Where(x => 1 == 1);
                int totalCount = 0;

                if (filter != null)
                {
                    if (filter.RoleAuthId.HasValue) dbQuery = dbQuery.Where(x => x.RoleAuthId == filter.RoleAuthId.Value);
                    if (filter.RoleId.HasValue) dbQuery = dbQuery.Where(x => x.RoleId == filter.RoleId.Value);
                    if (filter.AuthId.HasValue) dbQuery = dbQuery.Where(x => x.AuthId == filter.AuthId.Value);
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
                        string orderByName = typeof(Models.RoleAuth).GetProperties().Where(x => x.Name.ToUpper() == clientOrderByName.ToUpper()).First().Name;

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
                     .Select(x => new RoleAuthDTO
                     {
                         RoleAuthId = x.RoleAuthId,
                         RoleId = x.RoleId,
                         AuthId = x.AuthId,
                         CreatedDate = x.CreatedDate,
                         CreatedUser = x.CreatedUser,
                         UpdatedDate = x.UpdatedDate,
                         UpdatedUser = x.UpdatedUser,
                         CreatedUserText = dbContext.Users.First(y => y.UserId == x.CreatedUser).UserName,
                         UpdatedUserText = dbContext.Users.First(y => y.UserId == x.UpdatedUser).UserName
                     }).ToList();

                return new PageList<RoleAuthDTO> { Data = data, TotalCount = totalCount };

            }
        }

        public RoleAuthDTO SaveRoleAuth(RoleAuthDTO roleAuthDTO, UserInfo userInfo)
        {
            Models.RoleAuth roleAuth = new Models.RoleAuth();
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                bool isNewRecord = roleAuthDTO.RoleAuthId == 0;
                if (isNewRecord)
                {
                    roleAuth.CreatedDate = DateTime.Now;
                    roleAuth.CreatedUser = userInfo.UserId;
                }
                else
                {
                    roleAuth = dbContext.RoleAuths.First(x => x.RoleAuthId == roleAuthDTO.RoleAuthId);

                }

                roleAuth.UpdatedDate = DateTime.Now;
                roleAuth.UpdatedUser = userInfo.UserId;


                roleAuth.RoleId = roleAuthDTO.RoleId;
                roleAuth.AuthId = roleAuthDTO.AuthId;





                if (isNewRecord)
                {
                    dbContext.RoleAuths.Add(roleAuth);
                }

                dbContext.SaveChanges();

                roleAuthDTO.RoleAuthId = roleAuth.RoleAuthId;
            }

            return roleAuthDTO;
        }


        public void DeleteRoleAuth(RoleAuthDTO roleAuthDTO)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var roleAuth = dbContext.RoleAuths.FirstOrDefault(x => x.RoleAuthId == roleAuthDTO.RoleAuthId);
                if (roleAuth != null)
                {
                    dbContext.RoleAuths.Remove(roleAuth);
                    dbContext.SaveChanges();
                }
            }
        }
    }


    public class RoleAuthDTO
    {
        public Int32 RoleAuthId { get; set; }
        public Int32? RoleId { get; set; }
        public Int32? AuthId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Int32 CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Int32 UpdatedUser { get; set; }
        public String CreatedUserText { get; set; }
        public String UpdatedUserText { get; set; }
    }

    public class RoleAuthFilter
    {
        public Int32? RoleAuthId { get; set; }
        public Int32? RoleId { get; set; }
        public Int32? AuthId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? CreatedDate2 { get; set; }
        public Int32? CreatedUser { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? UpdatedDate2 { get; set; }
        public Int32? UpdatedUser { get; set; }
    }
}
