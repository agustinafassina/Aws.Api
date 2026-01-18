using System.Text;
using Amazon.CostExplorer;
using Amazon.CostExplorer.Model;
using AwsApi.Services.Interfaces;

namespace AwsApi.Services
{
    public class CostsService : ICostsService
    {
        private readonly IAmazonCostExplorer _costExplorer;
        private readonly ILogger<CostsService> _logger;

        public CostsService(IAmazonCostExplorer costExplorer, ILogger<CostsService> logger)
        {
            _costExplorer = costExplorer;
            _logger = logger;
        }

        public async Task<GetCostAndUsageResponse> GetCostsByTagAsync(string tagValue)
        {
            try
            {
                _logger.LogInformation("Getting costs for project tag: {TagValue}", tagValue);
                string projectValueTag = "Project";

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
                var costosPorServicio = new Dictionary<string, decimal>();
                decimal totalCost = 0;

                foreach (var resultado in response.ResultsByTime)
                {
                    foreach (var grupo in resultado.Groups)
                    {
                        try
                        {
                            string servicio = grupo.Keys[0];
                            decimal costo = decimal.Parse(grupo.Metrics["BlendedCost"].Amount);
                            totalCost += costo;

                            if (costosPorServicio.ContainsKey(servicio))
                            {
                                costosPorServicio[servicio] += costo;
                            }
                            else
                            {
                                costosPorServicio[servicio] = costo;
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
                    //GenerateHtml(tagValue, costosPorServicio, totalCost, startDate, endDate);
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

        private static string GenerateHtml(string tagValue, Dictionary<string, decimal> costosPorServicio, decimal totalCost, string startDate, string endDate)
        {
            string fileName = "cost_report.html";

            var sb = new StringBuilder();
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<title>AWS Cost Report</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("table { border-collapse: collapse; width: 100%; }");
            sb.AppendLine("th, td { border: 1px solid #7c3838ff; padding: 8px; }");
            sb.AppendLine("th { background-color: #7c3838ff; color: white; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine($"<h3>Project: {tagValue}</h3>");
            sb.AppendLine($"<h2>Cost report ({startDate} - {endDate})</h2>");
            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>Service</th><th>Cost (USD)</th></tr>");

            foreach (var servicio in costosPorServicio)
            {
                sb.AppendLine($"<tr><td>{servicio.Key}</td><td>{servicio.Value:F2}</td></tr>");
            }

            sb.AppendLine($"<tr><td><strong>Total</strong></td><td><strong>{totalCost:F2} USD</strong></td></tr>");
            sb.AppendLine("</table>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            File.WriteAllText($"{fileName}", sb.ToString());
            Console.WriteLine($"Report: {fileName}");
            return fileName;
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