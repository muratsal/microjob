using NanoGo.Jobs;
using NanoGo.Services.System;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace NanoGo
{
    public class JobMan
    {
        public System.Timers.Timer jobTimer1 = null;
        public Boolean isTimer1Working = false;
        private IWebHostEnvironment _hostEnvironment;
        private SystemLogService _logger;
        public JobMan(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            _logger = new SystemLogService();
        }

        public void SetTimer()
        {

            var jobTimer1 = new System.Timers.Timer(10000);
            jobTimer1.Elapsed += OnTimedEvent1;
            jobTimer1.AutoReset = true;
            jobTimer1.Enabled = true;
        }

        private void OnTimedEvent1(Object source, ElapsedEventArgs e)
        {
            try
            {
                if (isTimer1Working) return;
                isTimer1Working = true;

                new EmailJob(_hostEnvironment).RunJob();

               
                isTimer1Working = false;
            }
            catch (Exception ex)
            {
                isTimer1Working = false;

                new SystemLogService().AddLog("JobMan.Timer1", ex.ToString(), 1, "", "");
            }
        }

    }
}
