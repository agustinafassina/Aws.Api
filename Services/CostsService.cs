using System.Text;
using Amazon.CostExplorer;
using Amazon.CostExplorer.Model;
using AwsApi.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AwsApi.Services
{
    public class CostsService : ICostsService
    {
        private readonly IAmazonCostExplorer _costExplorer;
        private readonly ILogger<CostsService> _logger;
        private readonly IConfiguration _configuration;

        public CostsService(IAmazonCostExplorer costExplorer, ILogger<CostsService> logger, IConfiguration configuration)
        {
            _costExplorer = costExplorer;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<GetCostAndUsageResponse> GetCostsByTagAsync(string tagValue)
        {
            try
            {
                _logger.LogInformation("Getting costs for project tag: {TagValue}", tagValue);
                string projectValueTag = _configuration["Aws:ProjectTagKey"] ?? "Project";

                var (startDate, endDate) = GetCurrentMonthDates();

                List<string> projects = new List<string>{
                    tagValue
                };

                var request = new GetCostAndUsageRequest
                {
                    TimePeriod = new DateInterval
                    {
                        Start = startDate,
                        End = endDate
                    },
                    Granularity = Granularity.MONTHLY,
                    Metrics = new List<string> { "BlendedCost" },
                    GroupBy = new List<GroupDefinition>
                    {
                        new GroupDefinition
                        {
                            Key = "SERVICE",
                            Type = GroupDefinitionType.DIMENSION
                        }
                    }
                };

                request.Filter = new Expression
                {
                    Tags = new TagValues
                    {
                        Key = projectValueTag,
                        Values = projects
                    }
                };

                GetCostAndUsageResponse response = await _costExplorer.GetCostAndUsageAsync(request);
                _logger.LogInformation("Successfully retrieved costs for project tag: {TagValue}", tagValue);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting costs for project tag: {TagValue}. Error: {ErrorMessage}", tagValue, ex.Message);
                throw;
            }
        }

        public async Task<GetCostAndUsageResponse> GetCostsReport(string tagValue)
        {
            try
            {
                _logger.LogInformation("Generating cost report for project tag: {TagValue}", tagValue);

                GetCostAndUsageResponse response = await GetCostsByTagAsync(tagValue);
                var costsByService = new Dictionary<string, decimal>();
                decimal totalCost = 0;

                foreach (var result in response.ResultsByTime)
                {
                    foreach (var group in result.Groups)
                    {
                        try
                        {
                            string service = group.Keys[0];
                            decimal cost = decimal.Parse(group.Metrics["BlendedCost"].Amount);
                            totalCost += cost;

                            if (costsByService.ContainsKey(service))
                            {
                                costsByService[service] += cost;
                            }
                            else
                            {
                                costsByService[service] = cost;
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Error processing cost group for service. Continuing with next group.");
                            continue;
                        }
                    }
                }

                var (startDate, endDate) = GetCurrentMonthDates();

                try
                {
                    _logger.LogInformation("HTML report generated successfully for project tag: {TagValue}", tagValue);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error generating HTML report for project tag: {TagValue}. Continuing without HTML.", tagValue);
                }

                _logger.LogInformation("Cost report generated successfully for project tag: {TagValue}. Total cost: {TotalCost}", tagValue, totalCost);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating cost report for project tag: {TagValue}. Error: {ErrorMessage}", tagValue, ex.Message);
                throw;
            }
        }

        public async Task<GetCostAndUsageResponse> GetAllCostsAsync()
        {
            try
            {
                _logger.LogInformation("Getting all costs for current month");

                var (startDate, endDate) = GetCurrentMonthDates();

                var request = new GetCostAndUsageRequest
                {
                    TimePeriod = new DateInterval
                    {
                        Start = startDate,
                        End = endDate
                    },
                    Granularity = Granularity.MONTHLY,
                    Metrics = new List<string> { "BlendedCost" },
                    GroupBy = new List<GroupDefinition>
                    {
                        new GroupDefinition
                        {
                            Key = "SERVICE",
                            Type = GroupDefinitionType.DIMENSION
                        }
                    }
                };

                GetCostAndUsageResponse response = await _costExplorer.GetCostAndUsageAsync(request);
                _logger.LogInformation("Successfully retrieved all costs for current month");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all costs. Error: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        private (string startDate, string endDate) GetCurrentMonthDates()
        {
            var today = DateTime.UtcNow;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
            return (startOfMonth.ToString("yyyy-MM-dd"), endOfMonth.ToString("yyyy-MM-dd"));
        }
    }
}