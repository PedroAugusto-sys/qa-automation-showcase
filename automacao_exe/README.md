# Hub de Automações - Electron

Aplicação Electron para gerenciamento e execução centralizada de automações de testes.

## Estrutura do Projeto

```
automacao_exe/
├── electron-app/              # Aplicação Electron
│   ├── main/                  # Processo principal
│   ├── renderer/              # Interface React
│   ├── preload/               # Scripts de bridge
│   └── package.json
├── automation-runner/         # Utilitários Node.js
├── config/                     # Configurações
├── logs/                       # Logs exportados
└── AutomacaoHub.IntegrationTests/  # Projeto C# de automação
```

## Instalação

### 1. Instalar dependências do Electron

```bash
cd electron-app
npm install
```

### 2. Instalar dependências do Renderer (React)

```bash
cd electron-app/renderer
npm install
```

### 3. Instalar dependências do projeto C#

```bash
cd AutomacaoHub.IntegrationTests
dotnet restore
```

## Execução

### Modo Desenvolvimento

1. Iniciar o servidor de desenvolvimento do React:
```bash
cd electron-app/renderer
npm run dev
```

2. Em outro terminal, iniciar o Electron:
```bash
cd electron-app
npm run dev
```

### Modo Produção

```bash
cd electron-app
npm run build
npm start
```

## Funcionalidades

- ✅ Dashboard de automações
- ✅ Execução de automações individuais ou múltiplas
- ✅ Visualização de logs em tempo real
- ✅ Gerenciamento de automações (criar features, step definitions, page objects)
- ✅ Exportação de logs (TXT/JSON)
- ✅ Histórico de execuções

## Tecnologias

- **Electron** v28+
- **React** v18+
- **Reqnroll** (BDD framework para C#)
- **Selenium WebDriver 4.x** (automação de browser)
- **Node.js** v18+

## Configuração

1. Abra a aplicação Electron
2. Clique em "Selecione Projeto" na barra superior
3. Selecione o diretório do projeto de automação C#
4. As features serão automaticamente escaneadas e exibidas no dashboard

## Requisitos

- Node.js 18+
- .NET 8.0 SDK
- Chrome/Edge browser (para Selenium WebDriver)

