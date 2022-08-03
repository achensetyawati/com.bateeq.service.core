using Com.DanLiris.Service.Core.Lib.Helpers;
using System;

namespace Com.DanLiris.Service.Core.Lib.ViewModels
{
    public class HolidayViewModel : BasicViewModelOld
    {
        public string code { get; set; }

        public DateTime? date { get; set; }

        public string name { get; set; }

        public HolidayDivisionViewModel division { get; set; }

        public string description { get; set; }      
    }

    public class HolidayDivisionViewModel
    {
        public int? _id { get; set; }

        public string name { get; set; }
    }
}
