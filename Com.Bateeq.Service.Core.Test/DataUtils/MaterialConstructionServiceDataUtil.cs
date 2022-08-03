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
    class MaterialConstructionServiceDataUtil
    {
        public CoreDbContext DbContext { get; set; }
        public MaterialConstructionService MaterialConstructionService { get; set; }

        public MaterialConstructionServiceDataUtil(CoreDbContext dbContext, MaterialConstructionService materialConstructionService)
        {
            this.DbContext = dbContext;
            this.MaterialConstructionService = materialConstructionService;
        }
        public Task<MaterialConstruction> GetTestBuget()
        {
            MaterialConstruction test = MaterialConstructionService.DbSet.FirstOrDefault(comodity => comodity.Code.Equals("Test"));

            if (test != null)
                return Task.FromResult(test);
            else
            {
                string guid = Guid.NewGuid().ToString();
                test = new MaterialConstruction()
                {
                    Code = guid,
                    Name = string.Format("Test {0}", guid),
                };

                int id = MaterialConstructionService.Create(test);
                return MaterialConstructionService.GetAsync(id);
            }
        }
    }
}
