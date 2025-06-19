using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NanoGo.Shared
{
    public class UserInfo
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string SessionId { get; set; }
        public int? EmployeeId { get; set; }
        public string ProfileImage { get; set; }
        public int? CustomerId { get; set; }
        public int UserType { get; set; }
        public int? Language { get; set; }
        public string LanguageCode { get; set; }
        public int? ParentSectorId { get; set; } 
        public List<AuthInfo> AuthInfos {get; set;}
    }

    public class AuthInfo
    {
        public int AuthId { get; set; }
        public int AuthType { get; set; }
        public string AuthCode { get; set; }
        public string AuthDesc { get; set; }
    }
}
