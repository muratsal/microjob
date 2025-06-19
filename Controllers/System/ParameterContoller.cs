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
    public class ParameterController : ControllerBase
    {
        private readonly ParameterService _parameterService;
        private readonly UserInfo _userInfo;
        private readonly SessionHelper _sessionHelper;
        private readonly SystemLogService _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ParameterController(IHttpContextAccessor httpContextAccessor)
        {
            _parameterService = new ParameterService();
            _sessionHelper = new SessionHelper(httpContextAccessor.HttpContext);
            _userInfo = _sessionHelper.GetCurrentUser();
            _logger = new SystemLogService();
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("GetParameterList")]
        [SessionFilterAtrribute]
        public ReturnInfo<ParameterDTO> GetParameterList([FromBody] ParameterListQueryParams  parameterListQueryParams)
        {
             ReturnInfo<ParameterDTO> returnInfo = new ReturnInfo<ParameterDTO>();
            try
            {
                var resultData = _parameterService.GetParameterList(parameterListQueryParams.Filter, parameterListQueryParams.QueryInfo,parameterListQueryParams.IsExport);
               
                if (!parameterListQueryParams.IsExport)
                {
                    returnInfo.Data = resultData.Data;
                }
                else
                {
                    returnInfo.Key = ExcelHelper.AddListAsExcelToCache<ParameterDTO>(resultData.Data, parameterListQueryParams.ColumnInfos);
                }

                returnInfo.TotalCount = resultData.TotalCount;
                returnInfo.IsSuccess = true;

            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("ParameterController.GetParameterList", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;
        }

        [HttpPost("SaveParameter")]
        [SessionFilterAtrribute]
        public ReturnInfo<ParameterDTO> SaveParameter([FromBody] ParameterDTO parameterDTO)
        {
            ReturnInfo<ParameterDTO> returnInfo = new ReturnInfo<ParameterDTO>();

            try
            {
                returnInfo.Data = new List<ParameterDTO> { _parameterService.SaveParameter(parameterDTO, _userInfo) };
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.SAVED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("ParameterController.SaveParameter", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }


        [HttpPost("DeleteParameter")]
        [SessionFilterAtrribute]
        public ReturnInfo<ParameterDTO> DeleteParameter([FromBody] ParameterDTO parameterDTO)
        {
            ReturnInfo<ParameterDTO> returnInfo = new ReturnInfo<ParameterDTO>();

            try
            {
                _parameterService.DeleteParameter(parameterDTO);
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.DELETED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("ParameterController.DeleteParameter", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }

        public class ParameterListQueryParams
        {
            public ParameterFilter Filter { get; set; }
            public QueryInfo QueryInfo { get; set; }
            public bool IsExport { get; set; }
            public List<ColumnInfo> ColumnInfos {get;set;}
         }
    }
}
