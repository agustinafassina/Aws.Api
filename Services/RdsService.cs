using Amazon.RDS;
using Amazon.RDS.Model;
using AwsApi.Services.Interfaces;

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

                var instances = new List<DBInstance>();
                string marker = null;

                do
                {
                    var request = new DescribeDBInstancesRequest();
                    if (!string.IsNullOrEmpty(marker))
                    {
                        request.Marker = marker;
                    }

                    var response = await _client.DescribeDBInstancesAsync(request);
                    instances.AddRange(response.DBInstances);

                    marker = response.Marker;

                } while (!string.IsNullOrEmpty(marker));

                _logger.LogInformation("Successfully retrieved {Count} RDS instances", instances.Count);
                return instances;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving RDS instances. Error: {ErrorMessage}", ex.Message);
                throw;
            }
        }
    }
}