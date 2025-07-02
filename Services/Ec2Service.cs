using Amazon.EC2;
using Amazon.EC2.Model;
using AwsApi.Services.interfaces;

namespace AwsApi.Services
{
    public class Ec2Service : IEc2Service
    {
        private readonly IAmazonEC2 _client;

        public Ec2Service(IAmazonEC2 client)
        {
            _client = client;
        }

        public async Task<List<Instance>> GetInstancesAsync()
        {
            var response = await _client.DescribeInstancesAsync();
            var instances = new List<Instance>();

            foreach (var reservation in response.Reservations)
            {
                instances.AddRange(reservation.Instances);
            }
            return instances;
        }
    }
}