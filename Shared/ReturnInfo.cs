using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NanoGo.Shared
{
    public class ReturnInfo<T>
    {
        public ReturnInfo()
        {
            IsSuccess = false;
            IsLoginRequired = false;
        }
        public int TotalCount { get; set; }
        public bool IsSuccess { get; set; }
        public List<T> Data { get; set; }
        public string Key { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsLoginRequired { get; set; }

    }

    public class ReturnInfo
    {
        public ReturnInfo()
        {
            IsSuccess = false;
            IsLoginRequired = false;
        }
        public int TotalCount { get; set; }
        public bool IsSuccess { get; set; }
        public object Data { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsLoginRequired { get; set; }

    }
}
