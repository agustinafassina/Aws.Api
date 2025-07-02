using Amazon.RDS;
using Amazon.RDS.Model;
using AwsApi.Services.interfaces;

namespace AwsApi.Services
{
    public class RdsService : IRdsService
    {
        private readonly IAmazonRDS _client;

        public RdsService(IAmazonRDS client)
        {
            _client = client;
        }

        public async Task<List<DBInstance>> GetDbInstancesAsync()
        {
            var response = await _client.DescribeDBInstancesAsync();
            return response.DBInstances;
        }
    }
}