using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NanoGo.Services.System
{
    public class SystemLogService
    {
        private static LogService _logService;
        public void AddLog(string source, string message, int userId, string freeText = "", string freeText2 = "")
        {
            if (_logService == null) _logService = new LogService();

            _logService.AddLog(source, message, freeText, freeText2, "", LOG_TYPES.ERROR, userId);

        }

        public static void AddDBLog(string source, string message, int userId, string freeText = "", string freeText2 = "")
        {
            if (_logService == null) _logService = new LogService();

            _logService.AddLog(source, message, freeText, freeText2, "", LOG_TYPES.ERROR, userId);

        }
    }
}
