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
    public class EmailAttachmentController : ControllerBase
    {
        private readonly EmailAttachmentService _emailAttachmentService;
        private readonly UserInfo _userInfo;
        private readonly SessionHelper _sessionHelper;
        private readonly SystemLogService _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmailAttachmentController(IHttpContextAccessor httpContextAccessor)
        {
            _emailAttachmentService = new EmailAttachmentService();
            _sessionHelper = new SessionHelper(httpContextAccessor.HttpContext);
            _userInfo = _sessionHelper.GetCurrentUser();
            _logger = new SystemLogService();
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("GetEmailAttachmentList")]
        [SessionFilterAtrribute]
        public ReturnInfo<EmailAttachmentDTO> GetEmailAttachmentList([FromBody] EmailAttachmentListQueryParams  emailAttachmentListQueryParams)
        {
             ReturnInfo<EmailAttachmentDTO> returnInfo = new ReturnInfo<EmailAttachmentDTO>();
            try
            {
                var resultData = _emailAttachmentService.GetEmailAttachmentList(emailAttachmentListQueryParams.Filter, emailAttachmentListQueryParams.QueryInfo,emailAttachmentListQueryParams.IsExport);
               
                if (!emailAttachmentListQueryParams.IsExport)
                {
                    returnInfo.Data = resultData.Data;
                }
                else
                {
                    returnInfo.Key = ExcelHelper.AddListAsExcelToCache<EmailAttachmentDTO>(resultData.Data, emailAttachmentListQueryParams.ColumnInfos);
                }

                returnInfo.TotalCount = resultData.TotalCount;
                returnInfo.IsSuccess = true;

            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("EmailAttachmentController.GetEmailAttachmentList", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;
        }

        [HttpPost("SaveEmailAttachment")]
        [SessionFilterAtrribute]
        public ReturnInfo<EmailAttachmentDTO> SaveEmailAttachment([FromBody] EmailAttachmentDTO emailAttachmentDTO)
        {
            ReturnInfo<EmailAttachmentDTO> returnInfo = new ReturnInfo<EmailAttachmentDTO>();

            try
            {
                returnInfo.Data = new List<EmailAttachmentDTO> { _emailAttachmentService.SaveEmailAttachment(emailAttachmentDTO, _userInfo) };
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.SAVED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("EmailAttachmentController.SaveEmailAttachment", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }


        [HttpPost("DeleteEmailAttachment")]
        [SessionFilterAtrribute]
        public ReturnInfo<EmailAttachmentDTO> DeleteEmailAttachment([FromBody] EmailAttachmentDTO emailAttachmentDTO)
        {
            ReturnInfo<EmailAttachmentDTO> returnInfo = new ReturnInfo<EmailAttachmentDTO>();

            try
            {
                _emailAttachmentService.DeleteEmailAttachment(emailAttachmentDTO);
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.DELETED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("EmailAttachmentController.DeleteEmailAttachment", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }

        public class EmailAttachmentListQueryParams
        {
            public EmailAttachmentFilter Filter { get; set; }
            public QueryInfo QueryInfo { get; set; }
            public bool IsExport { get; set; }
            public List<ColumnInfo> ColumnInfos {get;set;}
         }
    }
}
