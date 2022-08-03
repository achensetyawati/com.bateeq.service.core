using Com.DanLiris.Service.Core.Lib.Helpers;
using System;

namespace Com.DanLiris.Service.Core.Lib.ViewModels
{
    public class GarmentProductViewModel : BasicViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public GarmentProductUomViewModel UOM { get; set; }
        public string ProductType { get; set; }
        public string Composition { get; set; }
        public string Const { get; set; }
        public string Yarn { get; set; }
        public string Width { get; set; }
        public string Tags { get; set; }
        public string Remark { get; set; }
    }
    public class GarmentProductUomViewModel
    {
        public int? Id { get; set; }
        public string Unit { get; set; }
    }
}
