using System.Threading.Tasks;

namespace Com.Danliris.Service.Core.Data.Migration.MigrationServices
{
    public interface ICurrencyMigrationService
    {
        Task<int> RunAsync(int startingNumber, int numberOfBatch);
    }
}
