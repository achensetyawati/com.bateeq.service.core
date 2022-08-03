using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.ViewModels.Module;
using System;
using System.Collections.Generic;

namespace Com.DanLiris.Service.Core.Lib.ViewModels
{
    public class StorageViewModel : BasicViewModelOld
    {

        public string code { get; set; }

        public string name { get; set; }

        public string description { get; set; }


        public string address { get; set; }
        public string phone { get; set; }
        public bool isCentral { get; set; }

        public StorageUnitViewModel unit { get; set; }

        public List<ModuleSourceViewModel> moduleSources { get; set; }
        public List<ModuleDestinationViewModel> moduleDestinations { get; set; }
    }

    public class StorageUnitViewModel
    {
        public int? _id { get; set; }

        public string name { get; set; }

        public DivisionViewModel division { get; set; }
    }

    public class ModuleSourceViewModel : BasicViewModelOld
    {
        public ModuleViewModel moduleSource { get; set; }
    }

    public class ModuleDestinationViewModel : BasicViewModelOld
    {
        public ModuleViewModel moduleDestination { get; set; }
    }
}
