using Amazon.RDS.Model;

namespace AwsApi.Services
{
    public interface IRdsService
    {
        Task<List<DBInstance>> GetDbInstancesAsync();
    }
}