using Amazon.CostExplorer.Model;
using AwsApi.Services.Dto;

namespace AwsApi.Services.interfaces
{
    public interface ICostsService
    {
        Task<GetCostAndUsageResponse> GetCostsByTagAsync(string tagValue);
        Task<GetCostAndUsageResponse> GetCostsReport(string tagValue);
    }
}