using Com.DanLiris.Service.Core.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.ViewModels
{
    public class MaterialConstructionViewModel : BasicViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
    }
}
