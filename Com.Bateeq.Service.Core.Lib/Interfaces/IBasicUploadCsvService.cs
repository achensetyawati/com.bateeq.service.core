using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;

namespace Com.DanLiris.Service.Core.Lib.Interfaces
{
    public interface IBasicUploadCsvService<TViewModel>
    {
        Tuple<bool, List<object>> UploadValidate(List<TViewModel> Data, List<KeyValuePair<string, StringValues>> Body);

        List<string> CsvHeader { get; }
    }
}
