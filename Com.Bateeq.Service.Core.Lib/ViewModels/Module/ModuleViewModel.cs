using Com.DanLiris.Service.Core.Lib.Helpers;
using System;
using System.Collections.Generic;

namespace Com.DanLiris.Service.Core.Lib.ViewModels.Module
{
    public class ModuleViewModel : BasicViewModelOld
    {

        public string Code { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public configViewModel config { get; set; }



        public class configViewModel
        {
            public destinationViewModel destination { get; set; }

            public sourceViewModel source { get; set; }

        }
        // public string UId { get; set; }


        public class destinationViewModel
        {
            public string type { get; set; }
            public List<string> value { get; set; }
        }

        public class sourceViewModel
        {
            public string type { get; set; }
            public List<string> value { get; set; }
        }
    }
}
