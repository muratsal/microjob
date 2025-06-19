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
    public class InventoryTransactionController : ControllerBase
    {
        private readonly InventoryTransactionService _ınventoryTransactionService;
        private readonly UserInfo _userInfo;
        private readonly SessionHelper _sessionHelper;
        private readonly SystemLogService _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InventoryTransactionController(IHttpContextAccessor httpContextAccessor)
        {
            _ınventoryTransactionService = new InventoryTransactionService();
            _sessionHelper = new SessionHelper(httpContextAccessor.HttpContext);
            _userInfo = _sessionHelper.GetCurrentUser();
            _logger = new SystemLogService();
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("GetInventoryTransactionList")]
        [SessionFilterAtrribute]
        public ReturnInfo<InventoryTransactionDTO> GetInventoryTransactionList([FromBody] InventoryTransactionListQueryParams  ınventoryTransactionListQueryParams)
        {
             ReturnInfo<InventoryTransactionDTO> returnInfo = new ReturnInfo<InventoryTransactionDTO>();
            try
            {
                var resultData = _ınventoryTransactionService.GetInventoryTransactionList(ınventoryTransactionListQueryParams.Filter, ınventoryTransactionListQueryParams.QueryInfo,ınventoryTransactionListQueryParams.IsExport);
               
                if (!ınventoryTransactionListQueryParams.IsExport)
                {
                    returnInfo.Data = resultData.Data;
                }
                else
                {
                    returnInfo.Key = ExcelHelper.AddListAsExcelToCache<InventoryTransactionDTO>(resultData.Data, ınventoryTransactionListQueryParams.ColumnInfos);
                }

                returnInfo.TotalCount = resultData.TotalCount;
                returnInfo.IsSuccess = true;

            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("InventoryTransactionController.GetInventoryTransactionList", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;
        }

        [HttpPost("SaveInventoryTransaction")]
        [SessionFilterAtrribute]
        public ReturnInfo<InventoryTransactionDTO> SaveInventoryTransaction([FromBody] InventoryTransactionDTO ınventoryTransactionDTO)
        {
            ReturnInfo<InventoryTransactionDTO> returnInfo = new ReturnInfo<InventoryTransactionDTO>();

            try
            {
                returnInfo.Data = new List<InventoryTransactionDTO> { _ınventoryTransactionService.SaveInventoryTransaction(ınventoryTransactionDTO, _userInfo) };
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.SAVED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("InventoryTransactionController.SaveInventoryTransaction", ex.ToString(), _userInfo.UserId);
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
               var _ınventoryTransactionListForCombo = _ınventoryTransactionService.GetListForCombo(filter.Search, filter.Id);
                returnInfo.IsSuccess = true;
                returnInfo.Data = _ınventoryTransactionListForCombo;
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("InventoryTransactionController.GetListForCombo", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;
        }


        [HttpPost("DeleteInventoryTransaction")]
        [SessionFilterAtrribute]
        public ReturnInfo<InventoryTransactionDTO> DeleteInventoryTransaction([FromBody] InventoryTransactionDTO ınventoryTransactionDTO)
        {
            ReturnInfo<InventoryTransactionDTO> returnInfo = new ReturnInfo<InventoryTransactionDTO>();

            try
            {
                _ınventoryTransactionService.DeleteInventoryTransaction(ınventoryTransactionDTO);
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.DELETED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("InventoryTransactionController.DeleteInventoryTransaction", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }

        public class InventoryTransactionListQueryParams
        {
            public InventoryTransactionFilter Filter { get; set; }
            public QueryInfo QueryInfo { get; set; }
            public bool IsExport { get; set; }
            public List<ColumnInfo> ColumnInfos {get;set;}
         }
    }
}
