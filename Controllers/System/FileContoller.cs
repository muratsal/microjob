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

namespace NanoGo.Controllers.System
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly FileService _fileService;
        private readonly UserInfo _userInfo;
        private readonly SessionHelper _sessionHelper;
        private readonly SystemLogService _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _hostEnvironment;

        public FileController(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostEnvironment)
        {
            _fileService = new FileService();
            _sessionHelper = new SessionHelper(httpContextAccessor.HttpContext);
            _userInfo = _sessionHelper.GetCurrentUser();
            _logger = new SystemLogService();
            _httpContextAccessor = httpContextAccessor;
            _hostEnvironment = hostEnvironment;
        }

        [HttpPost("GetFileList")]
        [SessionFilterAtrribute]
        public ReturnInfo<FileDTO> GetFileList([FromBody] FileListQueryParams fileListQueryParams)
        {
            ReturnInfo<FileDTO> returnInfo = new ReturnInfo<FileDTO>();
            try
            {
                var resultData = _fileService.GetFileList(fileListQueryParams.Filter, fileListQueryParams.QueryInfo, fileListQueryParams.IsExport, _userInfo);

                if (!fileListQueryParams.IsExport)
                {
                    returnInfo.Data = resultData.Data;
                }
                else
                {
                    returnInfo.Key = ExcelHelper.AddListAsExcelToCache<FileDTO>(resultData.Data, fileListQueryParams.ColumnInfos);
                }

                returnInfo.TotalCount = resultData.TotalCount;
                returnInfo.IsSuccess = true;

            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("FileController.GetFileList", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;
        }

        [HttpPost("SaveFile")]
        [SessionFilterAtrribute]
        public ReturnInfo<FileDTO> SaveFile([FromBody] FileDTO fileDTO)
        {
            ReturnInfo<FileDTO> returnInfo = new ReturnInfo<FileDTO>();

            try
            {
                returnInfo.Data = new List<FileDTO> { _fileService.SaveFile(fileDTO, _userInfo) };
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.SAVED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("FileController.SaveFile", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }

        [HttpPost("BulkSaveFile")]
        [SessionFilterAtrribute]
        public ReturnInfo<FileDTO> BulkSaveFile([FromBody] List<FileDTO> filesDTO)
        {
            ReturnInfo<FileDTO> returnInfo = new ReturnInfo<FileDTO>();

            try
            {
                filesDTO.ForEach(file =>
                {
                    _fileService.SaveFile(file, _userInfo);
                });

                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.SAVED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("FileController.BulkSaveFile", ex.ToString(), _userInfo.UserId);
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
                var _fileListForCombo = _fileService.GetListForCombo(filter.Search, filter.Id);
                returnInfo.IsSuccess = true;
                returnInfo.Data = _fileListForCombo;
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("FileController.GetListForCombo", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;
        }


        [HttpPost("DeleteFile")]
        [SessionFilterAtrribute]
        public ReturnInfo<FileDTO> DeleteFile([FromBody] FileDTO fileDTO)
        {
            ReturnInfo<FileDTO> returnInfo = new ReturnInfo<FileDTO>();

            try
            {
                _fileService.DeleteFile(fileDTO);
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.DELETED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("FileController.DeleteFile", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }


        [HttpPost("BulkDeleteFile")]
        [SessionFilterAtrribute]
        public ReturnInfo<FileDTO> BulkDeleteFile([FromBody] List<FileDTO> fileDTOs)
        {
            ReturnInfo<FileDTO> returnInfo = new ReturnInfo<FileDTO>();

            try
            {
                fileDTOs.ForEach(fileDTO =>
                {
                    _fileService.DeleteFile(fileDTO);

                });

                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.DELETED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("FileController.BulkDeleteFile", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }

        [HttpPost("UploadFile")]
        [SessionFilterAtrribute]
        public ReturnInfo UploadFile()
        {
            ReturnInfo returnInfo = new ReturnInfo();
            try
            {
                var files = _httpContextAccessor.HttpContext.Request.Form.Files;
                var path = _httpContextAccessor.HttpContext.Request.Form["path"].ToString();
                var tableNo = Int32.Parse(_httpContextAccessor.HttpContext.Request.Form["tableNo"].ToString());
                var refNo = Int32.Parse(_httpContextAccessor.HttpContext.Request.Form["tableReferenceId"].ToString());
                var description = _httpContextAccessor.HttpContext.Request.Form["description"].ToString();
                Boolean displayByCustomer = _httpContextAccessor.HttpContext.Request.Form["displayByCustomer"].ToString().ToUpper() =="TRUE" ? true :false;
                if (files.Count > 0)
                {
                    foreach (var item in files)
                    {
                        var guidName = Guid.NewGuid().ToString();
                        FileDTO fileDTO = new FileDTO();
                        fileDTO.TableNo = tableNo;
                        fileDTO.TableReferenceId = refNo;
                        fileDTO.Description = description;
                        fileDTO.DisplayByCustomer = displayByCustomer;
                        fileDTO.FileOriginalName = item.FileName;
                        fileDTO.FileSize = (double)item.Length;
                        fileDTO.FilePath = Path.Combine(path, guidName + Path.GetExtension(item.FileName));

                        _fileService.SaveFile(fileDTO, _userInfo);
                        var uploadPath = _hostEnvironment.WebRootPath + fileDTO.FilePath;

                        using (Stream fileStream = new FileStream(uploadPath, FileMode.Create))
                        {
                            item.CopyTo(fileStream);
                        }

                    }

                }

                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.SAVED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("FileController.SaveProduct", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }


        public class FileListQueryParams
        {
            public FileFilter Filter { get; set; }
            public QueryInfo QueryInfo { get; set; }
            public bool IsExport { get; set; }
            public List<ColumnInfo> ColumnInfos { get; set; }
        }

        public enum FILE_TABLE_NOS
        {
            PRICINGRESEARCHITEMS = 1,
        }
    }
}
