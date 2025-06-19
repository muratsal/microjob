using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NanoGo.Services.System;
using NanoGo.Shared;
using NanoGo.Shared.Export;
using NanoGo.Shared.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NanoGo.Controllers.System
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly ParameterService _parameterService;
        private readonly UserRoleService _userRoleService;
        private readonly UserInfo _userInfo;
        private readonly SessionHelper _sessionHelper;
        private readonly SystemLogService _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _hostEnvironment;
        public UserController(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostEnvironment)
        {
            _userService = new UserService();
            _parameterService = new ParameterService();
            _userRoleService = new UserRoleService();
            _sessionHelper = new SessionHelper(httpContextAccessor.HttpContext);
            _userInfo = _sessionHelper.GetCurrentUser();
            _logger = new SystemLogService();
            _httpContextAccessor = httpContextAccessor;
            _hostEnvironment = hostEnvironment;
        }




        [HttpPost("GetUserList")]
        [SessionFilterAtrribute]
        public ReturnInfo<UserDTO> GetUserList([FromBody] UserListQueryParams userListQueryParams)
        {
            ReturnInfo<UserDTO> returnInfo = new ReturnInfo<UserDTO>();

            try
            {
                var resultData = _userService.GetUserList(userListQueryParams.Filter, userListQueryParams.QueryInfo, userListQueryParams.IsExport);
                if (!userListQueryParams.IsExport)
                {
                    returnInfo.Data = resultData.Data;
                }
                else
                {
                    returnInfo.Key = ExcelHelper.AddListAsExcelToCache<UserDTO>(resultData.Data, userListQueryParams.ColumnInfos);
                }
                returnInfo.TotalCount = resultData.TotalCount;
                returnInfo.IsSuccess = true;
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("UserController.GetUserList", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;
        }



        [HttpPost("SaveUser")]
        [SessionFilterAtrribute]
        public ReturnInfo<UserDTO> SaveUser([FromBody] UserDTO userDTO)
        {
            ReturnInfo<UserDTO> returnInfo = new ReturnInfo<UserDTO>();

            try
            {
                returnInfo.Data = new List<UserDTO> { _userService.SaveUser(userDTO, _userInfo) };
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.SAVED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("UserController.SaveUser", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }

        [HttpGet("SetUserLanguage")]
        [SessionFilterAtrribute]
        public ReturnInfo<UserDTO> SetUserLanguage(string languageCode)
        {
            ReturnInfo<UserDTO> returnInfo = new ReturnInfo<UserDTO>();

            try
            {
                var user =  _userService.GetUserList(new UserFilter { UserId = _userInfo.UserId }, null, false).Data.First();
                var parameter =  _parameterService.GetParameterList(new ParameterFilter { ParamType = 13 ,FreeText1 = languageCode }, null, false).Data.First();
                user.Language = parameter.ParamId;
                _userService.SaveUser(user,_userInfo);

                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.SAVED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("UserController.SetUserLanguage", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }


        [HttpPost("SaveUserWithRoles")]
        [SessionFilterAtrribute]
        public ReturnInfo<UserDTO> SaveUserWithRoles([FromBody] UserWithRoleDTO userWithRoleDTO)
        {
            ReturnInfo<UserDTO> returnInfo = new ReturnInfo<UserDTO>();

            try
            {

                var userList = new List<UserDTO> { _userService.SaveUser(userWithRoleDTO.User, _userInfo) };

                var userRoles = _userRoleService.GetUserRoleList(new UserRoleFilter { UserId = userList[0].UserId }, null, false);

                userRoles.Data.ForEach(x =>
                {
                    _userRoleService.DeleteUserRole(x);
                });

                userWithRoleDTO.RoleInfos.ForEach(x =>
                {
                    if (x.IsUserRole)
                        _userRoleService.SaveUserRole(new UserRoleDTO { RoleId = x.RoleId, UserId = userList[0].UserId }, _userInfo);
                });

                returnInfo.Data = userList;
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.SAVED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("UserController.SaveUser", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }


        [HttpPost("DeleteUser")]
        [SessionFilterAtrribute]
        public ReturnInfo<UserDTO> DeleteUser(UserDTO userDTO)
        {
            ReturnInfo<UserDTO> returnInfo = new ReturnInfo<UserDTO>();

            try
            {
                _userService.DeleteUser(userDTO);
                returnInfo.IsSuccess = true;
                returnInfo.Message = "GENERAL.DELETED";
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("UserController.DeleteUser", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }



        [HttpPost("GetRoleInfo")]
        [SessionFilterAtrribute]
        public ReturnInfo<UserRoleInfo> GetRoleInfo([FromBody] UserDTO userDTO)
        {
            ReturnInfo<UserRoleInfo> returnInfo = new ReturnInfo<UserRoleInfo>();

            try
            {
                var userRoleInfo = _userService.GetRoleInfo(userDTO.UserId);
                returnInfo.IsSuccess = true;
                returnInfo.Data = userRoleInfo;
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("UserController.GetRoleInfo", ex.ToString(), _userInfo.UserId);
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
                var userListForCombo = _userService.GetListForCombo(filter.Search, filter.Id);
                returnInfo.IsSuccess = true;
                returnInfo.Data = userListForCombo;
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("UserController.GetListForCombo", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;
        }


        [HttpPost("UploadProfileImage")]
        [SessionFilterAtrribute]
        public ReturnInfo UploadProfileImage()
        {
            ReturnInfo returnInfo = new ReturnInfo();

            try
            {
                var files = _httpContextAccessor.HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    var file = files.First();
                    var uploadPath = Path.Combine(_hostEnvironment.WebRootPath, "AppImages\\User");
                    var uploadPathThumbnail = Path.Combine(_hostEnvironment.WebRootPath, "AppImages\\User\\Thumbnail");

                    if (file.Length > 0)
                    {

                        string fileName = Guid.NewGuid().ToString() + ".jpg";

                        Image image = Image.FromStream(file.OpenReadStream(), true, true);
                        var thumbnail = Utilities.ResizeImage(180, 180, image);
                        image = Image.FromStream(file.OpenReadStream(), true, true);
                        var resizedProductImage = Utilities.ResizeImage(800, 800, image);

                        thumbnail.Save(Path.Combine(uploadPathThumbnail, fileName), ImageFormat.Jpeg);
                        resizedProductImage.Save(Path.Combine(uploadPath, fileName), ImageFormat.Jpeg);

                        returnInfo.Data = Path.Combine("AppImages\\User", fileName);

                        using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
                        {
                            var user =  dbContext.Users.FirstOrDefault(x => x.UserId == _userInfo.UserId);
                            user.ProfileImage = returnInfo.Data.ToString();
                            dbContext.SaveChanges();
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
                _logger.AddLog("ProductController.UploadProductImage", ex.ToString(), _userInfo.UserId);
            }

            return returnInfo;

        }



        public class UserListQueryParams
        {
            public UserFilter Filter { get; set; }
            public QueryInfo QueryInfo { get; set; }
            public bool IsExport { get; set; }
            public List<ColumnInfo> ColumnInfos { get; set; }
        }

        public class UserWithRoleDTO
        {
            public UserDTO User { get; set; }
            public List<UserRoleInfo> RoleInfos { get; set; }
        }



    }

}
