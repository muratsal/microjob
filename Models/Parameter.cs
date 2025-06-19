using System;
using System.Collections.Generic;

#nullable disable

namespace NanoGo.Models
{
    public partial class Parameter
    {
        public Parameter()
        {
            EmployeeDepartments = new HashSet<Employee>();
            EmployeeTitles = new HashSet<Employee>();
            Inventories = new HashSet<Inventory>();
            Permissions = new HashSet<Permission>();
        }

        public int ParamId { get; set; }
        public int ParamType { get; set; }
        public string ParamCode { get; set; }
        public string ParamDesc { get; set; }
        public int? ParentId { get; set; }
        public string FreeText1 { get; set; }
        public string FreeText2 { get; set; }
        public string FreeText3 { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }

        public virtual User CreatedUserNavigation { get; set; }
        public virtual User UpdatedUserNavigation { get; set; }
        public virtual ICollection<Employee> EmployeeDepartments { get; set; }
        public virtual ICollection<Employee> EmployeeTitles { get; set; }
        public virtual ICollection<Inventory> Inventories { get; set; }
        public virtual ICollection<Permission> Permissions { get; set; }
    }
}
