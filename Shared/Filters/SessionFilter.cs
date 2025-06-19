using NanoGo.Services.System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NanoGo.Shared.Filters
{
    public class SessionFilterAtrribute : Attribute, IActionFilter
    {

        private SessionHelper _sessionHelper;
        public SessionFilterAtrribute()
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

             var sessionKey = context.HttpContext.Request.Query["session"].FirstOrDefault();
            _sessionHelper = new SessionHelper(context.HttpContext);
            if (string.IsNullOrEmpty(sessionKey) || _sessionHelper.GetCurrentUser() == null)
            {
                ReturnInfo returnInfo = new ReturnInfo();
                returnInfo.IsSuccess = false;
                returnInfo.IsLoginRequired = true;
                returnInfo.ErrorMessage = "MESSAGES.LOGIN_REQUIRED";
                context.Result = new JsonResult(returnInfo);
                return;
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            return;
        }

        
    }
}
