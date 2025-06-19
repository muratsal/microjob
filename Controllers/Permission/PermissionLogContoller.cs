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
    public class PermissionLogController : ControllerBase
    {
        private readonly PermissionLogService _permissionLogService;
        private readonly UserInfo _userInfo;
        private readonly SessionHelper _sessionHelper;
        private readonly SystemLogService _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionLogController(IHttpContextAccessor httpContextAccessor)
        {
            _permissionLogService = new PermissionLogService();
            _sessionHelper = new SessionHelper(httpContextAccessor.HttpContext);
            _userInfo = _sessionHelper.GetCurrentUser();
            _logger = new SystemLogService();
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("GetPermissionLogList")]
        [SessionFilterAtrribute]
        public ReturnInfo<PermissionLogDTO> GetPermissionLogList([FromBody] PermissionLogListQueryParams  permissionLogListQueryParams)
        {
             ReturnInfo<PermissionLogDTO> returnInfo = new ReturnInfo<PermissionLogDTO>();
            try
            {
                var resultData = _permissionLogService.GetPermissionLogList(permissionLogListQueryParams.Filter, permissionLogListQueryParams.QueryInfo,permissionLogListQueryParams.IsExport);
               
                if (!permissionLogListQueryParams.IsExport)
                {
                    returnInfo.Data = resultData.Data;
                }
                else
                {
                    returnInfo.Key = ExcelHelper.AddListAsExcelToCache<PermissionLogDTO>(resultData.Data, permissionLogListQueryParams.ColumnInfos);
                }

                returnInfo.TotalCount = resultData.TotalCount;
                returnInfo.IsSuccess = true;

            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("PermissionLogController.GetPermissionLogList", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;
        }

        [HttpPost("SavePermissionLog")]
        [SessionFilterAtrribute]
        public ReturnInfo<PermissionLogDTO> SavePermissionLog([FromBody] PermissionLogDTO permissionLogDTO)
        {
            ReturnInfo<PermissionLogDTO> returnInfo = new ReturnInfo<PermissionLogDTO>();

            try
            {
                returnInfo.Data = new List<PermissionLogDTO> { _permissionLogService.SavePermissionLog(permissionLogDTO, _userInfo) };
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.SAVED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("PermissionLogController.SavePermissionLog", ex.ToString(), _userInfo.UserId);
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
               var _permissionLogListForCombo = _permissionLogService.GetListForCombo(filter.Search, filter.Id);
                returnInfo.IsSuccess = true;
                returnInfo.Data = _permissionLogListForCombo;
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("PermissionLogController.GetListForCombo", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;
        }


        [HttpPost("DeletePermissionLog")]
        [SessionFilterAtrribute]
        public ReturnInfo<PermissionLogDTO> DeletePermissionLog([FromBody] PermissionLogDTO permissionLogDTO)
        {
            ReturnInfo<PermissionLogDTO> returnInfo = new ReturnInfo<PermissionLogDTO>();

            try
            {
                _permissionLogService.DeletePermissionLog(permissionLogDTO);
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.DELETED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("PermissionLogController.DeletePermissionLog", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }

        public class PermissionLogListQueryParams
        {
            public PermissionLogFilter Filter { get; set; }
            public QueryInfo QueryInfo { get; set; }
            public bool IsExport { get; set; }
            public List<ColumnInfo> ColumnInfos {get;set;}
         }
    }
}
