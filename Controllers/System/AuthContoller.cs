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
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly UserInfo _userInfo;
        private readonly SessionHelper _sessionHelper;
        private readonly SystemLogService _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthController(IHttpContextAccessor httpContextAccessor)
        {
            _authService = new AuthService();
            _sessionHelper = new SessionHelper(httpContextAccessor.HttpContext);
            _userInfo = _sessionHelper.GetCurrentUser();
            _logger = new SystemLogService();
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("GetAuthList")]
        [SessionFilterAtrribute]
        public ReturnInfo<AuthDTO> GetAuthList([FromBody] AuthListQueryParams  authListQueryParams)
        {
             ReturnInfo<AuthDTO> returnInfo = new ReturnInfo<AuthDTO>();
            try
            {
                var resultData = _authService.GetAuthList(authListQueryParams.Filter, authListQueryParams.QueryInfo,authListQueryParams.IsExport);
               
                if (!authListQueryParams.IsExport)
                {
                    returnInfo.Data = resultData.Data;
                }
                else
                {
                    returnInfo.Key = ExcelHelper.AddListAsExcelToCache<AuthDTO>(resultData.Data, authListQueryParams.ColumnInfos);
                }

                returnInfo.TotalCount = resultData.TotalCount;
                returnInfo.IsSuccess = true;

            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("AuthController.GetAuthList", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;
        }

        [HttpPost("SaveAuth")]
        [SessionFilterAtrribute]
        public ReturnInfo<AuthDTO> SaveAuth([FromBody] AuthDTO authDTO)
        {
            ReturnInfo<AuthDTO> returnInfo = new ReturnInfo<AuthDTO>();

            try
            {
                returnInfo.Data = new List<AuthDTO> { _authService.SaveAuth(authDTO, _userInfo) };
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.SAVED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("AuthController.SaveAuth", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }


        [HttpPost("DeleteAuth")]
        [SessionFilterAtrribute]
        public ReturnInfo<AuthDTO> DeleteAuth([FromBody] AuthDTO authDTO)
        {
            ReturnInfo<AuthDTO> returnInfo = new ReturnInfo<AuthDTO>();

            try
            {
                _authService.DeleteAuth(authDTO);
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.DELETED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("AuthController.DeleteAuth", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }

        public class AuthListQueryParams
        {
            public AuthFilter Filter { get; set; }
            public QueryInfo QueryInfo { get; set; }
            public bool IsExport { get; set; }
            public List<ColumnInfo> ColumnInfos {get;set;}
         }
    }
}
