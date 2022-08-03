using Com.DanLiris.Service.Core.Lib.Helpers;
using System;

namespace Com.DanLiris.Service.Core.Lib.ViewModels
{
    
        public class ArticleSubCounterViewModel : BasicViewModelOld
        {
            public string code { get; set; }

            public string name { get; set; }

            public string Description { get; set; }
            public DateTimeOffset Date { get; set; }
        }
    
}
