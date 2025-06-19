using System;
using System.Collections.Generic;

#nullable disable

namespace NanoGo.Models
{
    public partial class InventoryTransaction
    {
        public int InventoryTransId { get; set; }
        public int InventoryId { get; set; }
        public DateTime TransDate { get; set; }
        public int? EmployeeId { get; set; }
        public int TransType { get; set; }
        public string Note { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }

        public virtual User CreatedUserNavigation { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Inventory Inventory { get; set; }
        public virtual User UpdatedUserNavigation { get; set; }
    }
}
