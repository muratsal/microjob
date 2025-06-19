using System;
using System.Collections.Generic;

#nullable disable

namespace NanoGo.Models
{
    public partial class RoleAuth
    {
        public int RoleAuthId { get; set; }
        public int? RoleId { get; set; }
        public int? AuthId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }

        public virtual Auth Auth { get; set; }
        public virtual User CreatedUserNavigation { get; set; }
        public virtual Role Role { get; set; }
        public virtual User UpdatedUserNavigation { get; set; }
    }
}
