using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NanoGo.Shared
{
    public class SessionHelper
    {
        private HttpContext _httpContext;
        private static Dictionary<String, UserInfo> sessionUsers;
        public SessionHelper(HttpContext httpContext)
        {
            _httpContext = httpContext;
            if (sessionUsers == null)
            {
                sessionUsers = new Dictionary<string, UserInfo>();
            }
        }
        public UserInfo GetCurrentUser()
        {
            if (sessionUsers != null)
            {
                StringValues token;

                if (_httpContext.Request.Query.TryGetValue("session", out token))
                {
                    if (!string.IsNullOrEmpty(token))
                    {
                        if (sessionUsers.ContainsKey(token))
                        {
                            return sessionUsers[token];
                        }

                    }
                }

            }

            return null;
        }

        public UserInfo AddUserSession(UserInfo userInfo)
        {
            string key = Guid.NewGuid().ToString();
            if (sessionUsers.Any(x => x.Value.UserId == userInfo.UserId))
            {
                key = sessionUsers.First(x => x.Value.UserId == userInfo.UserId).Key;
                userInfo.SessionId = key;
                sessionUsers[key] = userInfo;
            }
            else
            {
                userInfo.SessionId = key;
                sessionUsers.Add(key, userInfo);
            }

            return userInfo;

        }

        public void RemoveUserSession(string sessionKey)
        {
            sessionUsers.Remove(sessionKey);
        }

        public UserInfo GetUserBySession(string sessionKey)
        {
            if (sessionUsers.Any(x => x.Key == sessionKey))
            {
                return sessionUsers.First(x => x.Key == sessionKey).Value;
            }
            else
                return null;
        }
    }
}
