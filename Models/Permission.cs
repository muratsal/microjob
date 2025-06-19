using System;
using System.Collections.Generic;

#nullable disable

namespace NanoGo.Models
{
    public partial class Permission
    {
        public Permission()
        {
            PermissionLogs = new HashSet<PermissionLog>();
        }

        public int PermissionId { get; set; }
        public int EmployeeId { get; set; }
        public int PermissionType { get; set; }
        public DateTime StartDate { get; set; }
        public int DayCount { get; set; }
        public DateTime EndDate { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public int? ProxyEmployee { get; set; }
        public string Duties { get; set; }
        public string Note { get; set; }
        public int? ApprovedWaitingEmployee { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }

        public virtual Employee ApprovedWaitingEmployeeNavigation { get; set; }
        public virtual User CreatedUserNavigation { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Parameter PermissionTypeNavigation { get; set; }
        public virtual Employee ProxyEmployeeNavigation { get; set; }
        public virtual User UpdatedUserNavigation { get; set; }
        public virtual ICollection<PermissionLog> PermissionLogs { get; set; }
    }
}
