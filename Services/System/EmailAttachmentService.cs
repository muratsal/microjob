using NanoGo.Models;
using NanoGo.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using File = System.IO.File;

namespace NanoGo.Services.System
{
    public class EmailAttachmentService
    {
        public PageList<EmailAttachmentDTO> GetEmailAttachmentList(EmailAttachmentFilter filter, NanoGo.Shared.QueryInfo queryInfo, bool isExport)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var dbQuery = dbContext.EmailAttachments.OrderByDescending(x => x.EmailAttachmentId).Where(x => 1 == 1);
                int totalCount = 0;

                if (filter != null)
                {
                    if (filter.EmailAttachmentId.HasValue) dbQuery = dbQuery.Where(x => x.EmailAttachmentId == filter.EmailAttachmentId.Value);
                    if (filter.EmailId.HasValue) dbQuery = dbQuery.Where(x => x.EmailId == filter.EmailId.Value);
                    if (!string.IsNullOrEmpty(filter.Name)) dbQuery = dbQuery.Where(x => x.Name.Contains(filter.Name));
                    if (!string.IsNullOrEmpty(filter.FilePath)) dbQuery = dbQuery.Where(x => x.FilePath.Contains(filter.FilePath));
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
                        string orderByName = typeof(Models.EmailAttachment).GetProperties().Where(x => x.Name.ToUpper() == clientOrderByName.ToUpper()).First().Name;

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
                     .Select(x => new EmailAttachmentDTO
                     {
                         EmailAttachmentId = x.EmailAttachmentId,
                         EmailId = x.EmailId,
                         Name = x.Name,
                         FilePath = x.FilePath,
                         CreatedDate = x.CreatedDate,
                         CreatedUser = x.CreatedUser,
                         UpdatedDate = x.UpdatedDate,
                         UpdatedUser = x.UpdatedUser,
                         CreatedUserText = dbContext.Users.First(y => y.UserId == x.CreatedUser).UserName,
                         UpdatedUserText = dbContext.Users.First(y => y.UserId == x.UpdatedUser).UserName
                     }).ToList();

                return new PageList<EmailAttachmentDTO> { Data = data, TotalCount = totalCount };

            }
        }

        public EmailAttachmentDTO SaveEmailAttachment(EmailAttachmentDTO emailAttachmentDTO, UserInfo userInfo)
        {
            Models.EmailAttachment emailAttachment = new Models.EmailAttachment();
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                bool isNewRecord = emailAttachmentDTO.EmailAttachmentId == 0;
                if (isNewRecord)
                {
                    emailAttachment.CreatedDate = DateTime.Now;
                    emailAttachment.CreatedUser = userInfo.UserId;
                }
                else
                {
                    emailAttachment = dbContext.EmailAttachments.First(x => x.EmailAttachmentId == emailAttachmentDTO.EmailAttachmentId);

                }

                emailAttachment.UpdatedDate = DateTime.Now;
                emailAttachment.UpdatedUser = userInfo.UserId;
                emailAttachment.EmailId = emailAttachmentDTO.EmailId;
                emailAttachment.Name = emailAttachmentDTO.Name;
                emailAttachment.FilePath = emailAttachmentDTO.FilePath;

                if (isNewRecord)
                {
                    dbContext.EmailAttachments.Add(emailAttachment);
                }

                dbContext.SaveChanges();

                emailAttachmentDTO.EmailAttachmentId = emailAttachment.EmailAttachmentId;
            }

            return emailAttachmentDTO;
        }


        public void DeleteEmailAttachment(EmailAttachmentDTO emailAttachmentDTO)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var emailAttachment = dbContext.EmailAttachments.FirstOrDefault(x => x.EmailAttachmentId == emailAttachmentDTO.EmailAttachmentId);
                if (emailAttachment != null)
                {
                    dbContext.EmailAttachments.Remove(emailAttachment);
                    dbContext.SaveChanges();
                }
            }
        }

        public void AddAttachmentFileToEmail(Email email, string fileName, byte[] fileData, string path, string relativePath, int userId)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var fileNameAndExtension = fileName.Split(".").ToList();
                EmailAttachment emailAttachment = new EmailAttachment();
                emailAttachment.EmailId = email.EmailId;
                string uniqueFileName = fileNameAndExtension[0].ToString() + "_" + Guid.NewGuid().ToString() + "." + fileNameAndExtension[1].ToString(); ;

                emailAttachment.FilePath = relativePath + "/" + uniqueFileName;
                emailAttachment.Name = fileName;
                emailAttachment.CreatedDate = DateTime.Now;
                emailAttachment.UpdatedDate = DateTime.Now;
                emailAttachment.CreatedUser = userId;
                emailAttachment.UpdatedUser = userId;

                string fileFullPath = path + "/" + uniqueFileName;

                File.WriteAllBytes(fileFullPath, fileData);

                dbContext.SaveChanges();

            }
        }
    }


    public class EmailAttachmentDTO
    {
        public Int32 EmailAttachmentId { get; set; }
        public Int32? EmailId { get; set; }
        public String Name { get; set; }
        public String FilePath { get; set; }
        public DateTime CreatedDate { get; set; }
        public Int32 CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Int32 UpdatedUser { get; set; }
        public String CreatedUserText { get; set; }
        public String UpdatedUserText { get; set; }
    }

    public class EmailAttachmentFilter
    {
        public Int32? EmailAttachmentId { get; set; }
        public Int32? EmailId { get; set; }
        public String Name { get; set; }
        public String FilePath { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? CreatedDate2 { get; set; }
        public Int32? CreatedUser { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? UpdatedDate2 { get; set; }
        public Int32? UpdatedUser { get; set; }
    }
}
