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
    public class EmailConfigController : ControllerBase
    {
        private readonly EmailConfigService _emailConfigService;
        private readonly UserInfo _userInfo;
        private readonly SessionHelper _sessionHelper;
        private readonly SystemLogService _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmailConfigController(IHttpContextAccessor httpContextAccessor)
        {
            _emailConfigService = new EmailConfigService();
            _sessionHelper = new SessionHelper(httpContextAccessor.HttpContext);
            _userInfo = _sessionHelper.GetCurrentUser();
            _logger = new SystemLogService();
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("GetEmailConfigList")]
        [SessionFilterAtrribute]
        public ReturnInfo<EmailConfigDTO> GetEmailConfigList([FromBody] EmailConfigListQueryParams  emailConfigListQueryParams)
        {
             ReturnInfo<EmailConfigDTO> returnInfo = new ReturnInfo<EmailConfigDTO>();
            try
            {
                var resultData = _emailConfigService.GetEmailConfigList(emailConfigListQueryParams.Filter, emailConfigListQueryParams.QueryInfo,emailConfigListQueryParams.IsExport);
               
                if (!emailConfigListQueryParams.IsExport)
                {
                    returnInfo.Data = resultData.Data;
                }
                else
                {
                    returnInfo.Key = ExcelHelper.AddListAsExcelToCache<EmailConfigDTO>(resultData.Data, emailConfigListQueryParams.ColumnInfos);
                }

                returnInfo.TotalCount = resultData.TotalCount;
                returnInfo.IsSuccess = true;

            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("EmailConfigController.GetEmailConfigList", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;
        }

        [HttpPost("SaveEmailConfig")]
        [SessionFilterAtrribute]
        public ReturnInfo<EmailConfigDTO> SaveEmailConfig([FromBody] EmailConfigDTO emailConfigDTO)
        {
            ReturnInfo<EmailConfigDTO> returnInfo = new ReturnInfo<EmailConfigDTO>();

            try
            {
                returnInfo.Data = new List<EmailConfigDTO> { _emailConfigService.SaveEmailConfig(emailConfigDTO, _userInfo) };
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.SAVED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("EmailConfigController.SaveEmailConfig", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }


        [HttpPost("DeleteEmailConfig")]
        [SessionFilterAtrribute]
        public ReturnInfo<EmailConfigDTO> DeleteEmailConfig([FromBody] EmailConfigDTO emailConfigDTO)
        {
            ReturnInfo<EmailConfigDTO> returnInfo = new ReturnInfo<EmailConfigDTO>();

            try
            {
                _emailConfigService.DeleteEmailConfig(emailConfigDTO);
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.DELETED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("EmailConfigController.DeleteEmailConfig", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }

        public class EmailConfigListQueryParams
        {
            public EmailConfigFilter Filter { get; set; }
            public QueryInfo QueryInfo { get; set; }
            public bool IsExport { get; set; }
            public List<ColumnInfo> ColumnInfos {get;set;}
         }
    }
}
