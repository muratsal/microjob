using System;
using System.Collections.Generic;

#nullable disable

namespace NanoGo.Models
{
    public partial class Company
    {
        public Company()
        {
            Employees = new HashSet<Employee>();
            Inventories = new HashSet<Inventory>();
        }

        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Logo { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string TaxAuthority { get; set; }
        public string TaxNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }

        public virtual User CreatedUserNavigation { get; set; }
        public virtual User UpdatedUserNavigation { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<Inventory> Inventories { get; set; }
    }
}
