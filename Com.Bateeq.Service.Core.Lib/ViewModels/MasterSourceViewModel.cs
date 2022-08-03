using System;
using Com.DanLiris.Service.Core.Lib.Helpers;

namespace Com.DanLiris.Service.Core.Lib.ViewModels
{
    public class MasterSourceViewModel : BasicViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
