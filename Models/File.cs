using System;
using System.Collections.Generic;

#nullable disable

namespace NanoGo.Models
{
    public partial class File
    {
        public int FileId { get; set; }
        public string Description { get; set; }
        public string FileOriginalName { get; set; }
        public int TableNo { get; set; }
        public int TableReferenceId { get; set; }
        public string FilePath { get; set; }
        public double FileSize { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }
        public bool? DisplayByCustomer { get; set; }

        public virtual User CreatedUserNavigation { get; set; }
        public virtual User UpdatedUserNavigation { get; set; }
    }
}
