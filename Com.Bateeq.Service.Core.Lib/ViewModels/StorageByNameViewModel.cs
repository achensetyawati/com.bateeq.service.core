using System;
using System.Collections.Generic;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.ViewModels
{
    public class StorageByNameViewModel
    {
        public string Code { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public UnitStorage Unit { get; set; }
    }

    public class UnitStorage
    {
        public string Name { get; set; }
        public DivisionStorage Division { get; set; }
    }

    public class DivisionStorage
    {
        public string Name { get; set; }
    }
}
