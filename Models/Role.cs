using System;
using System.Collections.Generic;

#nullable disable

namespace NanoGo.Models
{
    public partial class Role
    {
        public Role()
        {
            RoleAuths = new HashSet<RoleAuth>();
            UserRoles = new HashSet<UserRole>();
        }

        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleDesc { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }

        public virtual User CreatedUserNavigation { get; set; }
        public virtual User UpdatedUserNavigation { get; set; }
        public virtual ICollection<RoleAuth> RoleAuths { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
