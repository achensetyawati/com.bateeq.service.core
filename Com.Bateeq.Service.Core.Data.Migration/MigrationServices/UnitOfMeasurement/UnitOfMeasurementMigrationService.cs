using Com.Danliris.Service.Core.Mongo.MongoModels;
using Com.Danliris.Service.Core.Mongo.MongoRepositories;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Core.Data.Migration.MigrationServices
{
    public class UnitOfMeasurementMigrationService : IUnitOfMeasurementMigrationService
    {
        private readonly IUnitOfMeasurementMongoRepository _unitOfMeasurementRepository;
        private readonly CoreDbContext _dbContext;
        private readonly DbSet<Uom> _sqlUnitOfMeasurementDbSet;
        private int TotalInsertedData { get; set; } = 0;

        public UnitOfMeasurementMigrationService(IUnitOfMeasurementMongoRepository unitOfMeasurementRepository, CoreDbContext dbContext)
        {
            _unitOfMeasurementRepository = unitOfMeasurementRepository;
            _dbContext = dbContext;
            _sqlUnitOfMeasurementDbSet = _dbContext.Set<Uom>();
        }

        public async Task<int> RunAsync(int startingNumber, int numberOfBatch)
        {
            //Extract from Mongo
            var extractedData = await _unitOfMeasurementRepository.GetByBatch(startingNumber, numberOfBatch);

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

        private List<Uom> Transform(IEnumerable<UnitOfMeasurementMongo> extractedData)
        {
            return extractedData.Select(mongoUom => new Uom(mongoUom)).ToList();
        }

        private int Load(List<Uom> transformedData)
        {
            var existingUids = _sqlUnitOfMeasurementDbSet.Select(entity => entity.UId).ToList();
            transformedData = transformedData.Where(entity => !existingUids.Contains(entity.UId)).ToList();
            if (transformedData.Count > 0)
                _sqlUnitOfMeasurementDbSet.AddRange(transformedData);
            return _dbContext.SaveChanges();
        }
    }
}
