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
    public class UserRoleController : ControllerBase
    {
        private readonly UserRoleService _userRoleService;
        private readonly UserInfo _userInfo;
        private readonly SessionHelper _sessionHelper;
        private readonly SystemLogService _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserRoleController(IHttpContextAccessor httpContextAccessor)
        {
            _userRoleService = new UserRoleService();
            _sessionHelper = new SessionHelper(httpContextAccessor.HttpContext);
            _userInfo = _sessionHelper.GetCurrentUser();
            _logger = new SystemLogService();
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("GetUserRoleList")]
        [SessionFilterAtrribute]
        public ReturnInfo<UserRoleDTO> GetUserRoleList([FromBody] UserRoleListQueryParams  userRoleListQueryParams)
        {
             ReturnInfo<UserRoleDTO> returnInfo = new ReturnInfo<UserRoleDTO>();
            try
            {
                var resultData = _userRoleService.GetUserRoleList(userRoleListQueryParams.Filter, userRoleListQueryParams.QueryInfo,userRoleListQueryParams.IsExport);
               
                if (!userRoleListQueryParams.IsExport)
                {
                    returnInfo.Data = resultData.Data;
                }
                else
                {
                    returnInfo.Key = ExcelHelper.AddListAsExcelToCache<UserRoleDTO>(resultData.Data, userRoleListQueryParams.ColumnInfos);
                }

                returnInfo.TotalCount = resultData.TotalCount;
                returnInfo.IsSuccess = true;

            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("UserRoleController.GetUserRoleList", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;
        }

        [HttpPost("SaveUserRole")]
        [SessionFilterAtrribute]
        public ReturnInfo<UserRoleDTO> SaveUserRole([FromBody] UserRoleDTO userRoleDTO)
        {
            ReturnInfo<UserRoleDTO> returnInfo = new ReturnInfo<UserRoleDTO>();

            try
            {
                returnInfo.Data = new List<UserRoleDTO> { _userRoleService.SaveUserRole(userRoleDTO, _userInfo) };
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.SAVED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("UserRoleController.SaveUserRole", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }


        [HttpPost("DeleteUserRole")]
        [SessionFilterAtrribute]
        public ReturnInfo<UserRoleDTO> DeleteUserRole([FromBody] UserRoleDTO userRoleDTO)
        {
            ReturnInfo<UserRoleDTO> returnInfo = new ReturnInfo<UserRoleDTO>();

            try
            {
                _userRoleService.DeleteUserRole(userRoleDTO);
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.DELETED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("UserRoleController.DeleteUserRole", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }

        public class UserRoleListQueryParams
        {
            public UserRoleFilter Filter { get; set; }
            public QueryInfo QueryInfo { get; set; }
            public bool IsExport { get; set; }
            public List<ColumnInfo> ColumnInfos {get;set;}
         }
    }
}
