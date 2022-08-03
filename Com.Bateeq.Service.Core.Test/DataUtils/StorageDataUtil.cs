using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Models.Module;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Test.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.DanLiris.Service.Core.Test.DataUtils
{
    public class StorageDataUtil : BasicDataUtil<CoreDbContext, StorageService, Storage>, IEmptyData<StorageViewModel>
    {
        public StorageDataUtil(CoreDbContext dbContext, StorageService service) : base(dbContext, service)
        {
        }

        public StorageViewModel GetEmptyData()
        {
            return new StorageViewModel();
        }

        public override Storage GetNewData()
        {
            string guid = Guid.NewGuid().ToString();
            return new Storage
            {
                Name = string.Format("StorageName {0}", guid),
                UnitId = 1,
                Code= string.Format("StorageName {0}", guid),
                UId= string.Format("StorageName {0}", guid),
                ModuleDestinations =new List<ModuleDestination>()
                {
                    new ModuleDestination()
                    {
                        ModuleId=1,
                        StorageId=1,
                        Module=new Module()
                        {
                            Id=1,
                            Name="name",
                            Code=string.Format("StorageName {0}", guid)
                        },
                        UId=string.Format("StorageName {0}", guid),
                        DestinationValue=string.Format("StorageName {0}", guid),
                    }
                },
                ModuleSources= new List<ModuleSource>()
                {
                    new ModuleSource()
                    {
                        ModuleId=1,
                        StorageId=1,
                        Module =new Module()
                        {
                            Id=1,
                            Name="name",
                            Code=string.Format("StorageName {0}", guid)
                        },
                        UId=string.Format("StorageName {0}", guid),
                        SourceValue=string.Format("StorageName {0}", guid),
                    }
                }
                
            };
        }

        public override async Task<Storage> GetTestDataAsync()
        {
            var data = GetNewData();
            await Service.CreateModel(data);
            return data;
        }
    }
}
