using System;
using System.Collections.Generic;

#nullable disable

namespace NanoGo.Models
{
    public partial class UserRole
    {
        public int UserRoleId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }

        public virtual User CreatedUserNavigation { get; set; }
        public virtual Role Role { get; set; }
        public virtual User UpdatedUserNavigation { get; set; }
        public virtual User User { get; set; }
    }
}
