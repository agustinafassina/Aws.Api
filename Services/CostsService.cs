using Amazon.CostExplorer;
using Amazon.CostExplorer.Model;
using AwsApi.Services.interfaces;

namespace AwsApi.Services
{
    public class CostsService : ICostsService
    {
        private readonly IAmazonCostExplorer _costExplorer;

        public CostsService(IAmazonCostExplorer costExplorer)
        {
            _costExplorer = costExplorer;
        }

        public async Task<GetCostAndUsageResponse> GetCostsByTagAsync(string tagValue)
        {
            List<string> projects = new List<string>{
                tagValue
            };

            var request = new GetCostAndUsageRequest
            {
                TimePeriod = new DateInterval
                {
                    Start = DateTime.UtcNow.AddMonths(-1).ToString("yyyy-MM-dd"),
                    End = DateTime.UtcNow.ToString("yyyy-MM-dd")
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
                    Key = "Project",
                    Values = projects
                }
            };

            GetCostAndUsageResponse response = await _costExplorer.GetCostAndUsageAsync(request);
            return response;
        }
    }
}