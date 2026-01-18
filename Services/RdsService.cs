using Amazon.RDS;
using Amazon.RDS.Model;
using AwsApi.Services.interfaces;

namespace AwsApi.Services
{
    public class RdsService : IRdsService
    {
        private readonly IAmazonRDS _client;
        private readonly ILogger<RdsService> _logger;

        public RdsService(IAmazonRDS client, ILogger<RdsService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<List<DBInstance>> GetDbInstancesAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving RDS instances");

                var response = await _client.DescribeDBInstancesAsync();
                
                _logger.LogInformation("Successfully retrieved {Count} RDS instances", response.DBInstances.Count);
                return response.DBInstances;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving RDS instances. Error: {ErrorMessage}", ex.Message);
                throw;
            }
        }
    }
}