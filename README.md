# qa-automation-showcase

Testes end-to-end em C# com SpecFlow (BDD) e Playwright para um sistema web de gestão escolar, mais um hub em Electron para escolher e rodar as features.

*End-to-end tests in C# with SpecFlow (BDD) and Playwright for a school-management web app, plus an Electron hub to pick and run features. English below.*

## O que é testado

- Login de operador e permissões (Segurança)
- Acadêmico: séries, turmas, disciplinas, matrícula, remanejamento, boletins, frequência, avaliações
- Financeiro: resumo financeiro do aluno
- Cadastros gerais (cidades, empresa, professores, responsáveis) e tela de versão

## Arquitetura

- `QaShowcase.Specflow.IntegrationTests/` — features Gherkin (`Features/`), step definitions (`StepDefinitions/`) e page objects Playwright (`PageObjects/`, com `POBase`/`POFactory`). Hooks de browser em `BrowserHooks.cs`/`BrowserManager.cs`; dados fake com Bogus; asserts com FluentAssertions; NUnit como runner.
- `automacao_exe/AutomacaoHub.IntegrationTests/` — projeto C# (Reqnroll + Selenium) usado pelo hub.
- `automacao_exe/electron-app/` — hub Electron (main / preload / renderer) que lista features e dispara `dotnet test`.
- `automacao_exe/automation-runner/` — utilitários Node: leitura de features, parser de logs e gerador de templates.

## Setup

Requisitos: .NET SDK, Node.js, PowerShell (para instalar os browsers do Playwright).

```bash
cp .env.example .env   # preencha com seus valores
```

| Variável | Uso |
|---|---|
| `QA_TOKEN_URL` | endpoint de token/autenticação |
| `QA_USER` / `QA_PASSWORD` | usuário de teste |
| `QA_BASE_URL` | URL da aplicação |
| `QA_TENANT` | tenant usado no login |
| `QA_DEFAULT_OPERATOR_PASSWORD` | senha dos operadores criados pelos testes |

As variáveis precisam estar no ambiente ao rodar os testes.

## Rodando os testes

```bash
dotnet restore
dotnet build
pwsh QaShowcase.Specflow.IntegrationTests/bin/Debug/<tfm>/playwright.ps1 install
dotnet test QaShowcase.Specflow.IntegrationTests.sln
dotnet test --filter "FullyQualifiedName~LoginOperador"   # uma feature
```

## Hub Electron

```bash
cd automacao_exe/electron-app
npm install
cd renderer && npm install && cd ..
npm start        # ou: npm run dev
npm run build:win   # build Windows (pasta dist/)
```

## Estrutura

```
QaShowcase.Specflow.IntegrationTests/   testes SpecFlow + Playwright
automacao_exe/
  AutomacaoHub.IntegrationTests/        testes Reqnroll + Selenium
  electron-app/                         hub (main, preload, renderer)
  automation-runner/                    utilitários Node
.env.example
```

---

## English

**What it tests:** operator login and permissions, academic flows (grades, classes, subjects, enrollment, transfers, report cards, attendance, assessments), student financial summary, general registries and the version screen.

**Architecture:** Gherkin features, step definitions and Playwright page objects in `QaShowcase.Specflow.IntegrationTests/` (NUnit, Bogus, FluentAssertions). `automacao_exe/` holds a Reqnroll + Selenium project, the Electron hub that lists features and runs `dotnet test`, and Node helpers (feature scanner, log parser, template generator).

**Setup:** copy `.env.example` to `.env` and fill in `QA_TOKEN_URL`, `QA_USER`, `QA_PASSWORD`, `QA_BASE_URL`, `QA_TENANT`, `QA_DEFAULT_OPERATOR_PASSWORD`; export them before running.

**Run tests:** `dotnet restore && dotnet build`, install Playwright browsers with the generated `playwright.ps1 install`, then `dotnet test`.

**Run the hub:** in `automacao_exe/electron-app`, `npm install` (also in `renderer/`), then `npm start` (or `npm run dev`).
