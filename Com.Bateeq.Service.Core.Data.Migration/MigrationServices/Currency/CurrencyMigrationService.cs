using Com.Danliris.Service.Core.Mongo.MongoModels;
using Com.Danliris.Service.Core.Mongo.MongoRepositories;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Core.Data.Migration.MigrationServices
{
    public class CurrencyMigrationService : ICurrencyMigrationService
    {
        private readonly ICurrencyMongoRepository _currencyRepository;
        private readonly CoreDbContext _dbContext;
        private readonly DbSet<Currency> _currencyDbSet;

        private int TotalInsertedData { get; set; } = 0;

        public CurrencyMigrationService(ICurrencyMongoRepository currencyRepository, CoreDbContext dbContext)
        {
            _currencyRepository = currencyRepository;
            _dbContext = dbContext;
            _currencyDbSet = _dbContext.Set<Currency>();
        }

        //public Task<int> RunAsync(int startingNumber, int numberOfBatch)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<int> RunAsync(int startingNumber, int numberOfBatch)
        {
            //Extract from Mongo
            var extractedData = await _currencyRepository.GetByBatch(startingNumber, numberOfBatch);

            if (extractedData.Count() > 0)
            {
                var transformedData = Transform(extractedData);
                startingNumber += transformedData.Count;

                //Insert into SQL
                TotalInsertedData += Load(transformedData);

                await RunAsync(startingNumber, numberOfBatch);
            }

            return TotalInsertedData;
        }

        private List<Currency> Transform(IEnumerable<CurrencyMongo> extractedData)
        {
            return extractedData.Select(mongoCurrency => new Currency(mongoCurrency)).ToList();
        }

        private int Load(List<Currency> transformedData)
        {
            var existingUids = _currencyDbSet.Select(entity => entity.UId).ToList();
            transformedData = transformedData.Where(entity => !existingUids.Contains(entity.UId)).ToList();
            if (transformedData.Count > 0)
                _currencyDbSet.AddRange(transformedData);
            return _dbContext.SaveChanges();
        }
    }
}
