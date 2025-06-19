using NanoGo.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NanoGo.Services.System
{
    public class EmailConfigService
    {
        public PageList<EmailConfigDTO> GetEmailConfigList(EmailConfigFilter filter, NanoGo.Shared.QueryInfo queryInfo,bool isExport)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var dbQuery = dbContext.EmailConfigs.OrderByDescending(x => x.EmailConfigId).Where(x => 1 == 1);
                int totalCount = 0;

                if (filter != null)
                {
                    if (filter.EmailConfigId.HasValue) dbQuery = dbQuery.Where(x => x.EmailConfigId == filter.EmailConfigId.Value);
if (!string.IsNullOrEmpty(filter.ConfigName)) dbQuery = dbQuery.Where(x => x.ConfigName.Contains(filter.ConfigName));
if (!string.IsNullOrEmpty(filter.Host)) dbQuery = dbQuery.Where(x => x.Host.Contains(filter.Host));
if (!string.IsNullOrEmpty(filter.Port)) dbQuery = dbQuery.Where(x => x.Port.Contains(filter.Port));
if (filter.EnableSsl.HasValue) dbQuery = dbQuery.Where(x => x.EnableSsl == filter.EnableSsl.Value);
if (!string.IsNullOrEmpty(filter.UserName)) dbQuery = dbQuery.Where(x => x.UserName.Contains(filter.UserName));
if (!string.IsNullOrEmpty(filter.Password)) dbQuery = dbQuery.Where(x => x.Password.Contains(filter.Password));
if (filter.IsActive.HasValue) dbQuery = dbQuery.Where(x => x.IsActive == filter.IsActive.Value);
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
                        string orderByName = typeof(Models.EmailConfig).GetProperties().Where(x => x.Name.ToUpper() == clientOrderByName.ToUpper()).First().Name;

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
                     .Select(x => new EmailConfigDTO
                     {
                        EmailConfigId = x.EmailConfigId,
ConfigName = x.ConfigName,
Host = x.Host,
Port = x.Port,
EnableSsl = x.EnableSsl,
UserName = x.UserName,
Password = x.Password,
IsActive = x.IsActive,
CreatedDate = x.CreatedDate,
CreatedUser = x.CreatedUser,
UpdatedDate = x.UpdatedDate,
UpdatedUser = x.UpdatedUser,
                         CreatedUserText = dbContext.Users.First(y => y.UserId == x.CreatedUser).UserName,
                         UpdatedUserText = dbContext.Users.First(y => y.UserId == x.UpdatedUser).UserName
                     }).ToList();

                return new PageList<EmailConfigDTO> { Data = data, TotalCount = totalCount };

            }
        }

        public EmailConfigDTO SaveEmailConfig(EmailConfigDTO emailConfigDTO,  UserInfo userInfo)
        {
            Models.EmailConfig emailConfig = new Models.EmailConfig();
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                bool isNewRecord = emailConfigDTO.EmailConfigId == 0;
                if (isNewRecord)
                {
                    emailConfig.CreatedDate = DateTime.Now;
                    emailConfig.CreatedUser = userInfo.UserId;
                }
                else
                {
                    emailConfig = dbContext.EmailConfigs.First(x => x.EmailConfigId == emailConfigDTO.EmailConfigId);
                   
                }

                 emailConfig.UpdatedDate = DateTime.Now;
                 emailConfig.UpdatedUser = userInfo.UserId;

                
emailConfig.ConfigName = emailConfigDTO.ConfigName ;
emailConfig.Host = emailConfigDTO.Host ;
emailConfig.Port = emailConfigDTO.Port ;
emailConfig.EnableSsl = emailConfigDTO.EnableSsl ;
emailConfig.UserName = emailConfigDTO.UserName ;
emailConfig.Password = emailConfigDTO.Password ;
emailConfig.IsActive = emailConfigDTO.IsActive ;





                if (isNewRecord)
                {
                    dbContext.EmailConfigs.Add(emailConfig);
                }

                dbContext.SaveChanges();

                emailConfigDTO.EmailConfigId = emailConfig.EmailConfigId;
            }

            return emailConfigDTO;
        }


        public void DeleteEmailConfig(EmailConfigDTO emailConfigDTO)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var emailConfig = dbContext.EmailConfigs.FirstOrDefault(x => x.EmailConfigId == emailConfigDTO.EmailConfigId);
                if (emailConfig != null)
                {
                    dbContext.EmailConfigs.Remove(emailConfig);
                    dbContext.SaveChanges();
                }
            }
        }
    }


    public class EmailConfigDTO
    {
       public Int32 EmailConfigId { get; set; }
public String ConfigName { get; set; }
public String Host { get; set; }
public String Port { get; set; }
public Boolean? EnableSsl { get; set; }
public String UserName { get; set; }
public String Password { get; set; }
public Boolean? IsActive { get; set; }
public DateTime CreatedDate { get; set; }
public Int32 CreatedUser { get; set; }
public DateTime UpdatedDate { get; set; }
public Int32 UpdatedUser { get; set; }
       public String CreatedUserText  {get;set;}
       public String UpdatedUserText  {get;set;}
    }

    public class EmailConfigFilter
    {
        public Int32? EmailConfigId { get; set; }
public String ConfigName { get; set; }
public String Host { get; set; }
public String Port { get; set; }
public Boolean? EnableSsl { get; set; }
public String UserName { get; set; }
public String Password { get; set; }
public Boolean? IsActive { get; set; }
public DateTime? CreatedDate { get; set; }
public DateTime? CreatedDate2 { get; set; }
public Int32? CreatedUser { get; set; }
public DateTime? UpdatedDate { get; set; }
public DateTime? UpdatedDate2 { get; set; }
public Int32? UpdatedUser { get; set; }
    }
}
