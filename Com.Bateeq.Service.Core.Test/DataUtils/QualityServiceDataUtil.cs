using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.DanLiris.Service.Core.Test.DataUtils
{
    class QualityServiceDataUtil
    {
        public CoreDbContext DbContext { get; set; }
        public QualityService QualityService { get; set; }

        public QualityServiceDataUtil(CoreDbContext dbContext, QualityService qualityService)
        {
            this.DbContext = dbContext;
            this.QualityService = qualityService;
        }
        public Task<Quality> GetTestBuget()
        {
            Quality test = QualityService.DbSet.FirstOrDefault(comodity => comodity.Code.Equals("Test"));

            if (test != null)
                return Task.FromResult(test);
            else
            {
                string guid = Guid.NewGuid().ToString();
                test = new Quality()
                {
                    Code = guid,
                    Name = string.Format("Test {0}", guid),
                };

                int id = QualityService.Create(test);
                return QualityService.GetAsync(id);
            }
        }
    }
}
