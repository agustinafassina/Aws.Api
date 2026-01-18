using Amazon.Lambda;
using Amazon.Lambda.Model;
using AwsApi.Services.interfaces;

namespace AwsApi.Services
{
    public class LambdaService : ILambdaService
    {
        private readonly IAmazonLambda _lambdaClient;
        private readonly ILogger<LambdaService> _logger;

        public LambdaService(IAmazonLambda lambdaClient, ILogger<LambdaService> logger)
        {
            _lambdaClient = lambdaClient;
            _logger = logger;
        }

        public async Task<List<string>> ListFunctionsAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving Lambda functions");

                var functions = new List<string>();
                string marker = null;

                do
                {
                    var request = new ListFunctionsRequest { Marker = marker };
                    var response = await _lambdaClient.ListFunctionsAsync(request);

                    foreach (var function in response.Functions)
                    {
                        functions.Add(function.FunctionName);
                    }
                    marker = response.NextMarker;

                } while (!string.IsNullOrEmpty(marker));

                _logger.LogInformation("Successfully retrieved {Count} Lambda functions", functions.Count);
                return functions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Lambda functions. Error: {ErrorMessage}", ex.Message);
                throw;
            }
        }
    }
}