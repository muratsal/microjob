using System;
using System.Collections.Generic;

#nullable disable

namespace NanoGo.Models
{
    public partial class EmailAttachment
    {
        public int EmailAttachmentId { get; set; }
        public int? EmailId { get; set; }
        public string Name { get; set; }
        public string FilePath { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }

        public virtual User CreatedUserNavigation { get; set; }
        public virtual Email Email { get; set; }
        public virtual User UpdatedUserNavigation { get; set; }
    }
}
