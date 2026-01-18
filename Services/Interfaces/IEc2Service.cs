namespace AwsApi.Services.Interfaces
{
    public interface IEc2Service
    {
        Task<List<Amazon.EC2.Model.Instance>> GetInstancesAsync();
    }
}