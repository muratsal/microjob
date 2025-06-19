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
    public class MenuController : ControllerBase
    {
        private readonly MenuService _menuService;
        private readonly UserInfo _userInfo;
        private readonly SessionHelper _sessionHelper;
        private readonly SystemLogService _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MenuController(IHttpContextAccessor httpContextAccessor)
        {
            _menuService = new MenuService();
            _sessionHelper = new SessionHelper(httpContextAccessor.HttpContext);
            _userInfo = _sessionHelper.GetCurrentUser();
            _logger = new SystemLogService();
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("GetMenuList")]
        [SessionFilterAtrribute]
        public ReturnInfo<MenuDTO> GetMenuList([FromBody] MenuListQueryParams  menuListQueryParams)
        {
             ReturnInfo<MenuDTO> returnInfo = new ReturnInfo<MenuDTO>();
            try
            {
                var resultData = _menuService.GetMenuList(menuListQueryParams.Filter, menuListQueryParams.QueryInfo,menuListQueryParams.IsExport);
               
                if (!menuListQueryParams.IsExport)
                {
                    returnInfo.Data = resultData.Data;
                }
                else
                {
                    returnInfo.Key = ExcelHelper.AddListAsExcelToCache<MenuDTO>(resultData.Data, menuListQueryParams.ColumnInfos);
                }

                returnInfo.TotalCount = resultData.TotalCount;
                returnInfo.IsSuccess = true;

            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("MenuController.GetMenuList", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;
        }

        [HttpPost("SaveMenu")]
        [SessionFilterAtrribute]
        public ReturnInfo<MenuDTO> SaveMenu([FromBody] MenuDTO menuDTO)
        {
            ReturnInfo<MenuDTO> returnInfo = new ReturnInfo<MenuDTO>();

            try
            {
                returnInfo.Data = new List<MenuDTO> { _menuService.SaveMenu(menuDTO, _userInfo) };
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.SAVED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("MenuController.SaveMenu", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }


        [HttpPost("DeleteMenu")]
        [SessionFilterAtrribute]
        public ReturnInfo<MenuDTO> DeleteMenu([FromBody] MenuDTO menuDTO)
        {
            ReturnInfo<MenuDTO> returnInfo = new ReturnInfo<MenuDTO>();

            try
            {
                _menuService.DeleteMenu(menuDTO);
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.DELETED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("MenuController.DeleteMenu", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }

        public class MenuListQueryParams
        {
            public MenuFilter Filter { get; set; }
            public QueryInfo QueryInfo { get; set; }
            public bool IsExport { get; set; }
            public List<ColumnInfo> ColumnInfos {get;set;}
         }
    }
}
