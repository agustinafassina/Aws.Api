namespace AwsApi.Services.Interfaces
{
    public interface ILambdaService
    {
        Task<List<string>> ListFunctionsAsync();
    }
}