using System;
using System.Collections.Generic;

#nullable disable

namespace NanoGo.Models
{
    public partial class Counter
    {
        public int CounterId { get; set; }
        public string CounterName { get; set; }
        public int CurrentValue { get; set; }
        public string Prefix { get; set; }
        public bool AddYear { get; set; }
        public int PaddingCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }

        public virtual User CreatedUserNavigation { get; set; }
        public virtual User UpdatedUserNavigation { get; set; }
    }
}
