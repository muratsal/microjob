using System;
using System.Collections.Generic;

#nullable disable

namespace NanoGo.Models
{
    public partial class EmailConfig
    {
        public int EmailConfigId { get; set; }
        public string ConfigName { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public bool? EnableSsl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }

        public virtual User CreatedUserNavigation { get; set; }
        public virtual User UpdatedUserNavigation { get; set; }
    }
}
