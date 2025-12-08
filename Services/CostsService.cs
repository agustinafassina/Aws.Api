using System.Text;
using System.Text.Json;
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
            var today = DateTime.UtcNow;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            List<string> projects = new List<string>{
                tagValue
            };

            var request = new GetCostAndUsageRequest
            {
                TimePeriod = new DateInterval
                {
                    Start = startOfMonth.ToString("yyyy-MM-dd"),
                    End = endOfMonth.ToString("yyyy-MM-dd")
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

        public async Task<GetCostAndUsageResponse> GetCostsReport(string tagValue)
        {
            GetCostAndUsageResponse response = await GetCostsByTagAsync(tagValue);
            var costosPorServicio = new Dictionary<string, decimal>();
            decimal totalCost = 0;

            foreach (var resultado in response.ResultsByTime)
            {
                foreach (var grupo in resultado.Groups)
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
            }

            var today = DateTime.UtcNow;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
            string startDate = startOfMonth.ToString("yyyy-MM-dd");
            string endDate = endOfMonth.ToString("yyyy-MM-dd");

            GenerateHtml(tagValue, costosPorServicio, totalCost, startDate, endDate);
            return response;
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
    }
}