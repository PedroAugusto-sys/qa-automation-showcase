# AutomacaoHub.IntegrationTests

Projeto de automação de testes usando Reqnroll (BDD) e Selenium WebDriver.

## Estrutura

- `Features/` - Arquivos `.feature` (Gherkin)
- `StepDefinitions/` - Implementações das steps em C#
- `PageObjects/` - Page Objects usando Selenium WebDriver
- `Components/` - Componentes reutilizáveis
- `Drivers/` - WebDrivers (Chrome, Edge, etc.)

## Tecnologias

- **Reqnroll** - Framework BDD para .NET
- **Selenium WebDriver 4.x** - Automação de browser
- **NUnit** - Framework de testes
- **FluentAssertions** - Asserções fluentes

## Execução

### Via linha de comando:
```bash
dotnet test
```

### Executar feature específica:
```bash
dotnet test --filter "FullyQualifiedName~LoginExemplo"
```

### Executar por tag:
```bash
dotnet test --filter "Category=Login"
```

## Configuração

Edite `PageObjects/Contexto.cs` para configurar URL base e tenant.

