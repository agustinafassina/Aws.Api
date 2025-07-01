namespace AwsApi.Services
{
    public interface IEc2Service
    {
        Task<List<Amazon.EC2.Model.Instance>> GetInstancesAsync();
    }
}