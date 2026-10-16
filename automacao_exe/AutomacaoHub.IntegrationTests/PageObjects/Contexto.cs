namespace AutomacaoHub.IntegrationTests.PageObjects;

internal static class Contexto
{
    public static string BaseTenant { get; set; } = Environment.GetEnvironmentVariable("QA_TENANT") 
        ?? throw new InvalidOperationException("QA_TENANT environment variable is not set");
    public static string BaseUrl { get; set; } = Environment.GetEnvironmentVariable("QA_BASE_URL") 
        ?? throw new InvalidOperationException("QA_BASE_URL environment variable is not set");
    public static string Url => $"{BaseUrl}/{BaseTenant}";
}

