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
    public class RoleAuthController : ControllerBase
    {
        private readonly RoleAuthService _roleAuthService;
        private readonly UserInfo _userInfo;
        private readonly SessionHelper _sessionHelper;
        private readonly SystemLogService _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RoleAuthController(IHttpContextAccessor httpContextAccessor)
        {
            _roleAuthService = new RoleAuthService();
            _sessionHelper = new SessionHelper(httpContextAccessor.HttpContext);
            _userInfo = _sessionHelper.GetCurrentUser();
            _logger = new SystemLogService();
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("GetRoleAuthList")]
        [SessionFilterAtrribute]
        public ReturnInfo<RoleAuthDTO> GetRoleAuthList([FromBody] RoleAuthListQueryParams  roleAuthListQueryParams)
        {
             ReturnInfo<RoleAuthDTO> returnInfo = new ReturnInfo<RoleAuthDTO>();
            try
            {
                var resultData = _roleAuthService.GetRoleAuthList(roleAuthListQueryParams.Filter, roleAuthListQueryParams.QueryInfo,roleAuthListQueryParams.IsExport);
               
                if (!roleAuthListQueryParams.IsExport)
                {
                    returnInfo.Data = resultData.Data;
                }
                else
                {
                    returnInfo.Key = ExcelHelper.AddListAsExcelToCache<RoleAuthDTO>(resultData.Data, roleAuthListQueryParams.ColumnInfos);
                }

                returnInfo.TotalCount = resultData.TotalCount;
                returnInfo.IsSuccess = true;

            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("RoleAuthController.GetRoleAuthList", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;
        }

        [HttpPost("SaveRoleAuth")]
        [SessionFilterAtrribute]
        public ReturnInfo<RoleAuthDTO> SaveRoleAuth([FromBody] RoleAuthDTO roleAuthDTO)
        {
            ReturnInfo<RoleAuthDTO> returnInfo = new ReturnInfo<RoleAuthDTO>();

            try
            {
                returnInfo.Data = new List<RoleAuthDTO> { _roleAuthService.SaveRoleAuth(roleAuthDTO, _userInfo) };
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.SAVED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("RoleAuthController.SaveRoleAuth", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }


        [HttpPost("DeleteRoleAuth")]
        [SessionFilterAtrribute]
        public ReturnInfo<RoleAuthDTO> DeleteRoleAuth([FromBody] RoleAuthDTO roleAuthDTO)
        {
            ReturnInfo<RoleAuthDTO> returnInfo = new ReturnInfo<RoleAuthDTO>();

            try
            {
                _roleAuthService.DeleteRoleAuth(roleAuthDTO);
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.DELETED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("RoleAuthController.DeleteRoleAuth", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }

        public class RoleAuthListQueryParams
        {
            public RoleAuthFilter Filter { get; set; }
            public QueryInfo QueryInfo { get; set; }
            public bool IsExport { get; set; }
            public List<ColumnInfo> ColumnInfos {get;set;}
         }
    }
}
