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
    public class CounterController : ControllerBase
    {
        private readonly CounterService _counterService;
        private readonly UserInfo _userInfo;
        private readonly SessionHelper _sessionHelper;
        private readonly SystemLogService _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CounterController(IHttpContextAccessor httpContextAccessor)
        {
            _counterService = new CounterService();
            _sessionHelper = new SessionHelper(httpContextAccessor.HttpContext);
            _userInfo = _sessionHelper.GetCurrentUser();
            _logger = new SystemLogService();
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("GetCounterList")]
        [SessionFilterAtrribute]
        public ReturnInfo<CounterDTO> GetCounterList([FromBody] CounterListQueryParams  counterListQueryParams)
        {
             ReturnInfo<CounterDTO> returnInfo = new ReturnInfo<CounterDTO>();
            try
            {
                var resultData = _counterService.GetCounterList(counterListQueryParams.Filter, counterListQueryParams.QueryInfo,counterListQueryParams.IsExport);
               
                if (!counterListQueryParams.IsExport)
                {
                    returnInfo.Data = resultData.Data;
                }
                else
                {
                    returnInfo.Key = ExcelHelper.AddListAsExcelToCache<CounterDTO>(resultData.Data, counterListQueryParams.ColumnInfos);
                }

                returnInfo.TotalCount = resultData.TotalCount;
                returnInfo.IsSuccess = true;

            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("CounterController.GetCounterList", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;
        }

        [HttpPost("SaveCounter")]
        [SessionFilterAtrribute]
        public ReturnInfo<CounterDTO> SaveCounter([FromBody] CounterDTO counterDTO)
        {
            ReturnInfo<CounterDTO> returnInfo = new ReturnInfo<CounterDTO>();

            try
            {
                returnInfo.Data = new List<CounterDTO> { _counterService.SaveCounter(counterDTO, _userInfo) };
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.SAVED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("CounterController.SaveCounter", ex.ToString(), _userInfo.UserId);
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
               var _counterListForCombo = _counterService.GetListForCombo(filter.Search, filter.Id);
                returnInfo.IsSuccess = true;
                returnInfo.Data = _counterListForCombo;
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("CounterController.GetListForCombo", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;
        }


        [HttpPost("DeleteCounter")]
        [SessionFilterAtrribute]
        public ReturnInfo<CounterDTO> DeleteCounter([FromBody] CounterDTO counterDTO)
        {
            ReturnInfo<CounterDTO> returnInfo = new ReturnInfo<CounterDTO>();

            try
            {
                _counterService.DeleteCounter(counterDTO);
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.DELETED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("CounterController.DeleteCounter", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }

        public class CounterListQueryParams
        {
            public CounterFilter Filter { get; set; }
            public QueryInfo QueryInfo { get; set; }
            public bool IsExport { get; set; }
            public List<ColumnInfo> ColumnInfos {get;set;}
         }
    }
}
