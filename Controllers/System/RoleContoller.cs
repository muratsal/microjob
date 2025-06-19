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
    public class RoleController : ControllerBase
    {
        private readonly RoleService _roleService;
        private readonly RoleAuthService _roleAuthService;
        private readonly UserInfo _userInfo;
        private readonly SessionHelper _sessionHelper;
        private readonly SystemLogService _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RoleController(IHttpContextAccessor httpContextAccessor)
        {
            _roleService = new RoleService();
            _roleAuthService = new RoleAuthService();
            _sessionHelper = new SessionHelper(httpContextAccessor.HttpContext);
            _userInfo = _sessionHelper.GetCurrentUser();
            _logger = new SystemLogService();
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("GetRoleList")]
        [SessionFilterAtrribute]
        public ReturnInfo<RoleDTO> GetRoleList([FromBody] RoleListQueryParams roleListQueryParams)
        {
            ReturnInfo<RoleDTO> returnInfo = new ReturnInfo<RoleDTO>();
            try
            {
                var resultData = _roleService.GetRoleList(roleListQueryParams.Filter, roleListQueryParams.QueryInfo, roleListQueryParams.IsExport);

                if (!roleListQueryParams.IsExport)
                {
                    returnInfo.Data = resultData.Data;
                }
                else
                {
                    returnInfo.Key = ExcelHelper.AddListAsExcelToCache<RoleDTO>(resultData.Data, roleListQueryParams.ColumnInfos);
                }

                returnInfo.TotalCount = resultData.TotalCount;
                returnInfo.IsSuccess = true;

            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("RoleController.GetRoleList", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;
        }

        [HttpPost("SaveRole")]
        [SessionFilterAtrribute]
        public ReturnInfo<RoleDTO> SaveRole([FromBody] RoleDTO roleDTO)
        {
            ReturnInfo<RoleDTO> returnInfo = new ReturnInfo<RoleDTO>();

            try
            {
                returnInfo.Data = new List<RoleDTO> { _roleService.SaveRole(roleDTO, _userInfo) };
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.SAVED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("RoleController.SaveRole", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }

        [HttpPost("SaveRoleWithAuths")]
        [SessionFilterAtrribute]
        public ReturnInfo<RoleDTO> SaveRoleWithAuths([FromBody] RoleWithAuthDTO roleWithAuthDTO)
        {
            ReturnInfo<RoleDTO> returnInfo = new ReturnInfo<RoleDTO>();

            try
            {
                var roleList = new List<RoleDTO> { _roleService.SaveRole(roleWithAuthDTO.Role, _userInfo) };

                var roleAuths = _roleAuthService.GetRoleAuthList(new RoleAuthFilter { RoleId = roleList[0].RoleId }, null, false);

                roleAuths.Data.ForEach(x =>
                {
                    _roleAuthService.DeleteRoleAuth(x);
                });

                roleWithAuthDTO.AuthInfos.ForEach(x =>
                {
                    if (x.IsRoleAuth)
                        _roleAuthService.SaveRoleAuth(new RoleAuthDTO { AuthId = x.AuthId, RoleId = roleList[0].RoleId }, _userInfo);
                });

                returnInfo.Data = roleList;
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.SAVED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("UserController.SaveUser", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }


        [HttpPost("DeleteRole")]
        [SessionFilterAtrribute]
        public ReturnInfo<RoleDTO> DeleteRole([FromBody] RoleDTO roleDTO)
        {
            ReturnInfo<RoleDTO> returnInfo = new ReturnInfo<RoleDTO>();

            try
            {
                _roleService.DeleteRole(roleDTO);
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.DELETED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("RoleController.DeleteRole", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }


        [HttpPost("GetAuthInfo")]
        [SessionFilterAtrribute]
        public ReturnInfo<RoleAuthInfo> GetAuthInfo([FromBody] RoleDTO roleDTO)
        {
            ReturnInfo<RoleAuthInfo> returnInfo = new ReturnInfo<RoleAuthInfo>();

            try
            {
                var userRoleInfo = _roleService.GetAuthInfo(roleDTO.RoleId);
                returnInfo.IsSuccess = true;
                returnInfo.Data = userRoleInfo;
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("RoleController.GetAuthInfo", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;
        }

        public class RoleListQueryParams
        {
            public RoleFilter Filter { get; set; }
            public QueryInfo QueryInfo { get; set; }
            public bool IsExport { get; set; }
            public List<ColumnInfo> ColumnInfos { get; set; }
        }

        public class RoleWithAuthDTO
        {
            public RoleDTO Role { get; set; }
            public List<RoleAuthInfo> AuthInfos { get; set; }
        }
    }
}
