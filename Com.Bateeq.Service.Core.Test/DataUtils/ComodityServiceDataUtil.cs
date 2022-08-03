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
    class ComodityServiceDataUtil
    {
        public CoreDbContext DbContext { get; set; }

        public ComodityService ComodityService { get; set; }

        public ComodityServiceDataUtil(CoreDbContext dbContext, ComodityService comodityService)
        {
            this.DbContext = dbContext;
            this.ComodityService = comodityService;
        }

        public Task<Comodity> GetTestBuget()
        {
            Comodity test = ComodityService.DbSet.FirstOrDefault(comodity => comodity.Code.Equals("Test"));

            if (test != null)
                return Task.FromResult(test);
            else
            {
                string guid = Guid.NewGuid().ToString();
                test = new Comodity()
                {
                    Code = guid,
                    Name = string.Format("Test {0}", guid),
                };

                int id = ComodityService.Create(test);
                return ComodityService.GetAsync(id);
            }
        }
    }
}
