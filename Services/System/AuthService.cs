using NanoGo.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NanoGo.Services.System
{
    public class AuthService
    {
        public PageList<AuthDTO> GetAuthList(AuthFilter filter, NanoGo.Shared.QueryInfo queryInfo, bool isExport)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var dbQuery = dbContext.Auths.OrderByDescending(x => x.AuthId).Where(x => 1 == 1);
                int totalCount = 0;

                if (filter != null)
                {
                    if (filter.AuthId.HasValue) dbQuery = dbQuery.Where(x => x.AuthId == filter.AuthId.Value);
                    if (!string.IsNullOrEmpty(filter.AuthCode)) dbQuery = dbQuery.Where(x => x.AuthCode.Contains(filter.AuthCode));
                    if (!string.IsNullOrEmpty(filter.AuthDesc)) dbQuery = dbQuery.Where(x => x.AuthDesc.Contains(filter.AuthDesc));
                    if (filter.AuthType.HasValue) dbQuery = dbQuery.Where(x => x.AuthType == filter.AuthType.Value);
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
                        string orderByName = typeof(Models.Auth).GetProperties().Where(x => x.Name.ToUpper() == clientOrderByName.ToUpper()).First().Name;

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
                     .Select(x => new AuthDTO
                     {
                         AuthId = x.AuthId,
                         AuthCode = x.AuthCode,
                         AuthDesc = x.AuthDesc,
                         AuthType = x.AuthType,
                         CreatedDate = x.CreatedDate,
                         CreatedUser = x.CreatedUser,
                         UpdatedDate = x.UpdatedDate,
                         UpdatedUser = x.UpdatedUser,
                         CreatedUserText = dbContext.Users.First(y => y.UserId == x.CreatedUser).UserName,
                         UpdatedUserText = dbContext.Users.First(y => y.UserId == x.UpdatedUser).UserName
                     }).ToList();

                return new PageList<AuthDTO> { Data = data, TotalCount = totalCount };

            }
        }

        public AuthDTO SaveAuth(AuthDTO authDTO, UserInfo userInfo)
        {
            Models.Auth auth = new Models.Auth();
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                bool isNewRecord = authDTO.AuthId == 0;
                if (isNewRecord)
                {
                    auth.CreatedDate = DateTime.Now;
                    auth.CreatedUser = userInfo.UserId;
                }
                else
                {
                    auth = dbContext.Auths.First(x => x.AuthId == authDTO.AuthId);

                }

                auth.UpdatedDate = DateTime.Now;
                auth.UpdatedUser = userInfo.UserId;


                auth.AuthCode = authDTO.AuthCode;
                auth.AuthDesc = authDTO.AuthDesc;
                auth.AuthType = authDTO.AuthType;


                if (isNewRecord)
                {
                    dbContext.Auths.Add(auth);
                }

                dbContext.SaveChanges();

                authDTO.AuthId = auth.AuthId;
            }

            return authDTO;
        }


        public void DeleteAuth(AuthDTO authDTO)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var auth = dbContext.Auths.FirstOrDefault(x => x.AuthId == authDTO.AuthId);
                if (auth != null)
                {
                    dbContext.Auths.Remove(auth);
                    dbContext.SaveChanges();
                }
            }
        }
    }


    public class AuthDTO
    {
        public Int32 AuthId { get; set; }
        public String AuthCode { get; set; }
        public String AuthDesc { get; set; }
        public Int32 AuthType { get; set; }
        public DateTime CreatedDate { get; set; }
        public Int32 CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Int32 UpdatedUser { get; set; }
        public String CreatedUserText { get; set; }
        public String UpdatedUserText { get; set; }
    }

    public class AuthFilter
    {
        public Int32? AuthId { get; set; }
        public String AuthCode { get; set; }
        public String AuthDesc { get; set; }
        public Int32? AuthType { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? CreatedDate2 { get; set; }
        public Int32? CreatedUser { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? UpdatedDate2 { get; set; }
        public Int32? UpdatedUser { get; set; }
    }
}
