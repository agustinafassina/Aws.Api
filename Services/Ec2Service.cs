using Amazon.EC2;
using Amazon.EC2.Model;
using AwsApi.Services.Interfaces;

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

                var instances = new List<Instance>();
                string nextToken = null;

                do
                {
                    var request = new DescribeInstancesRequest();
                    if (!string.IsNullOrEmpty(nextToken))
                    {
                        request.NextToken = nextToken;
                    }

                    var response = await _client.DescribeInstancesAsync(request);

                    foreach (var reservation in response.Reservations)
                    {
                        instances.AddRange(reservation.Instances);
                    }

                    nextToken = response.NextToken;

                } while (!string.IsNullOrEmpty(nextToken));

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