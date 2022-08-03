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
    class YarnMaterialServiceDataUtil
    {
        public CoreDbContext DbContext { get; set; }
        public YarnMaterialService YarnMaterialService { get; set; }

        public YarnMaterialServiceDataUtil(CoreDbContext dbContext, YarnMaterialService yarnMaterialService)
        {
            this.DbContext = dbContext;
            this.YarnMaterialService = yarnMaterialService;
        }
        public Task<YarnMaterial> GetTestBuget()
        {
            YarnMaterial test = YarnMaterialService.DbSet.FirstOrDefault(comodity => comodity.Code.Equals("Test"));

            if (test != null)
                return Task.FromResult(test);
            else
            {
                string guid = Guid.NewGuid().ToString();
                test = new YarnMaterial()
                {
                    Code =guid,
                    Name = string.Format("Test {0}", guid),
                    Remark = "Remark"
                };

                int id = YarnMaterialService.Create(test);
                return YarnMaterialService.GetAsync(id);
            }
        }
    }
}
