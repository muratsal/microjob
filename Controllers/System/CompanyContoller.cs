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
    public class CompanyController : ControllerBase
    {
        private readonly CompanyService _companyService;
        private readonly UserInfo _userInfo;
        private readonly SessionHelper _sessionHelper;
        private readonly SystemLogService _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CompanyController(IHttpContextAccessor httpContextAccessor)
        {
            _companyService = new CompanyService();
            _sessionHelper = new SessionHelper(httpContextAccessor.HttpContext);
            _userInfo = _sessionHelper.GetCurrentUser();
            _logger = new SystemLogService();
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("GetCompanyList")]
        [SessionFilterAtrribute]
        public ReturnInfo<CompanyDTO> GetCompanyList([FromBody] CompanyListQueryParams  companyListQueryParams)
        {
             ReturnInfo<CompanyDTO> returnInfo = new ReturnInfo<CompanyDTO>();
            try
            {
                var resultData = _companyService.GetCompanyList(companyListQueryParams.Filter, companyListQueryParams.QueryInfo,companyListQueryParams.IsExport);
               
                if (!companyListQueryParams.IsExport)
                {
                    returnInfo.Data = resultData.Data;
                }
                else
                {
                    returnInfo.Key = ExcelHelper.AddListAsExcelToCache<CompanyDTO>(resultData.Data, companyListQueryParams.ColumnInfos);
                }

                returnInfo.TotalCount = resultData.TotalCount;
                returnInfo.IsSuccess = true;

            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("CompanyController.GetCompanyList", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;
        }

        [HttpPost("SaveCompany")]
        [SessionFilterAtrribute]
        public ReturnInfo<CompanyDTO> SaveCompany([FromBody] CompanyDTO companyDTO)
        {
            ReturnInfo<CompanyDTO> returnInfo = new ReturnInfo<CompanyDTO>();

            try
            {
                returnInfo.Data = new List<CompanyDTO> { _companyService.SaveCompany(companyDTO, _userInfo) };
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.SAVED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("CompanyController.SaveCompany", ex.ToString(), _userInfo.UserId);
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
               var _companyListForCombo = _companyService.GetListForCombo(filter.Search, filter.Id);
                returnInfo.IsSuccess = true;
                returnInfo.Data = _companyListForCombo;
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("CompanyController.GetListForCombo", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;
        }


        [HttpPost("DeleteCompany")]
        [SessionFilterAtrribute]
        public ReturnInfo<CompanyDTO> DeleteCompany([FromBody] CompanyDTO companyDTO)
        {
            ReturnInfo<CompanyDTO> returnInfo = new ReturnInfo<CompanyDTO>();

            try
            {
                _companyService.DeleteCompany(companyDTO);
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.DELETED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("CompanyController.DeleteCompany", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }

        public class CompanyListQueryParams
        {
            public CompanyFilter Filter { get; set; }
            public QueryInfo QueryInfo { get; set; }
            public bool IsExport { get; set; }
            public List<ColumnInfo> ColumnInfos {get;set;}
         }
    }
}
