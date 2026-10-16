namespace QaShowcase.Specflow.IntegrationTests.PageObjects;

/// <summary>Centralizes test configuration read from environment variables (see .env.example).</summary>
internal static class TestConfig
{
	public static string DefaultOperatorPassword =>
		Environment.GetEnvironmentVariable("QA_DEFAULT_OPERATOR_PASSWORD")
		?? throw new InvalidOperationException("QA_DEFAULT_OPERATOR_PASSWORD environment variable is not set");

	/// <summary>Resolves feature-file placeholders: &lt;DEFAULT_PASSWORD&gt; and &lt;GERAR_CPF&gt; (random valid test CPF).</summary>
	public static string Resolve(string value) => value?.Trim() switch
	{
		"<DEFAULT_PASSWORD>" => DefaultOperatorPassword,
		"<GERAR_CPF>" => Components.GeradorDeCPF.GerarCpf(),
		_ => value!
	};
}
