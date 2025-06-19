using System;
using System.Collections.Generic;

#nullable disable

namespace NanoGo.Models
{
    public partial class Inventory
    {
        public Inventory()
        {
            InventoryTransactions = new HashSet<InventoryTransaction>();
        }

        public int InventoryId { get; set; }
        public string InventoryName { get; set; }
        public string SerieNo { get; set; }
        public string Model { get; set; }
        public int InventoryType { get; set; }
        public bool IsActive { get; set; }
        public string Picture { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public int CompanyId { get; set; }
        public DateTime InventoryDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }

        public virtual Company Company { get; set; }
        public virtual User CreatedUserNavigation { get; set; }
        public virtual Parameter InventoryTypeNavigation { get; set; }
        public virtual User UpdatedUserNavigation { get; set; }
        public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; }
    }
}
