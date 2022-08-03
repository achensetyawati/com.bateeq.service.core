using System.Threading.Tasks;

namespace Com.Danliris.Service.Core.Data.Migration.MigrationServices
{
    public interface IProductMigrationService
    {
        Task<int> RunAsync(int startingNumber, int numberOfBatch);
    }
}
