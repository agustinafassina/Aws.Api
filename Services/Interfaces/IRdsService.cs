using Amazon.RDS.Model;

namespace AwsApi.Services.interfaces
{
    public interface IRdsService
    {
        Task<List<DBInstance>> GetDbInstancesAsync();
    }
}