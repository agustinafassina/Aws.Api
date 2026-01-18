using Amazon.RDS.Model;

namespace AwsApi.Services.Interfaces
{
    public interface IRdsService
    {
        Task<List<DBInstance>> GetDbInstancesAsync();
    }
}