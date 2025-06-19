using System;
using System.Collections.Generic;

#nullable disable

namespace NanoGo.Models
{
    public partial class Employee
    {
        public Employee()
        {
            InventoryTransactions = new HashSet<InventoryTransaction>();
            InverseResponsibleForNavigation = new HashSet<Employee>();
            PermissionApprovedWaitingEmployeeNavigations = new HashSet<Permission>();
            PermissionEmployees = new HashSet<Permission>();
            PermissionLogs = new HashSet<PermissionLog>();
            PermissionProxyEmployeeNavigations = new HashSet<Permission>();
        }

        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public DateTime WorkStartDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public int TitleId { get; set; }
        public int DepartmentId { get; set; }
        public int? UserId { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string GsmNumber { get; set; }
        public string Image { get; set; }
        public bool IsWorking { get; set; }
        public DateTime? EndWorkDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }
        public string IntegrationCode { get; set; }
        public int CompanyId { get; set; }
        public int? ResponsibleFor { get; set; }

        public virtual Company Company { get; set; }
        public virtual User CreatedUserNavigation { get; set; }
        public virtual Parameter Department { get; set; }
        public virtual Employee ResponsibleForNavigation { get; set; }
        public virtual Parameter Title { get; set; }
        public virtual User UpdatedUserNavigation { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; }
        public virtual ICollection<Employee> InverseResponsibleForNavigation { get; set; }
        public virtual ICollection<Permission> PermissionApprovedWaitingEmployeeNavigations { get; set; }
        public virtual ICollection<Permission> PermissionEmployees { get; set; }
        public virtual ICollection<PermissionLog> PermissionLogs { get; set; }
        public virtual ICollection<Permission> PermissionProxyEmployeeNavigations { get; set; }
    }
}
