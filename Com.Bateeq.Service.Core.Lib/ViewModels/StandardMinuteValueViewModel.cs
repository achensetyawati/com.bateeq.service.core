using Com.DanLiris.Service.Core.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.ViewModels
{
    public class StandardMinuteValueViewModel : BasicViewModel
    {
        public int BuyerId { get; set; }
        public string BuyerCode { get; set; }
        public string BuyerName { get; set; }
        public int ComodityId { get; set; }
        public string ComodityCode { get; set; }
        public string ComodityName { get; set; }
        public DateTimeOffset SMVDate { get; set; }
        public decimal SMVCutting { get; set; }
        public decimal SMVSewing { get; set; }
        public decimal SMVFinishing { get; set; }
    }
}
