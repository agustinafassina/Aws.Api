using Amazon.CostExplorer.Model;

namespace AwsApi.Services.Interfaces
{
    public interface ICostsService
    {
        Task<GetCostAndUsageResponse> GetCostsByTagAsync(string tagValue);
        Task<GetCostAndUsageResponse> GetCostsReport(string tagValue);
        Task<GetCostAndUsageResponse>  GetAllCostsAsync();
    }
}