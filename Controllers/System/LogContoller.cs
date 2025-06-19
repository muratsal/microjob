using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NanoGo.Services.System;
using NanoGo.Shared;
using NanoGo.Shared.Export;
using NanoGo.Shared.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace NanoGo.Controllers.System
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogController : ControllerBase
    {
        private readonly LogService _logService;
        private readonly UserInfo _userInfo;
        private readonly SessionHelper _sessionHelper;
        private readonly SystemLogService _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogController(IHttpContextAccessor httpContextAccessor)
        {
            _logService = new LogService();
            _sessionHelper = new SessionHelper(httpContextAccessor.HttpContext);
            _userInfo = _sessionHelper.GetCurrentUser();
            _logger = new SystemLogService();
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("GetLogList")]
        [SessionFilterAtrribute]
        public ReturnInfo<LogDTO> GetLogList([FromBody] LogListQueryParams  logListQueryParams)
        {
             ReturnInfo<LogDTO> returnInfo = new ReturnInfo<LogDTO>();
            try
            {
                var resultData = _logService.GetLogList(logListQueryParams.Filter, logListQueryParams.QueryInfo,logListQueryParams.IsExport);
               
                if (!logListQueryParams.IsExport)
                {
                    returnInfo.Data = resultData.Data;
                }
                else
                {
                    returnInfo.Key = ExcelHelper.AddListAsExcelToCache<LogDTO>(resultData.Data, logListQueryParams.ColumnInfos);
                }

                returnInfo.TotalCount = resultData.TotalCount;
                returnInfo.IsSuccess = true;

            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("LogController.GetLogList", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;
        }

        [HttpPost("SaveLog")]
        [SessionFilterAtrribute]
        public ReturnInfo<LogDTO> SaveLog([FromBody] LogDTO logDTO)
        {
            ReturnInfo<LogDTO> returnInfo = new ReturnInfo<LogDTO>();

            try
            {
                returnInfo.Data = new List<LogDTO> { _logService.SaveLog(logDTO, _userInfo) };
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.SAVED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("LogController.SaveLog", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }


        [HttpPost("DeleteLog")]
        [SessionFilterAtrribute]
        public ReturnInfo<LogDTO> DeleteLog([FromBody] LogDTO logDTO)
        {
            ReturnInfo<LogDTO> returnInfo = new ReturnInfo<LogDTO>();

            try
            {
                _logService.DeleteLog(logDTO);
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.DELETED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("LogController.DeleteLog", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }

        public class LogListQueryParams
        {
            public LogFilter Filter { get; set; }
            public QueryInfo QueryInfo { get; set; }
            public bool IsExport { get; set; }
            public List<ColumnInfo> ColumnInfos {get;set;}
         }
    }
}
