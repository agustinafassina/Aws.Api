using Amazon.Lambda;
using Amazon.Lambda.Model;
using AwsApi.Services.interfaces;

namespace AwsApi.Services
{
    public class LambdaService : ILambdaService
    {
        private readonly IAmazonLambda _lambdaClient;

        public LambdaService(IAmazonLambda lambdaClient)
        {
            _lambdaClient = lambdaClient;
        }

        public async Task<List<string>> ListFunctionsAsync()
        {
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

            return functions;
        }
    }
}