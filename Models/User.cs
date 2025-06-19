using System;
using System.Collections.Generic;

#nullable disable

namespace NanoGo.Models
{
    public partial class User
    {
        public User()
        {
            AuthCreatedUserNavigations = new HashSet<Auth>();
            AuthUpdatedUserNavigations = new HashSet<Auth>();
            CompanyCreatedUserNavigations = new HashSet<Company>();
            CompanyUpdatedUserNavigations = new HashSet<Company>();
            CounterCreatedUserNavigations = new HashSet<Counter>();
            CounterUpdatedUserNavigations = new HashSet<Counter>();
            EmailAttachmentCreatedUserNavigations = new HashSet<EmailAttachment>();
            EmailAttachmentUpdatedUserNavigations = new HashSet<EmailAttachment>();
            EmailConfigCreatedUserNavigations = new HashSet<EmailConfig>();
            EmailConfigUpdatedUserNavigations = new HashSet<EmailConfig>();
            EmailCreatedUserNavigations = new HashSet<Email>();
            EmailUpdatedUserNavigations = new HashSet<Email>();
            EmployeeCreatedUserNavigations = new HashSet<Employee>();
            EmployeeUpdatedUserNavigations = new HashSet<Employee>();
            EmployeeUsers = new HashSet<Employee>();
            FileCreatedUserNavigations = new HashSet<File>();
            FileUpdatedUserNavigations = new HashSet<File>();
            InventoryCreatedUserNavigations = new HashSet<Inventory>();
            InventoryTransactionCreatedUserNavigations = new HashSet<InventoryTransaction>();
            InventoryTransactionUpdatedUserNavigations = new HashSet<InventoryTransaction>();
            InventoryUpdatedUserNavigations = new HashSet<Inventory>();
            LogCreatedUserNavigations = new HashSet<Log>();
            LogUpdatedUserNavigations = new HashSet<Log>();
            MenuCreatedUserNavigations = new HashSet<Menu>();
            MenuUpdatedUserNavigations = new HashSet<Menu>();
            ParameterCreatedUserNavigations = new HashSet<Parameter>();
            ParameterUpdatedUserNavigations = new HashSet<Parameter>();
            PermissionCreatedUserNavigations = new HashSet<Permission>();
            PermissionLogCreatedUserNavigations = new HashSet<PermissionLog>();
            PermissionLogUpdatedUserNavigations = new HashSet<PermissionLog>();
            PermissionUpdatedUserNavigations = new HashSet<Permission>();
            RoleAuthCreatedUserNavigations = new HashSet<RoleAuth>();
            RoleAuthUpdatedUserNavigations = new HashSet<RoleAuth>();
            RoleCreatedUserNavigations = new HashSet<Role>();
            RoleUpdatedUserNavigations = new HashSet<Role>();
            UserRoleCreatedUserNavigations = new HashSet<UserRole>();
            UserRoleUpdatedUserNavigations = new HashSet<UserRole>();
            UserRoleUsers = new HashSet<UserRole>();
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string LastLoginIp { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }
        public string ProfileImage { get; set; }
        public int UserType { get; set; }
        public int? CustomerId { get; set; }
        public bool AccesFromOutside { get; set; }
        public int? Language { get; set; }

        public virtual ICollection<Auth> AuthCreatedUserNavigations { get; set; }
        public virtual ICollection<Auth> AuthUpdatedUserNavigations { get; set; }
        public virtual ICollection<Company> CompanyCreatedUserNavigations { get; set; }
        public virtual ICollection<Company> CompanyUpdatedUserNavigations { get; set; }
        public virtual ICollection<Counter> CounterCreatedUserNavigations { get; set; }
        public virtual ICollection<Counter> CounterUpdatedUserNavigations { get; set; }
        public virtual ICollection<EmailAttachment> EmailAttachmentCreatedUserNavigations { get; set; }
        public virtual ICollection<EmailAttachment> EmailAttachmentUpdatedUserNavigations { get; set; }
        public virtual ICollection<EmailConfig> EmailConfigCreatedUserNavigations { get; set; }
        public virtual ICollection<EmailConfig> EmailConfigUpdatedUserNavigations { get; set; }
        public virtual ICollection<Email> EmailCreatedUserNavigations { get; set; }
        public virtual ICollection<Email> EmailUpdatedUserNavigations { get; set; }
        public virtual ICollection<Employee> EmployeeCreatedUserNavigations { get; set; }
        public virtual ICollection<Employee> EmployeeUpdatedUserNavigations { get; set; }
        public virtual ICollection<Employee> EmployeeUsers { get; set; }
        public virtual ICollection<File> FileCreatedUserNavigations { get; set; }
        public virtual ICollection<File> FileUpdatedUserNavigations { get; set; }
        public virtual ICollection<Inventory> InventoryCreatedUserNavigations { get; set; }
        public virtual ICollection<InventoryTransaction> InventoryTransactionCreatedUserNavigations { get; set; }
        public virtual ICollection<InventoryTransaction> InventoryTransactionUpdatedUserNavigations { get; set; }
        public virtual ICollection<Inventory> InventoryUpdatedUserNavigations { get; set; }
        public virtual ICollection<Log> LogCreatedUserNavigations { get; set; }
        public virtual ICollection<Log> LogUpdatedUserNavigations { get; set; }
        public virtual ICollection<Menu> MenuCreatedUserNavigations { get; set; }
        public virtual ICollection<Menu> MenuUpdatedUserNavigations { get; set; }
        public virtual ICollection<Parameter> ParameterCreatedUserNavigations { get; set; }
        public virtual ICollection<Parameter> ParameterUpdatedUserNavigations { get; set; }
        public virtual ICollection<Permission> PermissionCreatedUserNavigations { get; set; }
        public virtual ICollection<PermissionLog> PermissionLogCreatedUserNavigations { get; set; }
        public virtual ICollection<PermissionLog> PermissionLogUpdatedUserNavigations { get; set; }
        public virtual ICollection<Permission> PermissionUpdatedUserNavigations { get; set; }
        public virtual ICollection<RoleAuth> RoleAuthCreatedUserNavigations { get; set; }
        public virtual ICollection<RoleAuth> RoleAuthUpdatedUserNavigations { get; set; }
        public virtual ICollection<Role> RoleCreatedUserNavigations { get; set; }
        public virtual ICollection<Role> RoleUpdatedUserNavigations { get; set; }
        public virtual ICollection<UserRole> UserRoleCreatedUserNavigations { get; set; }
        public virtual ICollection<UserRole> UserRoleUpdatedUserNavigations { get; set; }
        public virtual ICollection<UserRole> UserRoleUsers { get; set; }
    }
}
