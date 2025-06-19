using System;
using System.Collections.Generic;

#nullable disable

namespace NanoGo.Models
{
    public partial class PermissionLog
    {
        public int PermissionLogId { get; set; }
        public int EmployeeId { get; set; }
        public int Status { get; set; }
        public int PermissionId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }

        public virtual User CreatedUserNavigation { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Permission Permission { get; set; }
        public virtual User UpdatedUserNavigation { get; set; }
    }
}
