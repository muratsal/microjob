using System;
using System.Collections.Generic;

#nullable disable

namespace NanoGo.Models
{
    public partial class Auth
    {
        public Auth()
        {
            RoleAuths = new HashSet<RoleAuth>();
        }

        public int AuthId { get; set; }
        public string AuthCode { get; set; }
        public string AuthDesc { get; set; }
        public int AuthType { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }

        public virtual User CreatedUserNavigation { get; set; }
        public virtual User UpdatedUserNavigation { get; set; }
        public virtual ICollection<RoleAuth> RoleAuths { get; set; }
    }
}
