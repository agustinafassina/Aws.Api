namespace AwsApi.Services.interfaces
{
    public interface ILambdaService
    {
        Task<List<string>> ListFunctionsAsync();
    }
}