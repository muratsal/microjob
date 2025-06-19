using NanoGo.Models;
using NanoGo.Services.System;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using LMS.Jobs;

namespace NanoGo.Jobs
{
    public class EmailJob
    {
        private IWebHostEnvironment _hostEnvironment;
        public  EmailJob (IWebHostEnvironment environment)
        {
            _hostEnvironment = environment;
        }
        public void RunJob()
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {

                EmailConfig emailConfig = null;
                var emailsToSend = dbContext.Emails
                    .OrderByDescending(x => x.EmailId)
                    .Where(x => x.IsSend == false)
                    .Take(20)
                    .ToList();

                if (emailsToSend.Count > 0)
                {
                    emailConfig = dbContext.EmailConfigs.FirstOrDefault(x => x.IsActive == true);

                    if (emailConfig == null)
                    {
                        throw new Exception("Active email config not found!");
                    }

                    emailsToSend.ForEach(x =>
                    {
                        try
                        {
                            var emailAttachments =  dbContext.EmailAttachments.Where(y => y.EmailId == x.EmailId).ToList();

                            //EmailHelper.SendMail(emailConfig,x, emailAttachments, _hostEnvironment);
                            EmailHelper.SendEmailFromGoogleService(emailConfig, x, emailAttachments, _hostEnvironment);
                            x.IsSend = true;
                            x.IsSuccess = true;
                            x.SendDate = DateTime.Now;
                            dbContext.SaveChanges();

                        }
                        catch (Exception ex)
                        {

                            x.IsSend = true;
                            x.IsSuccess = false;
                            dbContext.SaveChanges();

                            SystemLogService.AddDBLog("EmailJob", ex.ToString(), 1, x.EmailId.ToString(), "");
                        }

                    });
                }
                else
                {
                   
                }
            }
        }
    }
}
