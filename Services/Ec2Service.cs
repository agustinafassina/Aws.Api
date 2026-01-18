using Amazon.EC2;
using Amazon.EC2.Model;
using AwsApi.Services.interfaces;

namespace AwsApi.Services
{
    public class Ec2Service : IEc2Service
    {
        private readonly IAmazonEC2 _client;
        private readonly ILogger<Ec2Service> _logger;

        public Ec2Service(IAmazonEC2 client, ILogger<Ec2Service> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<List<Instance>> GetInstancesAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving EC2 instances");

                var response = await _client.DescribeInstancesAsync();
                var instances = new List<Instance>();

                foreach (var reservation in response.Reservations)
                {
                    instances.AddRange(reservation.Instances);
                }

                _logger.LogInformation("Successfully retrieved {Count} EC2 instances", instances.Count);
                return instances;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving EC2 instances. Error: {ErrorMessage}", ex.Message);
                throw;
            }
        }
    }
}