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
using Microsoft.AspNetCore.Hosting;

namespace NanoGo.Controllers.System
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionController : ControllerBase
    {
        private readonly PermissionService _permissionService;
        private readonly UserInfo _userInfo;
        private readonly SessionHelper _sessionHelper;
        private readonly SystemLogService _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _hostEnvironment;

        public PermissionController(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostEnvironment)
        {
            _permissionService = new PermissionService();
            PermissionService.hostEnvironment = hostEnvironment;
            _sessionHelper = new SessionHelper(httpContextAccessor.HttpContext);
            _userInfo = _sessionHelper.GetCurrentUser();
            _logger = new SystemLogService();
            _httpContextAccessor = httpContextAccessor;
            _hostEnvironment = hostEnvironment;
        }

        [HttpPost("GetPermissionList")]
        [SessionFilterAtrribute]
        public ReturnInfo<PermissionDTO> GetPermissionList([FromBody] PermissionListQueryParams permissionListQueryParams)
        {
            ReturnInfo<PermissionDTO> returnInfo = new ReturnInfo<PermissionDTO>();
            try
            {
                var resultData = _permissionService.GetPermissionList(permissionListQueryParams.Filter, permissionListQueryParams.QueryInfo, permissionListQueryParams.IsExport, _userInfo);

                if (!permissionListQueryParams.IsExport)
                {
                    returnInfo.Data = resultData.Data;
                }
                else
                {
                    returnInfo.Key = ExcelHelper.AddListAsExcelToCache<PermissionDTO>(resultData.Data, permissionListQueryParams.ColumnInfos);
                }

                returnInfo.TotalCount = resultData.TotalCount;
                returnInfo.IsSuccess = true;

            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("PermissionController.GetPermissionList", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;
        }

        [HttpPost("SavePermission")]
        [SessionFilterAtrribute]
        public ReturnInfo<PermissionDTO> SavePermission([FromBody] PermissionDTO permissionDTO)
        {
            ReturnInfo<PermissionDTO> returnInfo = new ReturnInfo<PermissionDTO>();

            try
            {
                Boolean isNew = permissionDTO.PermissionId == 0;
                returnInfo.Data = new List<PermissionDTO> { _permissionService.SavePermission(permissionDTO, _userInfo) };

                if (isNew)
                {
                    _permissionService.ControlComfirmation(returnInfo.Data[0].PermissionId, null, _userInfo);
                }

                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.SAVED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("PermissionController.SavePermission", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }
        [HttpPost("ComfirmPermission")]
        [SessionFilterAtrribute]
        public ReturnInfo ComfirmPermission([FromBody] PermissionDTO permissionDTO, Boolean isComfirmed)
        {
            ReturnInfo returnInfo = new ReturnInfo();

            try
            {
                _permissionService.ControlComfirmation(permissionDTO.PermissionId, isComfirmed, _userInfo);

                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.SAVED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("PermissionController.CommfirmPermission", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }

        [HttpPost("GetListForCombo")]
        [SessionFilterAtrribute]
        public ReturnInfo<ItemForCombo> GetListForCombo([FromBody] ListForComboFilter filter)
        {
            ReturnInfo<ItemForCombo> returnInfo = new ReturnInfo<ItemForCombo>();

            try
            {
                var _permissionListForCombo = _permissionService.GetListForCombo(filter.Search, filter.Id);
                returnInfo.IsSuccess = true;
                returnInfo.Data = _permissionListForCombo;
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("PermissionController.GetListForCombo", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;
        }


        [HttpPost("DeletePermission")]
        [SessionFilterAtrribute]
        public ReturnInfo<PermissionDTO> DeletePermission([FromBody] PermissionDTO permissionDTO)
        {
            ReturnInfo<PermissionDTO> returnInfo = new ReturnInfo<PermissionDTO>();

            try
            {
                _permissionService.DeletePermission(permissionDTO);
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.DELETED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("PermissionController.DeletePermission", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }

        public class PermissionListQueryParams
        {
            public PermissionFilter Filter { get; set; }
            public QueryInfo QueryInfo { get; set; }
            public bool IsExport { get; set; }
            public List<ColumnInfo> ColumnInfos { get; set; }
        }
    }
}
