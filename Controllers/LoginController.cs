using NanoGo.Models;
using NanoGo.Services.System;
using NanoGo.Shared;
using NanoGo.Shared.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;

namespace NanoGo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {

        private readonly SessionHelper _sessionHelper;
        private readonly UserInfo _userInfo;
        private readonly SystemLogService _logger;
        private readonly UserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LoginController(IHttpContextAccessor httpContextAccessor)
        {
            _sessionHelper = new SessionHelper(httpContextAccessor.HttpContext);
            _logger = new SystemLogService();
            _userService = new UserService();
            _userInfo = _sessionHelper.GetCurrentUser();
            _httpContextAccessor = httpContextAccessor;
        }
        public string Index()
        {
            return "";
        }

        [HttpPost("Authenticate")]
        public JsonResult Authenticate([FromBody] UserLoginViewModel userLogin)
        {
            ReturnInfo returnInfo = new ReturnInfo();
            returnInfo.IsSuccess = false;
            UserInfo ui = null;
            try
            {

                using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
                {
                    var user = dbContext.Users.FirstOrDefault(x => x.UserName == userLogin.UserName && x.IsActive == true);

                    string test = _httpContextAccessor.HttpContext.Request.Host.Host;

                    if (user == null)
                    {
                        returnInfo.IsSuccess = false;
                        returnInfo.ErrorMessage = "LOGIN.NOTFOUND";
                    }
                    else if (test.Contains("erp.asrra") && !user.AccesFromOutside)
                    {
                        returnInfo.IsSuccess = false;
                        returnInfo.ErrorMessage = "LOGIN.ACCESSDENIED";
                    }
                    else
                    {
                        if (user.Password == userLogin.Password)
                        {
                            returnInfo.IsSuccess = true;
                            returnInfo.Message = "LOGIN.AUTHENTICATED";
                            ui = _userService.GetAuthenticatedUserInfo(user);
                            ui = _sessionHelper.AddUserSession(ui);
                            returnInfo.Data = ui;
                        }
                        else
                        {
                            returnInfo.IsSuccess = false;
                            returnInfo.ErrorMessage = "LOGIN.NOTFOUND";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("LoginController.Authenticate", ex.ToString(), 1);
            }


            return new JsonResult(returnInfo);
        }
        [HttpGet("GetAuthenticatedUser")]
        public JsonResult GetAuthenticatedUser(string session)
        {
            ReturnInfo returnInfo = new ReturnInfo();
            returnInfo.IsSuccess = false;
            returnInfo.IsLoginRequired = true;
            if (string.IsNullOrEmpty(session))
            {
                return new JsonResult(returnInfo);
            }
            else
            {
                var sessionUser = _sessionHelper.GetUserBySession(session);
                if (sessionUser == null)
                {
                    return new JsonResult(returnInfo);
                }
                else
                {
                    returnInfo.IsSuccess = true;
                    returnInfo.IsLoginRequired = false;
                    returnInfo.Data = sessionUser;
                }

            }

            return new JsonResult(returnInfo);
        }


        [HttpPost("GetUserMenuInfo")]
        [SessionFilterAtrribute]
        public JsonResult GetUserMenuInfo()
        {
            ReturnInfo returnInfo = new ReturnInfo();
            returnInfo.IsSuccess = false;
            returnInfo.IsLoginRequired = true;
            var menuItemsDTO = new List<MenuItemDTO>();
            try
            {

                using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
                {
                    var userRoles = dbContext.UserRoles.Where(x => x.UserId == _userInfo.UserId).Select(x => x.RoleId).ToList();
                    var roleAuths = dbContext.RoleAuths
                        .Where(x => userRoles.Contains(x.RoleId.Value))
                        .Select(x => x.AuthId)
                        .ToList()
                        .Distinct()
                        .ToList();
                    var userAuths = dbContext.Auths.Where(x => roleAuths.Contains(x.AuthId) && x.AuthType == 1).ToList();
                    var menus = dbContext.Menus.ToList();


                    DateTime now = DateTime.Now;

                    userAuths.ForEach((x) =>
                    {
                        string menuName = x.AuthCode.Split(".")[0];
                        string subMenuName = x.AuthCode.Split(".")[1];

                        if (menuItemsDTO.Any(y => y.MenuName == menuName))
                        {
                            var menuItem = menuItemsDTO.FirstOrDefault(y => y.MenuName == menuName);
                            menuItem.SubMenuItems.Add(new SubMenuItemDTO
                            {
                                MenuName = subMenuName,
                                TranslateName = $"{subMenuName}.{subMenuName}MENU",
                                Status = subMenuName.ToLowerInvariant()
                            });
                        }
                        else
                        {
                            var menu = menus.First(x => x.Name == menuName);
                            menuItemsDTO.Add(new MenuItemDTO
                            {
                                MenuIcon = menu.MenuIcon,
                                MenuName = menu.Name,
                                TranslateName = $"MAINMENU.{menu.Name}",
                                State = menu.State,
                                BadgeValue =  0,
                                SubMenuItems = new List<SubMenuItemDTO>()
                                {
                                new SubMenuItemDTO()
                                {
                                   MenuName = subMenuName,
                                   TranslateName = $"{subMenuName}.{subMenuName}MENU",
                                   Status = subMenuName.ToLowerInvariant(),
                                   

                                }
                                }
                            });
                        }
                    });
                }

                returnInfo.IsSuccess = true;
                returnInfo.IsLoginRequired = false;
                returnInfo.Data = menuItemsDTO;
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("LoginController.GetUserMenuInfo", ex.ToString(), _userInfo.UserId);
            }

            return new JsonResult(returnInfo);
        }

        [HttpPost("UpdatePassword")]
        [SessionFilterAtrribute]
        public JsonResult UpdatePassword([FromBody] UpdatePasswordModel passwordModel, string session)
        {

            ReturnInfo returnInfo = new ReturnInfo();
            returnInfo.IsSuccess = false;
            returnInfo.IsLoginRequired = true;

            if (string.IsNullOrEmpty(session))
            {
                return new JsonResult(returnInfo);
            }
            else
            {
                var sessionUser = _sessionHelper.GetUserBySession(session);

                if (sessionUser == null)
                {
                    return new JsonResult(returnInfo);
                }
                else
                {
                    using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
                    {
                        var user = dbContext.Users.FirstOrDefault(x => x.UserName == sessionUser.UserName && x.Password == passwordModel.OldPassword);
                        if (user == null)
                        {
                            returnInfo.IsSuccess = false;
                            returnInfo.ErrorMessage = "LOGIN.USERNAMEORUSERPASSWORDNOTVALID";
                        }
                        else
                        {
                            if (passwordModel.NewPassword.Trim().Length > 0)
                            {
                                user.Password = passwordModel.NewPassword.Trim();
                                dbContext.SaveChanges();
                                returnInfo.IsSuccess = true;
                                returnInfo.Message = "LOGIN.PASSWORDUPDATED";
                                returnInfo.Data = "";
                            }
                            else
                            {
                                returnInfo.IsSuccess = false;
                                returnInfo.ErrorMessage = "LOGIN.PASSWORDNOTVALID";
                            }
                        }
                    }
                }

            }

            return new JsonResult(returnInfo);
        }

        public class UpdatePasswordModel
        {
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }

        //private JsonResult UploadCountryDataset()
        //{
        //    ReturnInfo returnInfo = new ReturnInfo();
        //    returnInfo.IsSuccess = false;
        //    returnInfo.IsLoginRequired = false;

        //    using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
        //    {
        //        string jsonData = global::System.IO.File.ReadAllText(@"C:\Users\Murat\Desktop\countryData.json").ToString();
        //        //var serialzedData=  JsonSerializer.Deserialize<Rootobject>(jsonData);
        //        var serialzedData = JsonConvert.DeserializeObject<List<CountryData>>(jsonData);


        //        serialzedData.ForEach(x =>
        //        {
        //            Models.Country country = new Country
        //            {
        //                Capital = x.capital,
        //                CreatedDate = DateTime.Now,
        //                Currency = x.currency,
        //                CurrencySymbol = x.currency_symbol,
        //                CreatedUser = 1,
        //                Emoji = x.emoji,
        //                EmojiU = x.emojiU,
        //                Iso2 = x.iso2,
        //                Iso3 = x.iso3,
        //                Lattitude = x.latitude,
        //                Longitude = x.longitude,
        //                Name = x.name,
        //                Native = x.native,
        //                NumericCode = x.numeric_code,
        //                PhoneCode = x.phone_code,
        //                Region = x.region,
        //                SubRegion = x.subregion,
        //                Tld = x.tld,
        //                UpdatedDate = DateTime.Now,
        //                UpdatedUser = 1
        //            };

        //            dbContext.Countries.Add(country);
        //            dbContext.SaveChanges();

        //            x.states.ForEach(y =>
        //            {
        //                Models.State state = new Models.State()
        //                {
        //                    CountryId = country.CountryId,
        //                    CreatedDate = DateTime.Now,
        //                    CreatedUser = 1,
        //                    Latitude = y.latitude,
        //                    Longitude = y.longitude,
        //                    Name = y.name,
        //                    StateCode = y.state_code,
        //                    UpdatedDate = DateTime.Now,
        //                    UpdatedUser = 1,
        //                };
        //                dbContext.States.Add(state);
        //                dbContext.SaveChanges();

        //                y.cities.ForEach(z =>
        //                {
        //                    Models.City city = new Models.City()
        //                    {
        //                        CreatedDate = DateTime.Now,
        //                        CreatedUser = 1,
        //                        Longitude = z.longitude,
        //                        Name = z.name,
        //                        UpdatedDate = DateTime.Now,
        //                        UpdatedUser = 1,
        //                        Lattitude = z.latitude,
        //                        StateId = state.StateId
        //                    };
        //                    dbContext.Cities.Add(city);

        //                });

        //                dbContext.SaveChanges();
        //            });


        //        });
        //        //var dbQuery = dbContext.Cities.OrderByDescending(x => x.CityId).Where(x => 1 == 1);
        //        returnInfo.Data = serialzedData.Count;



        //    }



        //    return new JsonResult(returnInfo);
        //}


        //private JsonResult FetchPortData()
        //{
        //    ReturnInfo returnInfo = new ReturnInfo();
        //    returnInfo.IsSuccess = false;
        //    returnInfo.IsLoginRequired = false;

        //    string errorCountries = "errorCountries=";

        //    using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
        //    {
        //        var countries = dbContext.Countries.ToList();



        //        for (int i = 0; i < countries.Count; i++)
        //        {
        //            try
        //            {
        //                var client = new HttpClient();

        //                HttpResponseMessage response = client.GetAsync("https://www.searates.com/maritime/ports-map/?c=" + countries[i].Iso2).Result;

        //                if (response.IsSuccessStatusCode)
        //                {
        //                    var jsonData = response.Content.ReadAsStringAsync().Result;

        //                    var s = Newtonsoft.Json.JsonConvert.DeserializeObject<ports>(jsonData);

        //                    s.cports.ForEach(x =>
        //                    {
        //                        dbContext.Ports.Add(new Port()
        //                        {
        //                            PortName = x.name,
        //                            CountryId = countries[i].CountryId,
        //                            IsActive = true,
        //                            IsContainerPort = x.t.HasValue ? x.t.Value : false,
        //                            Latitude = x.lat,
        //                            Longitude = x.lng,
        //                            CreatedDate = DateTime.Now,
        //                            CreatedUser = 1,
        //                            UpdatedDate = DateTime.Now,
        //                            UpdatedUser = 1
        //                        });

        //                    });

        //                    dbContext.SaveChanges();

        //                }

        //            }
        //            catch
        //            {
        //                errorCountries += countries[i] + ",";
        //            }
        //        }
        //    }

        //    returnInfo.Data = errorCountries;
        //    return new JsonResult(returnInfo);

        //}

    }

    public class UserLoginViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class MenuItemDTO
    {
        public string MenuName { get; set; }
        public string TranslateName { get; set; }
        public string MenuIcon { get; set; }
        public string State { get; set; }
        public int BadgeValue { get; set; }


        public List<SubMenuItemDTO> SubMenuItems { get; set; }
    }

    public class SubMenuItemDTO
    {
        public string MenuName { get; set; }
        public string TranslateName { get; set; }
        public string Status { get; set; }
       
    }

    //{
    //        name: 'SYSTEM',
    //        type: 'dropDown',
    //        tooltip: 'SYSTEM',
    //        icon: 'person',
    //        state: 'system',
    //        sub: [
    //            { name: 'USER.USERMENU', state: 'user' },
    //            { name: 'ROLE.ROLEMENU', state: 'role' },
    //            { name: 'AUTH.AUTHMENU', state: 'auth' },
    //            { name: 'ROLEAUTH.ROLEAUTHMENU', state: 'roleauth' },
    //            { name: 'USERROLE.USERROLEMENU', state: 'userrole' },
    //            { name: 'MENU.MENUMENU', state: 'menu' }
    //        ]

    //    }


}


public class Rootobject
{
    public CountryData[] data { get; set; }
}

public class CountryData
{
    public int id { get; set; }
    public string name { get; set; }
    public string iso3 { get; set; }
    public string iso2 { get; set; }
    public string numeric_code { get; set; }
    public string phone_code { get; set; }
    public string capital { get; set; }
    public string currency { get; set; }
    public string currency_symbol { get; set; }
    public string tld { get; set; }
    public string native { get; set; }
    public string region { get; set; }
    public string subregion { get; set; }
    public Timezone[] timezones { get; set; }
    public Translations translations { get; set; }
    public string latitude { get; set; }
    public string longitude { get; set; }
    public string emoji { get; set; }
    public string emojiU { get; set; }
    public List<State> states { get; set; }
}

public class Translations
{
    public string kr { get; set; }
    public string br { get; set; }
    public string pt { get; set; }
    public string nl { get; set; }
    public string hr { get; set; }
    public string fa { get; set; }
    public string de { get; set; }
    public string es { get; set; }
    public string fr { get; set; }
    public string ja { get; set; }
    public string it { get; set; }
    public string cn { get; set; }
}

public class Timezone
{
    public string zoneName { get; set; }
    public int gmtOffset { get; set; }
    public string gmtOffsetName { get; set; }
    public string abbreviation { get; set; }
    public string tzName { get; set; }
}

public class State
{
    public int id { get; set; }
    public string name { get; set; }
    public string state_code { get; set; }
    public string latitude { get; set; }
    public string longitude { get; set; }

    public List<City> cities { get; set; }
}

public class City
{
    public int id { get; set; }
    public string name { get; set; }
    public string latitude { get; set; }
    public string longitude { get; set; }
}



public class ports
{
    public string ckey { get; set; }
    public List<Cport> cports { get; set; }
}

public class Cport
{
    public string name { get; set; }
    public bool? t { get; set; }
    public string ckey { get; set; }
    public string lat { get; set; }
    public string lng { get; set; }
}



