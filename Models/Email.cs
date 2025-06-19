using System;
using System.Collections.Generic;

#nullable disable

namespace NanoGo.Models
{
    public partial class Email
    {
        public Email()
        {
            EmailAttachments = new HashSet<EmailAttachment>();
        }

        public int EmailId { get; set; }
        public string EmailFrom { get; set; }
        public string EmailTo { get; set; }
        public string EmailToCc { get; set; }
        public string EmailToBcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool? IsHtml { get; set; }
        public bool? IsSend { get; set; }
        public bool? IsSuccess { get; set; }
        public DateTime? SendDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }

        public virtual User CreatedUserNavigation { get; set; }
        public virtual User UpdatedUserNavigation { get; set; }
        public virtual ICollection<EmailAttachment> EmailAttachments { get; set; }
    }
}
