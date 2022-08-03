using System;
using System.Collections.Generic;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.Helpers
{
    public static class Timestamp
    {
        private const string TIMESTAMP_FORMAT = "yyyyMMddHHmmssffff";
        public static string Generate(DateTime value)
        {
            return value.ToString(TIMESTAMP_FORMAT);
        }
    }
}
