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
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Hosting;

namespace NanoGo.Controllers.System
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryService _inventoryService;
        private readonly InventoryTransactionService _inventoryTransactionService;
        private readonly UserInfo _userInfo;
        private readonly SessionHelper _sessionHelper;
        private readonly SystemLogService _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _hostEnvironment;

        public InventoryController(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostEnvironment)
        {
            _inventoryService = new InventoryService();
            _sessionHelper = new SessionHelper(httpContextAccessor.HttpContext);
            _userInfo = _sessionHelper.GetCurrentUser();
            _logger = new SystemLogService();
            _httpContextAccessor = httpContextAccessor;
            _hostEnvironment = hostEnvironment;
            _inventoryTransactionService = new InventoryTransactionService();
        }

        [HttpPost("GetInventoryList")]
        [SessionFilterAtrribute]
        public ReturnInfo<InventoryDTO> GetInventoryList([FromBody] InventoryListQueryParams ınventoryListQueryParams)
        {
            ReturnInfo<InventoryDTO> returnInfo = new ReturnInfo<InventoryDTO>();
            try
            {
                var resultData = _inventoryService.GetInventoryList(ınventoryListQueryParams.Filter, ınventoryListQueryParams.QueryInfo, ınventoryListQueryParams.IsExport);

                if (!ınventoryListQueryParams.IsExport)
                {
                    returnInfo.Data = resultData.Data;
                }
                else
                {
                    returnInfo.Key = ExcelHelper.AddListAsExcelToCache<InventoryDTO>(resultData.Data, ınventoryListQueryParams.ColumnInfos);
                }

                returnInfo.TotalCount = resultData.TotalCount;
                returnInfo.IsSuccess = true;

            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("InventoryController.GetInventoryList", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;
        }

        [HttpPost("SaveInventory")]
        [SessionFilterAtrribute]
        public ReturnInfo<InventoryDTO> SaveInventory([FromBody] InventoryDTO ınventoryDTO)
        {
            ReturnInfo<InventoryDTO> returnInfo = new ReturnInfo<InventoryDTO>();

            try
            {
                Boolean isNew = ınventoryDTO.InventoryId == 0;
                returnInfo.Data = new List<InventoryDTO> { _inventoryService.SaveInventory(ınventoryDTO, _userInfo) };
              
                if (isNew)
                {
                    _inventoryTransactionService.SaveInventoryTransaction(new InventoryTransactionDTO
                    {
                        InventoryId = returnInfo.Data[0].InventoryId,
                        TransDate = DateTime.Now,
                        Note = "",
                        TransType = 1,
                        CreatedUser = _userInfo.UserId,
                        UpdatedUser = _userInfo.UserId,
                    }, _userInfo);
                }
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.SAVED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("InventoryController.SaveInventory", ex.ToString(), _userInfo.UserId);
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
                var _ınventoryListForCombo = _inventoryService.GetListForCombo(filter.Search, filter.Id);
                returnInfo.IsSuccess = true;
                returnInfo.Data = _ınventoryListForCombo;
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("InventoryController.GetListForCombo", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;
        }


        [HttpPost("DeleteInventory")]
        [SessionFilterAtrribute]
        public ReturnInfo<InventoryDTO> DeleteInventory([FromBody] InventoryDTO ınventoryDTO)
        {
            ReturnInfo<InventoryDTO> returnInfo = new ReturnInfo<InventoryDTO>();

            try
            {
                _inventoryService.DeleteInventory(ınventoryDTO);
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.DELETED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("InventoryController.DeleteInventory", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }
        [HttpPost("UploadInventoryPicture")]
        [SessionFilterAtrribute]
        public ReturnInfo UploadInventoryPicture()
        {
            ReturnInfo returnInfo = new ReturnInfo();

            try
            {
                var files = _httpContextAccessor.HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    var file = files.First();
                    var uploadPath = Path.Combine(_hostEnvironment.WebRootPath, "AppImages\\Inventory");
                    var uploadPathThumbnail = Path.Combine(_hostEnvironment.WebRootPath, "AppImages\\Inventory\\Thumbnail");

                    if (file.Length > 0)
                    {

                        string fileName = Guid.NewGuid().ToString() + ".jpg";

                        Image image = Image.FromStream(file.OpenReadStream(), true, true);
                        var thumbnail = Utilities.ResizeImage(100, 100, image);
                        image = Image.FromStream(file.OpenReadStream(), true, true);
                        var resizedProductImage = Utilities.ResizeImage(800, 800, image);

                        thumbnail.Save(Path.Combine(uploadPathThumbnail, fileName), ImageFormat.Jpeg);
                        resizedProductImage.Save(Path.Combine(uploadPath, fileName), ImageFormat.Jpeg);

                        returnInfo.Data = Path.Combine("AppImages\\Inventory", fileName);
                    }

                }

                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.SAVED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("EmployeeController.UploadInventoryPicture", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }

        public class InventoryListQueryParams
        {
            public InventoryFilter Filter { get; set; }
            public QueryInfo QueryInfo { get; set; }
            public bool IsExport { get; set; }
            public List<ColumnInfo> ColumnInfos { get; set; }
        }
    }
}
