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
    public class ProductMigrationService : IProductMigrationService
    {
        private readonly IProductMongoRepository _productRepository;
        private readonly CoreDbContext _dbContext;
        private readonly DbSet<Product> _sqlProductDbSet;
        private readonly DbSet<ProductSPPProperty> _sqlProductSPPPropertiesDbSet;
        private readonly DbSet<Uom> _sqlUomDbSet;
        private readonly DbSet<Currency> _sqlCurrencyDbSet;

        private int TotalInsertedData { get; set; } = 0;

        public ProductMigrationService(IProductMongoRepository productRepository, CoreDbContext dbContext)
        {
            _productRepository = productRepository;
            _dbContext = dbContext;
            _sqlProductDbSet = _dbContext.Set<Product>();
            _sqlProductSPPPropertiesDbSet = _dbContext.Set<ProductSPPProperty>();
            _sqlUomDbSet = _dbContext.Set<Uom>();
            _sqlCurrencyDbSet = _dbContext.Set<Currency>();
        }

        public async Task<int> RunAsync(int startingNumber, int numberOfBatch)
        {
            //Extract from Mongo
            var extractedData = await _productRepository.GetByBatch(startingNumber, numberOfBatch);

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

        private List<Product> Transform(IEnumerable<ProductMongo> extractedData)
        {
            return extractedData.Select(mongoProduct =>
            {
                var uom = _sqlUomDbSet.FirstOrDefault(f => f.UId.Equals(mongoProduct.uom._id.ToString()));
                var currency = _sqlCurrencyDbSet.FirstOrDefault(f => f.UId.Equals(mongoProduct.currency._id.ToString()));
                return new Product(mongoProduct, uom, currency);
            }).ToList();
        }

        private int Load(List<Product> transformedData)
        {
            var existingUids = _sqlProductDbSet.Select(entity => entity.UId).ToList();
            transformedData = transformedData.Where(entity => !existingUids.Contains(entity.UId)).ToList();
            if (transformedData.Count > 0)
                _sqlProductDbSet.AddRange(transformedData);
            return _dbContext.SaveChanges();
        }
    }
}
