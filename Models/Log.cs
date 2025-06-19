using System;
using System.Collections.Generic;

#nullable disable

namespace NanoGo.Models
{
    public partial class Log
    {
        public int LogId { get; set; }
        public int LogType { get; set; }
        public string Source { get; set; }
        public string Message { get; set; }
        public string FreeText1 { get; set; }
        public string FreeText2 { get; set; }
        public string FreeText3 { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }

        public virtual User CreatedUserNavigation { get; set; }
        public virtual User UpdatedUserNavigation { get; set; }
    }
}
