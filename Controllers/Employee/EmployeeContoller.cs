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
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Drawing;
using System.Drawing.Imaging;

namespace NanoGo.Controllers.System
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeService _employeeService;
        private readonly UserInfo _userInfo;
        private readonly SessionHelper _sessionHelper;
        private readonly SystemLogService _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _hostEnvironment;

        public EmployeeController(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostEnvironment)
        {
            _employeeService = new EmployeeService();
            _sessionHelper = new SessionHelper(httpContextAccessor.HttpContext);
            _userInfo = _sessionHelper.GetCurrentUser();
            _logger = new SystemLogService();
            _hostEnvironment = hostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("GetEmployeeList")]
        [SessionFilterAtrribute]
        public ReturnInfo<EmployeeDTO> GetEmployeeList([FromBody] EmployeeListQueryParams  employeeListQueryParams)
        {
             ReturnInfo<EmployeeDTO> returnInfo = new ReturnInfo<EmployeeDTO>();
            try
            {
                var resultData = _employeeService.GetEmployeeList(employeeListQueryParams.Filter, employeeListQueryParams.QueryInfo,employeeListQueryParams.IsExport);
               
                if (!employeeListQueryParams.IsExport)
                {
                    returnInfo.Data = resultData.Data;
                }
                else
                {
                    returnInfo.Key = ExcelHelper.AddListAsExcelToCache<EmployeeDTO>(resultData.Data, employeeListQueryParams.ColumnInfos);
                }

                returnInfo.TotalCount = resultData.TotalCount;
                returnInfo.IsSuccess = true;

            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("EmployeeController.GetEmployeeList", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;
        }

        [HttpPost("SaveEmployee")]
        [SessionFilterAtrribute]
        public ReturnInfo<EmployeeDTO> SaveEmployee([FromBody] EmployeeDTO employeeDTO)
        {
            ReturnInfo<EmployeeDTO> returnInfo = new ReturnInfo<EmployeeDTO>();

            try
            {
                returnInfo.Data = new List<EmployeeDTO> { _employeeService.SaveEmployee(employeeDTO, _userInfo) };
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.SAVED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("EmployeeController.SaveEmployee", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }

        [HttpPost("GetListForCombo")]
        [SessionFilterAtrribute]
        public ReturnInfo<ItemForCombo> GetListForCombo([FromBody] ListForComboFilter filter,bool allData=false)
        {
            ReturnInfo<ItemForCombo> returnInfo = new ReturnInfo<ItemForCombo>();

            try
            {
               var _employeeListForCombo = _employeeService.GetListForCombo(filter.Search, filter.Id, allData);
                returnInfo.IsSuccess = true;
                returnInfo.Data = _employeeListForCombo;
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("EmployeeController.GetListForCombo", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;
        }


        [HttpPost("DeleteEmployee")]
        [SessionFilterAtrribute]
        public ReturnInfo<EmployeeDTO> DeleteEmployee([FromBody] EmployeeDTO employeeDTO)
        {
            ReturnInfo<EmployeeDTO> returnInfo = new ReturnInfo<EmployeeDTO>();

            try
            {
                _employeeService.DeleteEmployee(employeeDTO);
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.DELETED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("EmployeeController.DeleteEmployee", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }

        [HttpPost("UploadEmployeeImage")]
        [SessionFilterAtrribute]
        public ReturnInfo UploadEmployeeImage()
        {
            ReturnInfo returnInfo = new ReturnInfo();

            try
            {
                var files = _httpContextAccessor.HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    var file = files.First();
                    var uploadPath = Path.Combine(_hostEnvironment.WebRootPath, "AppImages\\Employee");
                    var uploadPathThumbnail = Path.Combine(_hostEnvironment.WebRootPath, "AppImages\\Employee\\Thumbnail");

                    if (file.Length > 0)
                    {

                        string fileName = Guid.NewGuid().ToString() + ".jpg";

                        Image image = Image.FromStream(file.OpenReadStream(), true, true);
                        var thumbnail = Utilities.ResizeImage(100, 100, image);
                        image = Image.FromStream(file.OpenReadStream(), true, true);
                        var resizedProductImage = Utilities.ResizeImage(800, 800, image);

                        thumbnail.Save(Path.Combine(uploadPathThumbnail, fileName), ImageFormat.Jpeg);
                        resizedProductImage.Save(Path.Combine(uploadPath, fileName), ImageFormat.Jpeg);

                        returnInfo.Data = Path.Combine("AppImages\\Employee", fileName);
                    }

                }

                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.SAVED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("EmployeeController.UploadEmployeeImage", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }

        public class EmployeeListQueryParams
        {
            public EmployeeFilter Filter { get; set; }
            public QueryInfo QueryInfo { get; set; }
            public bool IsExport { get; set; }
            public List<ColumnInfo> ColumnInfos {get;set;}
         }
    }
}
