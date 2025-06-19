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
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly UserInfo _userInfo;
        private readonly SessionHelper _sessionHelper;
        private readonly SystemLogService _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmailController(IHttpContextAccessor httpContextAccessor)
        {
            _emailService = new EmailService();
            _sessionHelper = new SessionHelper(httpContextAccessor.HttpContext);
            _userInfo = _sessionHelper.GetCurrentUser();
            _logger = new SystemLogService();
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("GetEmailList")]
        [SessionFilterAtrribute]
        public ReturnInfo<EmailDTO> GetEmailList([FromBody] EmailListQueryParams  emailListQueryParams)
        {
             ReturnInfo<EmailDTO> returnInfo = new ReturnInfo<EmailDTO>();
            try
            {
                var resultData = _emailService.GetEmailList(emailListQueryParams.Filter, emailListQueryParams.QueryInfo,emailListQueryParams.IsExport);
               
                if (!emailListQueryParams.IsExport)
                {
                    returnInfo.Data = resultData.Data;
                }
                else
                {
                    returnInfo.Key = ExcelHelper.AddListAsExcelToCache<EmailDTO>(resultData.Data, emailListQueryParams.ColumnInfos);
                }

                returnInfo.TotalCount = resultData.TotalCount;
                returnInfo.IsSuccess = true;

            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("EmailController.GetEmailList", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;
        }

        [HttpPost("SaveEmail")]
        [SessionFilterAtrribute]
        public ReturnInfo<EmailDTO> SaveEmail([FromBody] EmailDTO emailDTO)
        {
            ReturnInfo<EmailDTO> returnInfo = new ReturnInfo<EmailDTO>();

            try
            {
                returnInfo.Data = new List<EmailDTO> { _emailService.SaveEmail(emailDTO, _userInfo) };
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.SAVED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("EmailController.SaveEmail", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }


        [HttpPost("DeleteEmail")]
        [SessionFilterAtrribute]
        public ReturnInfo<EmailDTO> DeleteEmail([FromBody] EmailDTO emailDTO)
        {
            ReturnInfo<EmailDTO> returnInfo = new ReturnInfo<EmailDTO>();

            try
            {
                _emailService.DeleteEmail(emailDTO);
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.DELETED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("EmailController.DeleteEmail", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }

        public class EmailListQueryParams
        {
            public EmailFilter Filter { get; set; }
            public QueryInfo QueryInfo { get; set; }
            public bool IsExport { get; set; }
            public List<ColumnInfo> ColumnInfos {get;set;}
         }
    }
}
