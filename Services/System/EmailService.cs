using NanoGo.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NanoGo.Services.System
{
    public class EmailService
    {
        public PageList<EmailDTO> GetEmailList(EmailFilter filter, NanoGo.Shared.QueryInfo queryInfo, bool isExport)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var dbQuery = dbContext.Emails.OrderByDescending(x => x.EmailId).Where(x => 1 == 1);
                int totalCount = 0;

                if (filter != null)
                {
                    if (filter.EmailId.HasValue) dbQuery = dbQuery.Where(x => x.EmailId == filter.EmailId.Value);
                    if (!string.IsNullOrEmpty(filter.EmailFrom)) dbQuery = dbQuery.Where(x => x.EmailFrom.Contains(filter.EmailFrom));
                    if (!string.IsNullOrEmpty(filter.EmailTo)) dbQuery = dbQuery.Where(x => x.EmailTo.Contains(filter.EmailTo));
                    if (!string.IsNullOrEmpty(filter.EmailToCc)) dbQuery = dbQuery.Where(x => x.EmailToCc.Contains(filter.EmailToCc));
                    if (!string.IsNullOrEmpty(filter.EmailToBcc)) dbQuery = dbQuery.Where(x => x.EmailToBcc.Contains(filter.EmailToBcc));
                    if (!string.IsNullOrEmpty(filter.Subject)) dbQuery = dbQuery.Where(x => x.Subject.Contains(filter.Subject));
                    if (!string.IsNullOrEmpty(filter.Body)) dbQuery = dbQuery.Where(x => x.Body.Contains(filter.Body));
                    if (filter.IsHtml.HasValue) dbQuery = dbQuery.Where(x => x.IsHtml == filter.IsHtml.Value);
                    if (filter.IsSend.HasValue) dbQuery = dbQuery.Where(x => x.IsSend == filter.IsSend.Value);
                    if (filter.IsSuccess.HasValue) dbQuery = dbQuery.Where(x => x.IsSuccess == filter.IsSuccess.Value);
                    if (filter.SendDate.HasValue) dbQuery = dbQuery.Where(x => x.SendDate >= filter.SendDate.Value);
                    if (filter.SendDate2.HasValue) dbQuery = dbQuery.Where(x => x.SendDate <= filter.SendDate2.Value);
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
                        string orderByName = typeof(Models.Email).GetProperties().Where(x => x.Name.ToUpper() == clientOrderByName.ToUpper()).First().Name;

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
                     .Select(x => new EmailDTO
                     {
                         EmailId = x.EmailId,
                         EmailFrom = x.EmailFrom,
                         EmailTo = x.EmailTo,
                         EmailToCc = x.EmailToCc,
                         EmailToBcc = x.EmailToBcc,
                         Subject = x.Subject,
                         Body = x.Body,
                         IsHtml = x.IsHtml,
                         IsSend = x.IsSend,
                         IsSuccess = x.IsSuccess,
                         SendDate = x.SendDate,
                         CreatedDate = x.CreatedDate,
                         CreatedUser = x.CreatedUser,
                         UpdatedDate = x.UpdatedDate,
                         UpdatedUser = x.UpdatedUser,
                         CreatedUserText = dbContext.Users.First(y => y.UserId == x.CreatedUser).UserName,
                         UpdatedUserText = dbContext.Users.First(y => y.UserId == x.UpdatedUser).UserName
                     }).ToList();

                return new PageList<EmailDTO> { Data = data, TotalCount = totalCount };

            }
        }

        public EmailDTO SaveEmail(EmailDTO emailDTO, UserInfo userInfo)
        {
            Models.Email email = new Models.Email();
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                bool isNewRecord = emailDTO.EmailId == 0;
                if (isNewRecord)
                {
                    email.CreatedDate = DateTime.Now;
                    email.CreatedUser = userInfo.UserId;
                }
                else
                {
                    email = dbContext.Emails.First(x => x.EmailId == emailDTO.EmailId);

                }

                email.UpdatedDate = DateTime.Now;
                email.UpdatedUser = userInfo.UserId;


                email.EmailFrom = emailDTO.EmailFrom;
                email.EmailTo = emailDTO.EmailTo;
                email.EmailToCc = emailDTO.EmailToCc;
                email.EmailToBcc = emailDTO.EmailToBcc;
                email.Subject = emailDTO.Subject;
                email.Body = emailDTO.Body;
                email.IsHtml = emailDTO.IsHtml;
                email.IsSend = emailDTO.IsSend;
                email.IsSuccess = emailDTO.IsSuccess;
                email.SendDate = emailDTO.SendDate;





                if (isNewRecord)
                {
                    dbContext.Emails.Add(email);
                }

                dbContext.SaveChanges();

                emailDTO.EmailId = email.EmailId;
            }

            return emailDTO;
        }


        public void DeleteEmail(EmailDTO emailDTO)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var email = dbContext.Emails.FirstOrDefault(x => x.EmailId == emailDTO.EmailId);
                if (email != null)
                {
                    dbContext.Emails.Remove(email);
                    dbContext.SaveChanges();
                }
            }
        }
    }


    public class EmailDTO
    {
        public Int32 EmailId { get; set; }
        public String EmailFrom { get; set; }
        public String EmailTo { get; set; }
        public String EmailToCc { get; set; }
        public String EmailToBcc { get; set; }
        public String Subject { get; set; }
        public String Body { get; set; }
        public Boolean? IsHtml { get; set; }
        public Boolean? IsSend { get; set; }
        public Boolean? IsSuccess { get; set; }
        public DateTime? SendDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public Int32 CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Int32 UpdatedUser { get; set; }
        public String CreatedUserText { get; set; }
        public String UpdatedUserText { get; set; }
    }

    public class EmailFilter
    {
        public Int32? EmailId { get; set; }
        public String EmailFrom { get; set; }
        public String EmailTo { get; set; }
        public String EmailToCc { get; set; }
        public String EmailToBcc { get; set; }
        public String Subject { get; set; }
        public String Body { get; set; }
        public Boolean? IsHtml { get; set; }
        public Boolean? IsSend { get; set; }
        public Boolean? IsSuccess { get; set; }
        public DateTime? SendDate { get; set; }
        public DateTime? SendDate2 { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? CreatedDate2 { get; set; }
        public Int32? CreatedUser { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? UpdatedDate2 { get; set; }
        public Int32? UpdatedUser { get; set; }
    }
}
