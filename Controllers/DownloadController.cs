using NanoGo.Services.System;
using NanoGo.Shared;
using NanoGo.Shared.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NanoGo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly UserInfo _userInfo;
        private readonly SessionHelper _sessionHelper;
        private readonly SystemLogService _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DownloadController(IHttpContextAccessor httpContextAccessor)
        {
            _sessionHelper = new SessionHelper(httpContextAccessor.HttpContext);
            _userInfo = _sessionHelper.GetCurrentUser();
            _logger = new SystemLogService();
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("FromCacheByKey")]
        public FileResult FromCacheByKey()
        {
            string key = _httpContextAccessor.HttpContext.Request.Query["key"].ToString();
            var cacheItem = DownloadCache.GetCacheItemByKey(key);

            byte[] fileByteArray = (byte[])cacheItem.Value;
            //MemoryStream stream = new MemoryStream();
            //stream.Write(fileByteArray, 0, fileByteArray.Length);
            string contentType = "";
            string fileName = "";
            if (cacheItem.DocType == DocumentType.EXCEL)
            {
                contentType = "application/octet-stream";
                fileName = "Export.xlsx";
            }
            return File(fileByteArray, contentType, fileName);
        }
    }
}
