/**
 * Traduz termos BDD para português
 * @param {string} step - Step em inglês
 * @returns {string} Step traduzido
 */
function translateBDDStep(step) {
  if (!step) return step;
  
  // Traduções básicas
  const translations = {
    'Given': 'Dado',
    'When': 'Quando',
    'Then': 'Então',
    'And': 'E',
    'But': 'Mas',
    'Given que': 'Dado que',
    'Given ': 'Dado ',
    'When ': 'Quando ',
    'Then ': 'Então ',
    'And ': 'E ',
    'But ': 'Mas '
  };
  
  let translated = step;
  for (const [en, pt] of Object.entries(translations)) {
    translated = translated.replace(new RegExp(en, 'gi'), pt);
  }
  
  return translated;
}

/**
 * Detecta e extrai informações de timeout do erro
 * @param {string} errorMessage - Mensagem de erro
 * @returns {Object|null} Objeto com informações de timeout ou null
 */
function detectTimeoutInfo(errorMessage) {
  if (!errorMessage) return null;
  
  // Padrão: "Timeout 5000ms exceeded."
  const timeoutMatch = errorMessage.match(/Timeout\s+(\d+)\s*(ms|s|milliseconds?|seconds?)\s+exceeded/i);
  if (timeoutMatch) {
    const timeoutValue = parseInt(timeoutMatch[1]);
    const timeoutUnit = timeoutMatch[2];
    
    // Procura por "waiting for Locator(...)" ou similar
    const locatorMatch = errorMessage.match(/waiting\s+for\s+(Locator\([^)]+\)|"[^"]+"|'[^']+'|\.\w+)/i);
    const locator = locatorMatch ? locatorMatch[1] : null;
    
    // Procura por "Filter" ou "HasText" que podem indicar condições adicionais
    const filterMatch = errorMessage.match(/Filter\([^)]+\)/i);
    const hasTextMatch = errorMessage.match(/HasText\s*=\s*["']([^"']+)["']/i);
    
    return {
      type: 'timeout',
      timeoutValue,
      timeoutUnit,
      locator,
      filter: filterMatch ? filterMatch[0] : null,
      expectedText: hasTextMatch ? hasTextMatch[1] : null,
      message: `Timeout de ${timeoutValue}${timeoutUnit} excedido${locator ? ` ao aguardar: ${locator}` : ''}`
    };
  }
  
  return null;
}

/**
 * Detecta tipo de erro e extrai informações detalhadas
 * @param {string} errorMessage - Mensagem de erro
 * @param {string} stackTrace - Stack trace (opcional)
 * @returns {Object} Objeto com tipo de erro e informações detalhadas
 */
function detectErrorType(errorMessage, stackTrace = '') {
  const allText = (errorMessage + ' ' + stackTrace).toLowerCase();
  
  // Timeout
  const timeoutInfo = detectTimeoutInfo(errorMessage);
  if (timeoutInfo) {
    return {
      type: 'timeout',
      ...timeoutInfo,
      suggestion: 'Verifique se o elemento está visível e acessível. Considere aumentar o timeout se necessário.'
    };
  }
  
  // Elemento não encontrado
  if (allText.includes('element not found') || 
      allText.includes('no such element') ||
      allText.includes('elemento não encontrado') ||
      allText.includes('locator not found')) {
    const locatorMatch = errorMessage.match(/(Locator\([^)]+\)|"[^"]+"|'[^']+'|\.\w+)/i);
    return {
      type: 'element_not_found',
      locator: locatorMatch ? locatorMatch[1] : null,
      suggestion: 'Verifique se o seletor do elemento está correto e se a página carregou completamente.'
    };
  }
  
  // Assertion falhou
  if (allText.includes('assertion') || 
      allText.includes('assert') ||
      allText.includes('expected') && allText.includes('but was') ||
      allText.includes('esperado') && allText.includes('mas foi')) {
    return {
      type: 'assertion_failed',
      suggestion: 'O valor esperado não corresponde ao valor recebido. Verifique os dados e a lógica do teste.'
    };
  }
  
  // Null Reference
  if (allText.includes('nullreferenceexception') || 
      allText.includes('null reference') ||
      allText.includes('object reference not set')) {
    return {
      type: 'null_reference',
      suggestion: 'Um objeto não foi inicializado. Verifique se todos os objetos foram criados antes de uso.'
    };
  }
  
  // Connection/Network error
  if (allText.includes('connection') || 
      allText.includes('network') ||
      allText.includes('timeout') && allText.includes('connection') ||
      allText.includes('econnrefused')) {
    return {
      type: 'connection_error',
      suggestion: 'Erro de conexão com o servidor. Verifique se a aplicação está rodando e acessível.'
    };
  }
  
  // Chrome/Playwright error
  if (allText.includes('playwright') || 
      allText.includes('browser') && allText.includes('closed') ||
      allText.includes('target closed')) {
    return {
      type: 'browser_error',
      suggestion: 'Erro relacionado ao navegador. Verifique se o Chrome/Playwright está instalado corretamente.'
    };
  }
  
  // Exception genérica
  const exceptionMatch = errorMessage.match(/^(System\.)?([\w.]+Exception)/i);
  if (exceptionMatch) {
    return {
      type: 'exception',
      exceptionType: exceptionMatch[2],
      suggestion: `Exceção do tipo ${exceptionMatch[2]}. Verifique o stack trace para mais detalhes.`
    };
  }
  
  // Tipo desconhecido
  return {
    type: 'unknown',
    suggestion: 'Erro não categorizado. Verifique os logs completos para mais detalhes.'
  };
}

/**
 * Extrai cenários individuais do output do dotnet test/Reqnroll com informações detalhadas
 * @param {string} output - Output completo da execução
 * @returns {Array<Object>} Array de cenários com nome, status, erros, stack trace, BDD steps, etc.
 */
function extractScenarios(output) {
  const scenarios = [];
  const lines = output.split('\n');
  
  // Mapa de cenários por nome completo do teste
  const scenariosByTestName = new Map();
  
  // Estado durante parsing
  let currentScenario = null;
  let currentTestName = null;
  let collectingError = false;
  let collectingStackTrace = false;
  let collectingOutput = false;
  let errorMessage = '';
  let stackTrace = '';
  let standardOutput = '';
  let bddSteps = [];
  let inFeature = false;
  let featureName = '';
  
  for (let i = 0; i < lines.length; i++) {
    const line = lines[i];
    const trimmedLine = line.trim();
    const originalLine = line; // Mantém espaços originais para output
    
    // Detecta Feature
    const featureMatch = trimmedLine.match(/Feature:\s*(.+)/i);
    if (featureMatch) {
      featureName = featureMatch[1].trim();
      inFeature = true;
      continue;
    }
    
    // Detecta Feature a partir do arquivo executado (formato do automacao_exe)
    // Padrão: "📄 Arquivo executado: path/to/Feature.feature"
    const fileExecutedMatch = trimmedLine.match(/📄\s*Arquivo executado:\s*(.+\.feature)/i);
    if (fileExecutedMatch) {
      const filePath = fileExecutedMatch[1].trim();
      // Extrai nome da feature do caminho do arquivo
      const fileName = filePath.split(/[\\/]/).pop();
      featureName = fileName.replace(/\.feature$/i, '');
      inFeature = true;
      continue;
    }
    
    // Detecta início de cenário (Reqnroll/SpecFlow)
    const scenarioMatch = trimmedLine.match(/Scenario(?: Outline)?:?\s*(.+)/i);
    if (scenarioMatch) {
      // Finaliza cenário anterior se existir
      if (currentScenario) {
        scenarios.push(currentScenario);
      }
      
      currentScenario = {
        name: scenarioMatch[1].trim(),
        status: 'unknown',
        featureName: featureName,
        fullTestName: null,
        errorMessage: '',
        stackTrace: '',
        standardOutput: '',
        bddSteps: [],
        lineNumber: i,
        duration: null
      };
      currentTestName = null;
      errorMessage = '';
      stackTrace = '';
      standardOutput = '';
      bddSteps = [];
      collectingError = false;
      collectingStackTrace = false;
      collectingOutput = false;
      continue;
    }
    
    // Detecta resultado de teste (NUnit/dotnet test)
    // Padrões: "Passed TestName", "Failed TestName", "Passed  TestName (XXms)"
    const passedMatch = trimmedLine.match(/Passed\s+(.+?)(?:\s*\(([\d.]+)\s*(ms|s)\))?$/i);
    if (passedMatch) {
      const testName = passedMatch[1].trim();
      const durationStr = passedMatch[2];
      const durationUnit = passedMatch[3];
      
      // Extrai nome do cenário do nome do teste
      // Formato: FeatureName.ScenarioName ou ScenarioName
      const parts = testName.split('.');
      const scenarioName = parts.length > 1 ? parts[parts.length - 1] : testName;
      
      // Procura cenário existente ou cria novo
      let scenario = scenarios.find(s => 
        s.fullTestName === testName || 
        (s.name && s.name.toLowerCase() === scenarioName.toLowerCase()) ||
        testName.toLowerCase().includes(s.name?.toLowerCase() || '')
      );
      
      if (!scenario && currentScenario) {
        scenario = currentScenario;
        scenario.fullTestName = testName;
        if (!scenario.name) {
          scenario.name = scenarioName;
        }
      }
      
      if (!scenario) {
        scenario = {
          name: scenarioName,
          status: 'passed',
          featureName: featureName,
          fullTestName: testName,
          errorMessage: '',
          stackTrace: '',
          standardOutput: '',
          bddSteps: [],
          lineNumber: i,
          duration: durationStr ? parseFloat(durationStr) : null
        };
        scenarios.push(scenario);
      } else {
        scenario.status = 'passed';
        scenario.fullTestName = testName;
        if (durationStr) {
          scenario.duration = parseFloat(durationStr);
        }
      }
      
      scenariosByTestName.set(testName, scenario);
      currentScenario = scenario;
      currentTestName = testName;
      collectingError = false;
      collectingStackTrace = false;
      collectingOutput = false;
      continue;
    }
    
    const failedMatch = trimmedLine.match(/Failed\s+(.+?)(?:\s*\(([\d.]+)\s*(ms|s)\))?$/i);
    if (failedMatch) {
      const testName = failedMatch[1].trim();
      const durationStr = failedMatch[2];
      
      const parts = testName.split('.');
      const scenarioName = parts.length > 1 ? parts[parts.length - 1] : testName;
      
      let scenario = scenarios.find(s => 
        s.fullTestName === testName || 
        (s.name && s.name.toLowerCase() === scenarioName.toLowerCase()) ||
        testName.toLowerCase().includes(s.name?.toLowerCase() || '')
      );
      
      if (!scenario && currentScenario) {
        scenario = currentScenario;
        scenario.fullTestName = testName;
        if (!scenario.name) {
          scenario.name = scenarioName;
        }
      }
      
      if (!scenario) {
        scenario = {
          name: scenarioName,
          status: 'failed',
          featureName: featureName,
          fullTestName: testName,
          errorMessage: '',
          stackTrace: '',
          standardOutput: '',
          bddSteps: [],
          lineNumber: i,
          duration: durationStr ? parseFloat(durationStr) : null
        };
        scenarios.push(scenario);
      } else {
        scenario.status = 'failed';
        scenario.fullTestName = testName;
        if (durationStr) {
          scenario.duration = parseFloat(durationStr);
        }
      }
      
      scenariosByTestName.set(testName, scenario);
      currentScenario = scenario;
      currentTestName = testName;
      // Quando um teste falha, começa a coletar erro imediatamente
      // A próxima linha geralmente contém informações de erro
      collectingError = true;
      collectingStackTrace = false;
      collectingOutput = false;
      errorMessage = '';
      stackTrace = '';
      continue;
    }
    
    // Detecta mensagem de erro (ex: "Mensagem: System.NullReferenceException...")
    // Pode aparecer sem "Mensagem:" antes, apenas a exceção
    if (trimmedLine.match(/^Mensagem:\s*/i) || trimmedLine.match(/^Message:\s*/i)) {
      if (currentScenario) {
        collectingError = true;
        collectingStackTrace = false;
        collectingOutput = false;
        errorMessage = trimmedLine.replace(/^Mensagem:\s*/i, '').replace(/^Message:\s*/i, '').trim();
        if (currentScenario.errorMessage) {
          currentScenario.errorMessage += '\n' + errorMessage;
        } else {
          currentScenario.errorMessage = errorMessage;
        }
      }
      continue;
    }
    
    // Detecta exceções diretamente (System.Exception, System.NullReferenceException, etc.)
    if (!collectingError && !collectingStackTrace && !collectingOutput && currentScenario && currentScenario.status === 'failed') {
      const exceptionMatch = trimmedLine.match(/^(System\.[\w.]+Exception)\s*[:\s]*(.+)?/i);
      if (exceptionMatch && !currentScenario.errorMessage) {
        collectingError = true;
        errorMessage = trimmedLine;
        currentScenario.errorMessage = errorMessage;
        continue;
      }
    }
    
    // Detecta início de stack trace
    if (trimmedLine.match(/^Rastreamento de Pilha:/i) || trimmedLine.match(/^Stack Trace:/i) || trimmedLine.match(/^StackTrace:/i)) {
      collectingError = false;
      collectingStackTrace = true;
      collectingOutput = false;
      if (currentScenario) {
        stackTrace = '';
      }
      continue;
    }
    
    // Detecta início de output padrão
    if (trimmedLine.match(/^Saída Padrão:/i) || trimmedLine.match(/^Standard Output:/i) || trimmedLine.match(/^StandardOutput:/i)) {
      collectingError = false;
      collectingStackTrace = false;
      collectingOutput = true;
      if (currentScenario) {
        standardOutput = '';
      }
      continue;
    }
    
    // Coleta mensagem de erro (continua até encontrar stack trace ou output padrão)
    if (collectingError && currentScenario && trimmedLine) {
      // Para de coletar erro quando encontrar stack trace ou output padrão
      if (trimmedLine.match(/^Rastreamento de Pilha:/i) || trimmedLine.match(/^Stack Trace:/i) || 
          trimmedLine.match(/^StackTrace:/i) || trimmedLine.match(/^Saída Padrão:/i) || 
          trimmedLine.match(/^Standard Output:/i) || trimmedLine.match(/^StandardOutput:/i)) {
        collectingError = false;
        if (trimmedLine.match(/^Rastreamento/i) || trimmedLine.match(/^Stack/i)) {
          collectingStackTrace = true;
          stackTrace = '';
        } else {
          collectingOutput = true;
          standardOutput = '';
        }
      } else if (trimmedLine.match(/^Passed\s+/i) || trimmedLine.match(/^Failed\s+/i) || 
                 trimmedLine.match(/^Scenario:/i)) {
        // Novo teste encontrado, para de coletar erro deste
        collectingError = false;
        collectingStackTrace = false;
        collectingOutput = false;
      } else {
        // Adiciona linha ao erro
        // Remove prefixo "Mensagem:" ou "Message:" se existir
        let lineToAdd = trimmedLine.replace(/^Mensagem:\s*/i, '').replace(/^Message:\s*/i, '');
        
        // Captura informações de timeout dos logs (ex: "waiting for Locator(...)")
        // Essas informações são importantes e devem ser incluídas na mensagem de erro
        if (lineToAdd.includes('waiting for') || 
            lineToAdd.includes('Locator') ||
            lineToAdd.includes('timeout') ||
            lineToAdd.includes('exceeded')) {
          // Esta linha contém informações importantes sobre o timeout
          if (lineToAdd.trim()) {
            if (currentScenario.errorMessage) {
              currentScenario.errorMessage += '\n' + lineToAdd.trim();
            } else {
              currentScenario.errorMessage = lineToAdd.trim();
            }
          }
        } else if (lineToAdd.trim()) {
          // Linha normal de erro
          if (currentScenario.errorMessage) {
            currentScenario.errorMessage += '\n' + lineToAdd.trim();
          } else {
            currentScenario.errorMessage = lineToAdd.trim();
          }
        }
      }
    }
    
    // Coleta stack trace (continua até encontrar output padrão)
    if (collectingStackTrace && currentScenario) {
      // Para de coletar stack trace quando encontrar output padrão ou próximo teste
      if (trimmedLine.match(/^Saída/i) || trimmedLine.match(/^Standard/i) || 
          trimmedLine.match(/^Passed\s+/i) || trimmedLine.match(/^Failed\s+/i) ||
          trimmedLine.match(/^Scenario:/i)) {
        collectingStackTrace = false;
        if (trimmedLine.match(/^Saída/i) || trimmedLine.match(/^Standard/i)) {
          collectingOutput = true;
          standardOutput = '';
        }
      } else if (trimmedLine.trim()) {
        // Adiciona linha ao stack trace
        if (currentScenario.stackTrace) {
          currentScenario.stackTrace += '\n' + trimmedLine;
        } else {
          currentScenario.stackTrace = trimmedLine;
        }
      }
    }
    
    // Detecta BDD steps mesmo fora do bloco de output padrão
    // Se não está coletando output mas encontrou um cenário, tenta detectar steps BDD
    if (!collectingOutput && !collectingError && !collectingStackTrace && currentScenario) {
      // Detecta linhas que contêm padrões de BDD steps
      const stepMatch = trimmedLine.match(/^(Given|When|Then|And|But)\s+(.+?)(?:\s*->\s*(.+))?$/i);
      if (stepMatch) {
        const stepKeyword = stepMatch[1];
        const stepText = stepMatch[2].trim();
        const stepResult = stepMatch[3] || '';
        
        // Traduz para português
        const translatedStep = translateBDDStep(`${stepKeyword} ${stepText}`);
        
        // Determina status do step baseado no resultado
        let stepStatus = 'passed';
        if (stepResult) {
          if (stepResult.toLowerCase().includes('error') || stepResult.toLowerCase().includes('failed')) {
            stepStatus = 'failed';
          } else if (stepResult.toLowerCase().includes('skipped')) {
            stepStatus = 'skipped';
          }
        }
        
        const newStep = {
          keyword: stepKeyword,
          text: stepText,
          translated: translatedStep,
          result: stepResult.trim(),
          status: stepStatus,
          fullLine: translatedStep + (stepResult ? ` -> ${stepResult}` : ''),
          lineNumber: i
        };
        
        if (currentScenario.bddSteps) {
          currentScenario.bddSteps.push(newStep);
        } else {
          currentScenario.bddSteps = [newStep];
        }
        
        // Se este step falhou, marca como o step onde falhou
        if (stepStatus === 'failed' && !currentScenario.failedStep) {
          currentScenario.failedStep = newStep;
          currentScenario.failedStepIndex = currentScenario.bddSteps.length - 1;
        }
        
        continue;
      }
    }
    
    // Coleta output padrão (BDD steps)
    if (collectingOutput && currentScenario) {
      // Detecta BDD steps (Given/When/Then/And) com padrões mais robustos
      // Padrões: "Given que preciso...", "-> done: ...", "-> error: ...", "-> skipped because of previous errors"
      // Também detecta linhas que começam com "->" que podem ser continuação de steps
      const stepMatch = trimmedLine.match(/^(Given|When|Then|And|But)\s+(.+?)(?:\s*->\s*(.+))?$/i);
      if (stepMatch) {
        const stepKeyword = stepMatch[1];
        const stepText = stepMatch[2].trim();
        const stepResult = stepMatch[3] || '';
        
        // Traduz para português
        const translatedStep = translateBDDStep(`${stepKeyword} ${stepText}`);
        
        // Determina status do step baseado no resultado
        let stepStatus = 'passed';
        if (stepResult) {
          if (stepResult.toLowerCase().includes('error') || stepResult.toLowerCase().includes('failed')) {
            stepStatus = 'failed';
          } else if (stepResult.toLowerCase().includes('skipped')) {
            stepStatus = 'skipped';
          }
        }
        
        const newStep = {
          keyword: stepKeyword,
          text: stepText,
          translated: translatedStep,
          result: stepResult.trim(),
          status: stepStatus,
          fullLine: translatedStep + (stepResult ? ` -> ${stepResult}` : ''),
          lineNumber: i
        };
        
        bddSteps.push(newStep);
        
        if (currentScenario.bddSteps) {
          currentScenario.bddSteps.push(newStep);
        } else {
          currentScenario.bddSteps = [newStep];
        }
        
        // Se este step falhou, marca como o step onde falhou
        if (stepStatus === 'failed' && !currentScenario.failedStep) {
          currentScenario.failedStep = newStep;
          currentScenario.failedStepIndex = currentScenario.bddSteps.length - 1;
        }
      } else if (trimmedLine.match(/^->\s*(done|error|skipped)/i)) {
        // Linhas que começam com "-> done:", "-> error:" ou "-> skipped" mas não têm step keyword
        // Adiciona ao último step se existir
        if (currentScenario.bddSteps && currentScenario.bddSteps.length > 0) {
          const lastStep = currentScenario.bddSteps[currentScenario.bddSteps.length - 1];
          if (lastStep) {
            // Atualiza o resultado do último step
            const resultText = trimmedLine.replace(/^->\s*/, '').trim();
            lastStep.result = resultText;
            
            // Atualiza status baseado no resultado
            if (resultText.toLowerCase().includes('error') || resultText.toLowerCase().includes('failed')) {
              lastStep.status = 'failed';
              // Se este step falhou, marca como o step onde falhou
              if (!currentScenario.failedStep) {
                currentScenario.failedStep = lastStep;
                currentScenario.failedStepIndex = currentScenario.bddSteps.length - 1;
              }
            } else if (resultText.toLowerCase().includes('skipped')) {
              lastStep.status = 'skipped';
            } else if (resultText.toLowerCase().includes('done')) {
              lastStep.status = 'passed';
            }
            
            lastStep.fullLine = lastStep.translated + ` -> ${lastStep.result}`;
          }
        } else if (trimmedLine) {
          // Se não há steps ainda, adiciona ao output padrão
          if (currentScenario.standardOutput) {
            currentScenario.standardOutput += '\n' + trimmedLine;
          } else {
            currentScenario.standardOutput = trimmedLine;
          }
        }
      } else if (trimmedLine) {
        // Adiciona outras linhas do output
        // Mas verifica se é informação adicional sobre o último step (ex: timeout details)
        if (currentScenario.bddSteps && currentScenario.bddSteps.length > 0) {
          const lastStep = currentScenario.bddSteps[currentScenario.bddSteps.length - 1];
          // Se o último step falhou ou esta linha contém informações de erro (ex: timeout, locator)
          // Adiciona como informação adicional do step
          if (lastStep.status === 'failed' || 
              trimmedLine.includes('timeout') || 
              trimmedLine.includes('waiting for') || 
              trimmedLine.includes('Locator') ||
              trimmedLine.includes('Exception') ||
              trimmedLine.includes('===========================') ||
              trimmedLine.match(/^=+\s*logs\s*=+/i)) {
            // Adiciona como informação adicional do step
            if (!lastStep.errorDetails) {
              lastStep.errorDetails = [];
            }
            lastStep.errorDetails.push(trimmedLine);
            
            // Se contém informações de timeout, também adiciona à mensagem de erro do cenário
            if (trimmedLine.includes('timeout') || trimmedLine.includes('waiting for')) {
              if (!currentScenario.errorMessage) {
                currentScenario.errorMessage = '';
              }
              if (!currentScenario.errorMessage.includes(trimmedLine)) {
                currentScenario.errorMessage += (currentScenario.errorMessage ? '\n' : '') + trimmedLine;
              }
            }
          } else {
            // Adiciona ao output padrão
            if (currentScenario.standardOutput) {
              currentScenario.standardOutput += '\n' + trimmedLine;
            } else {
              currentScenario.standardOutput = trimmedLine;
            }
          }
        } else {
          // Se não há steps, verifica se é informação de erro importante
          if (trimmedLine.includes('timeout') || 
              trimmedLine.includes('waiting for') || 
              trimmedLine.includes('Locator') ||
              trimmedLine.includes('Exception')) {
            // Adiciona à mensagem de erro do cenário se não existir
            if (!currentScenario.errorMessage) {
              currentScenario.errorMessage = '';
            }
            if (!currentScenario.errorMessage.includes(trimmedLine)) {
              currentScenario.errorMessage += (currentScenario.errorMessage ? '\n' : '') + trimmedLine;
            }
          } else {
            // Adiciona ao output padrão
            if (currentScenario.standardOutput) {
              currentScenario.standardOutput += '\n' + trimmedLine;
            } else {
              currentScenario.standardOutput = trimmedLine;
            }
          }
        }
      }
    }
    
    // Detecta padrões de falha sem "Failed TestName" explícito
    // Padrões: "Com falha: 1", "Falha na Execução", "Total de testes: X, Com falha: Y"
    const failurePatternMatch = trimmedLine.match(/(?:Com\s+falha|Falha\s+na\s+Execução|Total\s+de\s+testes.*Com\s+falha)[:\s]+(\d+)/i);
    if (failurePatternMatch) {
      const failedCount = parseInt(failurePatternMatch[1]);
      if (failedCount > 0) {
        // Se há um cenário atual, marca como falhou
        if (currentScenario && currentScenario.status !== 'failed') {
          currentScenario.status = 'failed';
          collectingError = true;
        } else if (!currentScenario) {
          // Cria um novo cenário baseado no Feature/Scenario do início do log
          const scenarioNameFromLog = output.match(/Cenário:\s*(.+)/i) || 
                                     output.match(/Scenario:\s*(.+)/i);
          const scenarioName = scenarioNameFromLog ? scenarioNameFromLog[1].trim() : 'Cenário não identificado';
          
          currentScenario = {
            name: scenarioName,
            status: 'failed',
            featureName: featureName,
            fullTestName: null,
            errorMessage: '',
            stackTrace: '',
            standardOutput: '',
            bddSteps: [],
            lineNumber: i,
            duration: null
          };
          scenarios.push(currentScenario);
          collectingError = true;
        }
      }
    }
    
    // Detecta "Falha na Execução de Teste" sem número
    // IMPORTANTE: Ignora se for a mensagem de resumo (que contém "📋" ou "RESULTADO DOS CENÁRIOS")
    // Essa mensagem é gerada DEPOIS da execução e não deve criar um novo cenário de falha
    const isSummaryMessage = trimmedLine.includes('📋') || 
                            trimmedLine.includes('RESULTADO DOS CENÁRIOS') ||
                            trimmedLine.includes('RESULTADO') ||
                            trimmedLine.match(/^=+\s*$/); // Linha só com "===="
    
    if (!isSummaryMessage && (trimmedLine.match(/Falha\s+na\s+Execução\s+de\s+Teste/i) || trimmedLine.match(/Falha\s+na\s+Execução/i))) {
      // Só cria cenário de falha se não houver cenários já extraídos
      // Se já há cenários, significa que a execução foi detectada corretamente
      const hasExtractedScenarios = scenarios.length > 0;
      
      if (!hasExtractedScenarios) {
        if (currentScenario && currentScenario.status !== 'failed') {
          currentScenario.status = 'failed';
          collectingError = true;
        } else if (!currentScenario) {
          const scenarioNameFromLog = output.match(/Cenário:\s*(.+)/i) || 
                                     output.match(/Scenario:\s*(.+)/i);
          const scenarioName = scenarioNameFromLog ? scenarioNameFromLog[1].trim() : 'Cenário não identificado';
          
          currentScenario = {
            name: scenarioName,
            status: 'failed',
            featureName: featureName,
            fullTestName: null,
            errorMessage: '',
            stackTrace: '',
            standardOutput: '',
            bddSteps: [],
            lineNumber: i,
            duration: null
          };
          scenarios.push(currentScenario);
          collectingError = true;
        }
      }
    }
    
    // Extrai erros dos logs do Playwright (pw:api)
    // Captura linhas que indicam timeout, waiting for, locator not found
    // IMPORTANTE: Não marca como erro se for apenas um log informativo (sem "exceeded", "not found", "Exception", etc.)
    if (trimmedLine.includes('pw:api')) {
      // Só marca como erro se for realmente um erro, não apenas um log informativo
      // Logs informativos como "waiting for Locator" são normais durante a execução
      const isRealError = trimmedLine.includes('timeout exceeded') || 
                         trimmedLine.includes('exceeded') ||
                         trimmedLine.includes('not found') ||
                         trimmedLine.includes('Exception') ||
                         trimmedLine.includes('Error:') ||
                         trimmedLine.includes('failed') ||
                         (trimmedLine.includes('waiting for') && trimmedLine.includes('exceeded'));
      
      const isErrorLine = isRealError;
      
      if (isErrorLine) {
        // Se há um cenário atual, adiciona à mensagem de erro
        if (currentScenario) {
          // Se o cenário já falhou ou está em estado de erro, adiciona à mensagem de erro
          if (currentScenario.status === 'failed' || collectingError) {
            if (!currentScenario.errorMessage) {
              currentScenario.errorMessage = '';
            }
            // Remove timestamp do início da linha se existir
            const cleanLine = trimmedLine.replace(/^\d{4}-\d{2}-\d{2}T[\d:.Z]+\s+/, '').replace(/^pw:api\s+/, '').trim();
            if (cleanLine && !currentScenario.errorMessage.includes(cleanLine)) {
              currentScenario.errorMessage += (currentScenario.errorMessage ? '\n' : '') + cleanLine;
            }
          }
          // Se contém timeout ou erro crítico, marca o cenário como falhou
          if (trimmedLine.includes('timeout') && trimmedLine.includes('exceeded')) {
            if (currentScenario.status !== 'failed') {
              currentScenario.status = 'failed';
              collectingError = true;
            }
          }
        } else {
          // Se não há cenário mas encontrou erro crítico, cria um
          // IMPORTANTE: Só cria se realmente não houver cenários já extraídos
          // Se já há cenários, significa que a execução foi detectada e este é apenas um log informativo
          const hasExtractedScenarios = scenarios.length > 0;
          
          if (!hasExtractedScenarios) {
            const scenarioNameFromLog = output.match(/Cenário:\s*(.+)/i) || 
                                       output.match(/Scenario:\s*(.+)/i);
            const scenarioName = scenarioNameFromLog ? scenarioNameFromLog[1].trim() : 'Cenário não identificado';
            
            const cleanLine = trimmedLine.replace(/^\d{4}-\d{2}-\d{2}T[\d:.Z]+\s+/, '').replace(/^pw:api\s+/, '').trim();
            
            currentScenario = {
              name: scenarioName,
              status: 'failed',
              featureName: featureName,
              fullTestName: null,
              errorMessage: cleanLine,
              stackTrace: '',
              standardOutput: '',
              bddSteps: [],
              lineNumber: i,
              duration: null
            };
            scenarios.push(currentScenario);
            collectingError = true;
          }
        }
      }
    }
    
    // Detecta stack traces sem cabeçalho
    // Padrões: linhas que começam com espaços seguidos de "at" ou contêm nomes de arquivos .cs
    if (!collectingStackTrace && !collectingOutput) {
      const isStackTraceLine = (trimmedLine.match(/^\s+at\s+/) || 
                                trimmedLine.match(/\.cs:\d+/) ||
                                trimmedLine.match(/^\s+em\s+/) ||
                                (trimmedLine.match(/^\s+/) && trimmedLine.match(/\([^)]+\)/)) ||
                                trimmedLine.match(/^\s+at\s+[\w.]+\(/) ||
                                trimmedLine.match(/^\s+---\s+End\s+of\s+stack\s+trace/i));
      
      if (isStackTraceLine) {
        if (currentScenario) {
          if (currentScenario.status === 'failed' || collectingError) {
            // Se não está coletando stack trace mas encontrou uma linha de stack trace
            if (!currentScenario.stackTrace) {
              currentScenario.stackTrace = '';
            }
            currentScenario.stackTrace += (currentScenario.stackTrace ? '\n' : '') + trimmedLine;
            collectingStackTrace = true;
            collectingError = false;
            continue;
          }
        } else {
          // Se não há cenário mas encontrou stack trace, cria um cenário falhado
          const scenarioNameFromLog = output.match(/Cenário:\s*(.+)/i) || 
                                     output.match(/Scenario:\s*(.+)/i);
          const scenarioName = scenarioNameFromLog ? scenarioNameFromLog[1].trim() : 'Cenário não identificado';
          
          currentScenario = {
            name: scenarioName,
            status: 'failed',
            featureName: featureName,
            fullTestName: null,
            errorMessage: '',
            stackTrace: trimmedLine,
            standardOutput: '',
            bddSteps: [],
            lineNumber: i,
            duration: null
          };
          scenarios.push(currentScenario);
          collectingStackTrace = true;
          collectingError = false;
          continue;
        }
      }
    }
    
    // Detecta exceções em qualquer lugar do output (mesmo sem bloco estruturado)
    // Também detecta TimeoutException, NullReferenceException, etc.
    if (!collectingError && !collectingStackTrace && !collectingOutput) {
      const exceptionMatch = trimmedLine.match(/^(System\.[\w.]+Exception|[\w.]+Exception|TimeoutException|NullReferenceException|InvalidOperationException|ArgumentException)\s*[:\s]*(.+)?/i);
      if (exceptionMatch) {
        // Se há um cenário atual, adiciona a exceção
        if (currentScenario) {
          if (!currentScenario.errorMessage) {
            currentScenario.errorMessage = '';
          }
          if (!currentScenario.errorMessage.includes(trimmedLine)) {
            currentScenario.errorMessage += (currentScenario.errorMessage ? '\n' : '') + trimmedLine;
            currentScenario.status = 'failed';
            collectingError = true;
          }
        } else {
          // Cria um novo cenário para a exceção
          const scenarioNameFromLog = output.match(/Cenário:\s*(.+)/i) || 
                                     output.match(/Scenario:\s*(.+)/i);
          const scenarioName = scenarioNameFromLog ? scenarioNameFromLog[1].trim() : 'Cenário não identificado';
          
          currentScenario = {
            name: scenarioName,
            status: 'failed',
            featureName: featureName,
            fullTestName: null,
            errorMessage: trimmedLine,
            stackTrace: '',
            standardOutput: '',
            bddSteps: [],
            lineNumber: i,
            duration: null
          };
          scenarios.push(currentScenario);
          collectingError = true;
        }
        continue;
      }
      
      // Detecta mensagens de timeout do Playwright mesmo sem Exception explícita
      const timeoutMatch = trimmedLine.match(/Timeout\s+(\d+)\s*(ms|s|milliseconds?|seconds?)\s+exceeded/i);
      if (timeoutMatch && !currentScenario?.errorMessage?.includes('Timeout')) {
        if (currentScenario) {
          if (!currentScenario.errorMessage) {
            currentScenario.errorMessage = '';
          }
          if (!currentScenario.errorMessage.includes(trimmedLine)) {
            currentScenario.errorMessage += (currentScenario.errorMessage ? '\n' : '') + trimmedLine;
            currentScenario.status = 'failed';
            collectingError = true;
          }
        } else {
          const scenarioNameFromLog = output.match(/Cenário:\s*(.+)/i) || 
                                     output.match(/Scenario:\s*(.+)/i);
          const scenarioName = scenarioNameFromLog ? scenarioNameFromLog[1].trim() : 'Cenário não identificado';
          
          currentScenario = {
            name: scenarioName,
            status: 'failed',
            featureName: featureName,
            fullTestName: null,
            errorMessage: trimmedLine,
            stackTrace: '',
            standardOutput: '',
            bddSteps: [],
            lineNumber: i,
            duration: null
          };
          scenarios.push(currentScenario);
          collectingError = true;
        }
      }
    }
    
    // Detecta linhas de erro que não estão em blocos estruturados
    // Isso captura erros que aparecem após um teste falhar, mas antes dos blocos estruturados
    if (!collectingError && !collectingStackTrace && !collectingOutput && currentScenario && currentScenario.status === 'failed') {
      // Se acabou de encontrar um "Failed", a próxima linha pode ser erro
      if (i > 0 && lines[i - 1].trim().match(/^Failed\s+/i)) {
        // Próxima linha após "Failed" pode ser mensagem de erro ou exceção
        if (trimmedLine.match(/^System\.[\w.]+Exception/i) || 
            trimmedLine.match(/Exception/i) && !currentScenario.errorMessage) {
          collectingError = true;
          currentScenario.errorMessage = trimmedLine;
          continue;
        }
      }
      
      // Procura por exceções e erros em geral
      if (trimmedLine.match(/Exception|Error|Failed/i) && !trimmedLine.match(/Passed|Success/i)) {
        // Evita adicionar se já está coletando em outro modo ou se já existe
        if (!currentScenario.errorMessage || !currentScenario.errorMessage.includes(trimmedLine)) {
          // Só adiciona se não está em um bloco estruturado
          if (!trimmedLine.match(/^Rastreamento/i) && !trimmedLine.match(/^Stack/i) && 
              !trimmedLine.match(/^Saída/i) && !trimmedLine.match(/^Standard/i)) {
            if (currentScenario.errorMessage) {
              currentScenario.errorMessage += '\n' + trimmedLine;
            } else {
              currentScenario.errorMessage = trimmedLine;
            }
          }
        }
      }
    }
  }
  
  // Adiciona o cenário atual se existir
  if (currentScenario && !scenarios.find(s => s === currentScenario)) {
    scenarios.push(currentScenario);
  }
  
  // Limpa espaços em branco excessivos das mensagens de erro e stack traces
  // E adiciona informações detalhadas de erro
  scenarios.forEach(scenario => {
    if (scenario.errorMessage) {
      scenario.errorMessage = scenario.errorMessage.replace(/\n{3,}/g, '\n\n').trim();
    }
    if (scenario.stackTrace) {
      scenario.stackTrace = scenario.stackTrace.replace(/\n{3,}/g, '\n\n').trim();
    }
    if (scenario.standardOutput) {
      scenario.standardOutput = scenario.standardOutput.replace(/\n{3,}/g, '\n\n').trim();
    }
    
    // Detecta tipo de erro e adiciona informações detalhadas
    if (scenario.status === 'failed' && scenario.errorMessage) {
      const errorInfo = detectErrorType(scenario.errorMessage, scenario.stackTrace || '');
      scenario.errorType = errorInfo.type;
      scenario.errorDetails = errorInfo;
      
      // Se há um step que falhou, adiciona informações de erro ao step também
      if (scenario.failedStep) {
        scenario.failedStep.errorType = errorInfo.type;
        scenario.failedStep.errorDetails = errorInfo;
      }
    }
    
    // Marca steps subsequentes como skipped se houve falha
    if (scenario.status === 'failed' && scenario.failedStepIndex !== undefined && scenario.bddSteps) {
      for (let i = scenario.failedStepIndex + 1; i < scenario.bddSteps.length; i++) {
        if (scenario.bddSteps[i].status === 'passed') {
          scenario.bddSteps[i].status = 'skipped';
          scenario.bddSteps[i].result = 'skipped because of previous errors';
          scenario.bddSteps[i].fullLine = scenario.bddSteps[i].translated + ' -> skipped because of previous errors';
        }
      }
    }
  });
  
  // Filtra cenários de cleanup/teardown e métodos auxiliares
  // Remove cenários que são apenas métodos de cleanup (Errors, Cleanup, etc.)
  // ou que não têm nome válido ou são apenas métodos auxiliares
  const cleanupPatterns = [
    /^Errors\(\)$/i,
    /^Cleanup\(\)$/i,
    /^Teardown\(\)$/i,
    /^Setup\(\)$/i,
    /^BeforeScenario\(\)$/i,
    /^AfterScenario\(\)$/i,
    /^BeforeFeature\(\)$/i,
    /^AfterFeature\(\)$/i,
    /^BeforeTestRun\(\)$/i,
    /^AfterTestRun\(\)$/i,
    /^Initialize\(\)$/i,
    /^Finalize\(\)$/i,
    /^Dispose\(\)$/i,
    /^.*Cleanup$/i,
    /^.*Teardown$/i,
    /^.*Error$/i,
    /^.*Errors$/i
  ];
  
  // Filtra cenários que são métodos de cleanup ou têm nome inválido
  let realScenarios = scenarios.filter(scenario => {
    // Se não tem nome, remove
    if (!scenario.name || scenario.name.trim() === '') {
      return false;
    }
    
    // Remove se é um método de cleanup/teardown
    const isCleanup = cleanupPatterns.some(pattern => pattern.test(scenario.name));
    if (isCleanup) {
      return false;
    }
    
    // Remove se o nome do teste completo contém apenas o nome do método (sem contexto de cenário)
    // Cenários reais geralmente têm nomes descritivos, não apenas nomes de métodos
    if (scenario.fullTestName) {
      // Se o fullTestName é muito similar ao name e parece ser apenas um método, remove
      const nameLower = scenario.name.toLowerCase();
      const fullTestNameLower = scenario.fullTestName.toLowerCase();
      
      // Se o fullTestName termina com .name() ou é apenas o name, pode ser um método
      if (fullTestNameLower.endsWith('.' + nameLower) || fullTestNameLower === nameLower) {
        // Verifica se parece ser um método (tem parênteses no nome)
        if (nameLower.includes('()') || nameLower.match(/^[a-z][a-z0-9]*\(\)$/i)) {
          return false;
        }
      }
    }
    
    // Mantém o cenário se passou em todos os filtros
    return true;
  });
  
  // Se não encontrou cenários reais mas há resultados de teste, tenta extrair do resumo
  // Mas apenas se realmente não encontrou nenhum cenário válido
  if (realScenarios.length === 0) {
    // Tenta extrair do resumo final do log (formato do automacao_exe)
    // Padrão: "🎯 Cenário executado: Nome do Cenário"
    const scenarioExecutedMatch = output.match(/🎯\s*Cenário executado:\s*(.+)/i);
    if (scenarioExecutedMatch) {
      const scenarioName = scenarioExecutedMatch[1].trim();
      
      // Tenta extrair duração do resumo
      // Padrão: "Duração: 1m 1s" ou "Duração: 61s" ou "Duração: 1 min 1 seg"
      let duration = null;
      // Primeiro tenta formato "Xm Ys" ou "X min Y seg"
      const durationMatchMinutes = output.match(/Duração:\s*(\d+)\s*(?:m|min)(?:\s+(\d+)\s*(?:s|seg))?/i);
      if (durationMatchMinutes) {
        const minutes = parseInt(durationMatchMinutes[1]);
        const seconds = durationMatchMinutes[2] ? parseInt(durationMatchMinutes[2]) : 0;
        duration = (minutes * 60 + seconds) * 1000; // Converte para ms
      } else {
        // Fallback: formato "Xs" ou "X seg"
        const durationMatchSeconds = output.match(/Duração:\s*(\d+)\s*(?:s|seg)/i);
        if (durationMatchSeconds) {
          const seconds = parseInt(durationMatchSeconds[1]);
          duration = seconds * 1000; // Converte segundos para ms
        }
      }
      
      // Determina status baseado no resumo
      // Padrão: "Status: ✅ Sucesso" ou "Status: ❌ Falha"
      let status = 'passed';
      if (output.match(/Status:\s*❌|Status:\s*Falha|Status:\s*Failed/i)) {
        status = 'failed';
      } else if (output.match(/Status:\s*✅|Status:\s*Sucesso|Status:\s*Success/i)) {
        status = 'passed';
      }
      
      // Extrai informações de erro dos logs do Playwright e exceções se houver falha
      let errorMessage = '';
      let stackTrace = '';
      
      if (status === 'failed') {
        const playwrightErrors = [];
        const exceptions = [];
        const stackTraceLines = [];
        
        const outputLines = output.split('\n');
        for (const line of outputLines) {
          const trimmed = line.trim();
          // Captura erros do Playwright
          if (trimmed.includes('pw:api') && (trimmed.includes('timeout') || trimmed.includes('waiting for') || trimmed.includes('exceeded') || trimmed.includes('not found'))) {
            const cleanLine = trimmed.replace(/^\d{4}-\d{2}-\d{2}T[\d:.Z]+/, '').trim();
            if (cleanLine && !playwrightErrors.includes(cleanLine)) {
              playwrightErrors.push(cleanLine);
            }
          }
          // Captura exceções
          if (trimmed.match(/^(System\.[\w.]+Exception|[\w.]+Exception)/i)) {
            if (!exceptions.includes(trimmed)) {
              exceptions.push(trimmed);
            }
          }
          // Captura stack traces (linhas com "at" ou ".cs:")
          if (trimmed.match(/^\s+at\s+/) || trimmed.match(/\.cs:\d+/) || trimmed.match(/^\s+em\s+/)) {
            if (!stackTraceLines.includes(trimmed)) {
              stackTraceLines.push(trimmed);
            }
          }
        }
        
        errorMessage = [...exceptions, ...playwrightErrors].join('\n');
        stackTrace = stackTraceLines.join('\n');
      }
      
      realScenarios.push({
        name: scenarioName,
        status: status,
        featureName: featureName,
        fullTestName: null,
        errorMessage: errorMessage,
        stackTrace: stackTrace,
        standardOutput: '',
        bddSteps: [],
        lineNumber: 0,
        duration: duration
      });
    } else {
      // Fallback: tenta extrair do resumo padrão do dotnet test
      const summaryMatch = output.match(/Passed:\s*(\d+).*Failed:\s*(\d+)/i);
      if (summaryMatch) {
        const passed = parseInt(summaryMatch[1]);
        const failed = parseInt(summaryMatch[2]);
        
        // Só cria cenários genéricos se houver pelo menos um teste executado
        // e não encontrou nenhum cenário real nos logs
        if (passed > 0 || failed > 0) {
          for (let p = 0; p < passed; p++) {
            realScenarios.push({
              name: `Cenário ${p + 1}`,
              status: 'passed',
              featureName: featureName,
              fullTestName: null,
              errorMessage: '',
              stackTrace: '',
              standardOutput: '',
              bddSteps: [],
              lineNumber: 0,
              duration: null
            });
          }
          for (let f = 0; f < failed; f++) {
            realScenarios.push({
              name: `Cenário ${passed + f + 1}`,
              status: 'failed',
              featureName: featureName,
              fullTestName: null,
              errorMessage: '',
              stackTrace: '',
              standardOutput: '',
              bddSteps: [],
              lineNumber: 0,
              duration: null
            });
          }
        }
      } else {
        // Último fallback: tenta extrair de "Total de testes: X, Aprovados: Y" ou "Com falha: Y"
        const totalMatch = output.match(/Total de testes:\s*(\d+).*Aprovados:\s*(\d+)/i);
        const comFalhaMatch = output.match(/Total de testes:\s*(\d+).*Com falha:\s*(\d+)/i);
        
        let total = 0;
        let passed = 0;
        let failed = 0;
        
        if (comFalhaMatch) {
          total = parseInt(comFalhaMatch[1]);
          failed = parseInt(comFalhaMatch[2]);
          passed = total - failed;
        } else if (totalMatch) {
          total = parseInt(totalMatch[1]);
          passed = parseInt(totalMatch[2]);
          failed = total - passed;
        }
          
        if (total > 0) {
          // Tenta extrair nome do cenário de outras partes do log
          const scenarioNameMatch = output.match(/Cenário executado:\s*(.+)/i) || 
                                   output.match(/Cenário:\s*(.+)/i) ||
                                   output.match(/Scenario:\s*(.+)/i) ||
                                   output.match(/Test:\s*(.+)/i);
          const extractedName = scenarioNameMatch ? scenarioNameMatch[1].trim() : null;
          
          // Extrai informações de erro dos logs do Playwright e exceções
          const playwrightErrors = [];
          const exceptions = [];
          const stackTraceLines = [];
          
          const outputLines = output.split('\n');
          for (const line of outputLines) {
            const trimmed = line.trim();
            // Captura erros do Playwright
            if (trimmed.includes('pw:api') && (trimmed.includes('timeout') || trimmed.includes('waiting for') || trimmed.includes('exceeded'))) {
              const cleanLine = trimmed.replace(/^\d{4}-\d{2}-\d{2}T[\d:.Z]+/, '').trim();
              if (cleanLine && !playwrightErrors.includes(cleanLine)) {
                playwrightErrors.push(cleanLine);
              }
            }
            // Captura exceções
            if (trimmed.match(/^(System\.[\w.]+Exception|[\w.]+Exception)/i)) {
              if (!exceptions.includes(trimmed)) {
                exceptions.push(trimmed);
              }
            }
            // Captura stack traces (linhas com "at" ou ".cs:")
            if (trimmed.match(/^\s+at\s+/) || trimmed.match(/\.cs:\d+/) || trimmed.match(/^\s+em\s+/)) {
              if (!stackTraceLines.includes(trimmed)) {
                stackTraceLines.push(trimmed);
              }
            }
          }
          
          const errorMessage = [...exceptions, ...playwrightErrors].join('\n');
          const stackTrace = stackTraceLines.join('\n');
          
          for (let p = 0; p < passed; p++) {
            realScenarios.push({
              name: extractedName || `Cenário ${p + 1}`,
              status: 'passed',
              featureName: featureName,
              fullTestName: null,
              errorMessage: '',
              stackTrace: '',
              standardOutput: '',
              bddSteps: [],
              lineNumber: 0,
              duration: null
            });
          }
          for (let f = 0; f < failed; f++) {
            realScenarios.push({
              name: extractedName || `Cenário ${passed + f + 1}`,
              status: 'failed',
              featureName: featureName,
              fullTestName: null,
              errorMessage: errorMessage,
              stackTrace: stackTrace,
              standardOutput: '',
              bddSteps: [],
              lineNumber: 0,
              duration: null
            });
          }
        }
      }
    }
  }
  
  // Retorna apenas os cenários reais
  return realScenarios;
}

/**
 * Categoriza erros e falhas baseado no output da execução
 * @param {Object} result - Resultado da execução
 * @param {string} result.stdout - Saída padrão
 * @param {string} result.stderr - Saída de erro
 * @param {number} result.exitCode - Código de saída
 * @param {boolean} result.success - Se a execução foi bem-sucedida
 * @param {boolean} result.hasTestExecution - Se testes foram executados
 * @param {boolean} result.hasCriticalErrors - Se há erros críticos
 * @param {string} result.patternMismatchReason - Motivo de não correspondência ao padrão
 * @returns {Object} Objeto com categoria, descrição e output normalizado
 */
function categorizeExecution(result) {
  const { stdout = '', stderr = '', exitCode, success, hasTestExecution, hasCriticalErrors, patternMismatchReason } = result;
  const allOutput = (stdout + stderr).toLowerCase();
  const originalOutput = stdout + stderr;
  
  // Extrai cenários individuais
  const scenarios = extractScenarios(originalOutput);
  const passedScenarios = scenarios.filter(s => s.status === 'passed');
  const failedScenarios = scenarios.filter(s => s.status === 'failed');

  // Se foi bem-sucedido, categoria é sucesso
  // Considera sucesso apenas se: success é true E há execução de testes E exitCode é 0
  // Exit code diferente de 0 sempre indica falha, independentemente dos cenários
  if (success && hasTestExecution && exitCode === 0) {
    let description = '✅ Execução bem-sucedida';
    
    // Adiciona informações sobre cenários individuais
    if (scenarios.length > 0) {
      description += '\n\n📊 RESULTADO DOS CENÁRIOS:\n' + '='.repeat(60) + '\n';
      
      if (passedScenarios.length > 0) {
        description += `\n✅ CENÁRIOS APROVADOS (${passedScenarios.length}):\n`;
        passedScenarios.forEach((scenario, index) => {
          const duration = scenario.duration ? ` (${scenario.duration}ms)` : '';
          description += `  ${index + 1}. ${scenario.name}${duration}\n`;
          
          // Adiciona BDD steps se houver
          if (scenario.bddSteps && scenario.bddSteps.length > 0) {
            description += `     Passos executados:\n`;
            scenario.bddSteps.forEach(step => {
              const statusIcon = step.result.includes('error') || step.result.includes('skipped') ? '❌' : '✅';
              description += `     ${statusIcon} ${step.fullLine}\n`;
            });
          }
        });
      }
      
      if (failedScenarios.length > 0) {
        description += `\n❌ CENÁRIOS FALHARAM (${failedScenarios.length}):\n`;
        failedScenarios.forEach((scenario, index) => {
          const duration = scenario.duration ? ` (${scenario.duration}ms)` : '';
          description += `  ${index + 1}. ${scenario.name}${duration}\n`;
          
          // Adiciona mensagem de erro
          if (scenario.errorMessage) {
            description += `     💥 Erro: ${scenario.errorMessage.split('\n')[0]}\n`;
          }
          
          // Adiciona BDD steps se houver
          if (scenario.bddSteps && scenario.bddSteps.length > 0) {
            description += `     Passos executados:\n`;
            scenario.bddSteps.forEach(step => {
              const statusIcon = step.result.includes('error') || step.result.includes('skipped') ? '❌' : '✅';
              description += `     ${statusIcon} ${step.fullLine}\n`;
            });
          }
        });
      } else {
        description += `\n✅ Todos os ${scenarios.length} cenário(s) foram aprovados!\n`;
      }
    }
    
    return {
      category: 'sucesso',
      description: description,
      normalizedOutput: normalizeOutput(originalOutput),
      scenarios: scenarios
    };
  }

  // Categoria: Arquivo não encontrado ou testes não executados
  if (patternMismatchReason) {
    let subCategory = 'arquivo_nao_encontrado';
    let description = patternMismatchReason;
    
    // Determina a categoria baseada no motivo
    if (patternMismatchReason.includes('não encontrado') || patternMismatchReason.includes('Arquivo de teste não encontrado')) {
      subCategory = 'arquivo_nao_encontrado';
    } else if (patternMismatchReason.includes('encontrados mas não executados') || patternMismatchReason.includes('Testes encontrados mas não executados')) {
      subCategory = 'testes_nao_executados';
    } else if (patternMismatchReason.includes('não executados') || patternMismatchReason.includes('Nenhum teste executado')) {
      subCategory = 'testes_nao_executados';
    } else {
      subCategory = 'problema_execucao';
    }
    
    // Adiciona detalhes se disponíveis
    if (result.patternMismatchDetails) {
      description = result.patternMismatchDetails;
    } else {
      // Tenta construir uma descrição melhor com informações disponíveis
      const filterInfo = result.filterUsed ? `\nFiltro usado: "${result.filterUsed}"` : '';
      const filesInfo = result.filesMatched !== null ? `\nArquivos encontrados: ${result.filesMatched}` : '';
      const testsInfo = result.testsDiscovered !== null ? `\nTestes descobertos: ${result.testsDiscovered}` : '';
      const runInfo = result.testsRun !== null ? `\nTestes executados: ${result.testsRun}` : '';
      
      if (filterInfo || filesInfo || testsInfo || runInfo) {
        description = `${patternMismatchReason}${filterInfo}${filesInfo}${testsInfo}${runInfo}`;
      }
    }
    
    return {
      category: subCategory,
      description: description,
      normalizedOutput: normalizeOutput(originalOutput)
    };
  }

  // Categoria: Erros críticos do sistema
  if (hasCriticalErrors) {
    let subCategory = 'erro_sistema';
    let description = `📋 ERRO CRÍTICO DO SISTEMA\n${'='.repeat(60)}\n` +
      `❌ O sistema não conseguiu executar os testes.\n\n` +
      `🔧 O QUE FAZER:\n${'-'.repeat(60)}\n`;
    
    if (allOutput.includes('no test is available')) {
      subCategory = 'sem_testes_disponiveis';
      description += `1️⃣ Verifique se há testes no projeto\n` +
        `2️⃣ Compile o projeto: dotnet build\n` +
        `3️⃣ Verifique se os arquivos .feature estão no projeto correto\n\n`;
    } else if (allOutput.includes('test discovery failed')) {
      subCategory = 'falha_descoberta_testes';
      description += `1️⃣ Verifique se o projeto está compilado corretamente\n` +
        `2️⃣ Execute: dotnet restore e depois dotnet build\n` +
        `3️⃣ Verifique se há erros de compilação no projeto\n\n`;
    } else if (allOutput.includes('no suitable test host')) {
      subCategory = 'host_indisponivel';
      description += `1️⃣ Verifique se o .NET SDK está instalado corretamente\n` +
        `2️⃣ Tente executar: dotnet --version\n` +
        `3️⃣ Reinicie o aplicativo\n\n`;
    } else {
      description += `1️⃣ Verifique os logs completos abaixo\n` +
        `2️⃣ Tente executar os testes diretamente pelo Visual Studio\n` +
        `3️⃣ Verifique a configuração do projeto\n\n`;
    }
    
    description += `📌 INFORMAÇÕES TÉCNICAS (para desenvolvedores):\n${'-'.repeat(60)}\n` +
      `Exit Code: ${exitCode || 'N/A'}\n` +
      `Verifique o output completo para mais detalhes.`;

    return {
      category: subCategory,
      description: description,
      normalizedOutput: normalizeOutput(originalOutput)
    };
  }

  // Categoria: Falha na execução dos testes
  // Marca como falha se: success é false (que já considera exitCode !== 0) OU há cenários que falharam
  // Exit code diferente de 0 sempre resulta em success = false
  const hasFailedScenarios = failedScenarios.length > 0;
  if ((hasTestExecution && !success) || (hasTestExecution && hasFailedScenarios)) {
    // Tenta identificar o tipo de falha específica
    let subCategory = 'teste_falhou';
    let description = `📋 FALHA NA EXECUÇÃO DOS TESTES\n${'='.repeat(60)}\n` +
      `❌ Os testes foram executados, mas falharam.\n\n`;
    
    // Adiciona informações detalhadas sobre cenários individuais
    if (scenarios.length > 0) {
      description += `📊 RESULTADO DOS CENÁRIOS:\n${'='.repeat(60)}\n`;
      
      if (passedScenarios.length > 0) {
        description += `\n✅ CENÁRIOS APROVADOS (${passedScenarios.length}):\n`;
        passedScenarios.forEach((scenario, index) => {
          const duration = scenario.duration ? ` (${scenario.duration}ms)` : '';
          description += `  ${index + 1}. ${scenario.name}${duration}\n`;
          
          // Adiciona BDD steps se houver
          if (scenario.bddSteps && scenario.bddSteps.length > 0) {
            description += `     Passos executados:\n`;
            scenario.bddSteps.forEach(step => {
              const statusIcon = step.result.includes('error') || step.result.includes('skipped') ? '❌' : '✅';
              description += `     ${statusIcon} ${step.fullLine}\n`;
            });
          }
        });
      }
      
      if (failedScenarios.length > 0) {
        description += `\n❌ CENÁRIOS FALHARAM (${failedScenarios.length}):\n`;
        failedScenarios.forEach((scenario, index) => {
          const duration = scenario.duration ? ` (${scenario.duration}ms)` : '';
          description += `  ${index + 1}. ${scenario.name}${duration}\n`;
          
          // Adiciona mensagem de erro completa
          if (scenario.errorMessage) {
            description += `     💥 Mensagem de Erro:\n`;
            scenario.errorMessage.split('\n').forEach(line => {
              if (line.trim()) {
                description += `        ${line.trim()}\n`;
              }
            });
          }
          
          // Adiciona stack trace se houver
          if (scenario.stackTrace) {
            description += `     📍 Rastreamento de Pilha:\n`;
            scenario.stackTrace.split('\n').slice(0, 5).forEach(line => {
              if (line.trim()) {
                description += `        ${line.trim()}\n`;
              }
            });
            if (scenario.stackTrace.split('\n').length > 5) {
              description += `        ... (${scenario.stackTrace.split('\n').length - 5} linhas adicionais)\n`;
            }
          }
          
          // Adiciona BDD steps se houver
          if (scenario.bddSteps && scenario.bddSteps.length > 0) {
            description += `     📝 Passos BDD Executados:\n`;
            scenario.bddSteps.forEach(step => {
              const statusIcon = step.result.includes('error') ? '❌' : 
                                step.result.includes('skipped') ? '⏭️' : '✅';
              description += `     ${statusIcon} ${step.fullLine}\n`;
            });
            
            // Mostra o último step executado antes da falha
            const lastStep = scenario.bddSteps[scenario.bddSteps.length - 1];
            if (lastStep && scenario.status === 'failed') {
              description += `     🎯 Último step executado: ${lastStep.translated || lastStep.fullLine}\n`;
            }
          }
          
          description += `\n`;
        });
      }
      
      description += `\n`;
    } else {
      // Se não há cenários identificados mas há falha, tenta extrair informações dos logs
      const playwrightErrors = [];
      const exceptions = [];
      const stackTraceLines = [];
      let lastBDDStep = null;
      
      const outputLines = originalOutput.split('\n');
      for (const line of outputLines) {
        const trimmed = line.trim();
        // Captura erros do Playwright
        if (trimmed.includes('pw:api') && (trimmed.includes('timeout') || trimmed.includes('waiting for') || trimmed.includes('exceeded') || trimmed.includes('not found') || trimmed.includes('Error') || trimmed.includes('failed'))) {
          const cleanLine = trimmed.replace(/^\d{4}-\d{2}-\d{2}T[\d:.Z]+\s+/, '').replace(/^pw:api\s+/, '').trim();
          if (cleanLine && !playwrightErrors.includes(cleanLine)) {
            playwrightErrors.push(cleanLine);
          }
        }
        // Captura exceções
        if (trimmed.match(/^(System\.[\w.]+Exception|[\w.]+Exception|TimeoutException)/i)) {
          if (!exceptions.includes(trimmed)) {
            exceptions.push(trimmed);
          }
        }
        // Captura stack traces
        if (trimmed.match(/^\s+at\s+/) || trimmed.match(/\.cs:\d+/) || trimmed.match(/^\s+em\s+/)) {
          if (!stackTraceLines.includes(trimmed)) {
            stackTraceLines.push(trimmed);
          }
        }
        // Detecta último step BDD executado
        const stepMatch = trimmed.match(/^(Given|When|Then|And|But)\s+(.+?)(?:\s*->\s*(.+))?$/i);
        if (stepMatch) {
          const stepKeyword = stepMatch[1];
          const stepText = stepMatch[2].trim();
          const translatedStep = translateBDDStep(`${stepKeyword} ${stepText}`);
          lastBDDStep = {
            keyword: stepKeyword,
            text: stepText,
            translated: translatedStep,
            fullLine: translatedStep
          };
        }
      }
      
      if (playwrightErrors.length > 0 || exceptions.length > 0 || lastBDDStep) {
        description += `\n📋 INFORMAÇÕES DE ERRO EXTRAÍDAS DOS LOGS:\n${'='.repeat(60)}\n`;
        
        if (lastBDDStep) {
          description += `\n🎯 Último Step BDD Executado:\n`;
          description += `   ${lastBDDStep.translated}\n`;
        }
        
        if (exceptions.length > 0) {
          description += `\n💥 Exceções Encontradas:\n`;
          exceptions.forEach(exception => {
            description += `   ${exception}\n`;
          });
        }
        
        if (playwrightErrors.length > 0) {
          description += `\n🌐 Erros do Playwright:\n`;
          playwrightErrors.slice(0, 10).forEach(error => {
            description += `   ${error}\n`;
          });
          if (playwrightErrors.length > 10) {
            description += `   ... (${playwrightErrors.length - 10} erros adicionais)\n`;
          }
        }
        
        if (stackTraceLines.length > 0) {
          description += `\n📍 Rastreamento de Pilha (primeiras 5 linhas):\n`;
          stackTraceLines.slice(0, 5).forEach(line => {
            description += `   ${line}\n`;
          });
          if (stackTraceLines.length > 5) {
            description += `   ... (${stackTraceLines.length - 5} linhas adicionais)\n`;
          }
        }
        
        description += `\n`;
      }
    }
    
    description += `🔧 O QUE FAZER:\n${'-'.repeat(60)}\n`;
    
    // Usa informações de erro dos cenários para categorizar melhor
    let detectedErrorType = null;
    if (failedScenarios.length > 0) {
      const firstFailedScenario = failedScenarios[0];
      if (firstFailedScenario.errorDetails) {
        detectedErrorType = firstFailedScenario.errorDetails.type;
        subCategory = firstFailedScenario.errorDetails.type === 'timeout' ? 'timeout' :
                     firstFailedScenario.errorDetails.type === 'element_not_found' ? 'elemento_nao_encontrado' :
                     firstFailedScenario.errorDetails.type === 'assertion_failed' ? 'assertion_falhou' :
                     firstFailedScenario.errorDetails.type === 'null_reference' ? 'erro_null_reference' :
                     firstFailedScenario.errorDetails.type === 'connection_error' ? 'erro_conexao' :
                     firstFailedScenario.errorDetails.type === 'browser_error' ? 'erro_chrome' :
                     subCategory;
        
        // Adiciona sugestão específica baseada no tipo de erro detectado
        if (firstFailedScenario.errorDetails.suggestion) {
          description += firstFailedScenario.errorDetails.suggestion + '\n\n';
        }
        
        // Adiciona informações específicas de timeout se disponíveis
        if (firstFailedScenario.errorDetails.type === 'timeout' && firstFailedScenario.errorDetails.locator) {
          description += `📍 Elemento esperado: ${firstFailedScenario.errorDetails.locator}\n`;
          if (firstFailedScenario.errorDetails.expectedText) {
            description += `📝 Texto esperado: "${firstFailedScenario.errorDetails.expectedText}"\n`;
          }
          description += `⏱️ Timeout: ${firstFailedScenario.errorDetails.timeoutValue}${firstFailedScenario.errorDetails.timeoutUnit}\n\n`;
        }
      }
    }
    
    // Fallback para detecção baseada em texto se não houver informações detalhadas
    if (!detectedErrorType) {
      if (allOutput.includes('timeout') || allOutput.includes('timed out')) {
        subCategory = 'timeout';
        description += `1️⃣ O teste demorou muito para executar (timeout)\n` +
          `2️⃣ Verifique se a aplicação está respondendo corretamente\n` +
          `3️⃣ Aumente o tempo limite se necessário\n` +
          `4️⃣ Verifique se há processos travados\n\n`;
      } else if (allOutput.includes('assertion') || allOutput.includes('assert')) {
        subCategory = 'assertion_falhou';
        description += `1️⃣ O teste falhou em uma verificação (assertion)\n` +
          `2️⃣ Verifique o que o teste esperava vs. o que foi recebido\n` +
          `3️⃣ Verifique os dados de teste e a aplicação\n\n`;
      } else if (allOutput.includes('element not found') || allOutput.includes('no such element')) {
        subCategory = 'elemento_nao_encontrado';
        description += `1️⃣ Um elemento da página não foi encontrado\n` +
          `2️⃣ Verifique se a página carregou completamente\n` +
          `3️⃣ Verifique se o seletor do elemento está correto\n` +
          `4️⃣ Verifique se a aplicação mudou e o teste precisa ser atualizado\n\n`;
      } else if (allOutput.includes('connection') || allOutput.includes('network')) {
        subCategory = 'erro_conexao';
        description += `1️⃣ Erro de conexão com a aplicação ou servidor\n` +
          `2️⃣ Verifique se a aplicação está rodando\n` +
          `3️⃣ Verifique sua conexão de internet\n` +
          `4️⃣ Verifique se a URL está correta\n\n`;
      } else if (allOutput.includes('exception') && allOutput.includes('null')) {
        subCategory = 'erro_null_reference';
        description += `1️⃣ Erro de referência nula no código do teste\n` +
          `2️⃣ Verifique se todos os objetos foram inicializados corretamente\n` +
          `3️⃣ Verifique os logs completos para encontrar a linha do erro\n\n`;
      } else if (allOutput.includes('chrome') || allOutput.includes('chromedriver')) {
        subCategory = 'erro_chrome';
        description += `1️⃣ Erro relacionado ao Chrome ou ChromeDriver\n` +
          `2️⃣ Verifique se o ChromeDriver está atualizado\n` +
          `3️⃣ Verifique se o Chrome está instalado corretamente\n` +
          `4️⃣ Tente fechar todas as instâncias do Chrome e executar novamente\n\n`;
      } else {
        description += `1️⃣ Verifique os logs completos abaixo para mais detalhes\n` +
          `2️⃣ Revise o código do teste que falhou\n` +
          `3️⃣ Verifique se a aplicação está funcionando corretamente\n\n`;
      }
    }
    
    description += `📌 INFORMAÇÕES TÉCNICAS (para desenvolvedores):\n${'-'.repeat(60)}\n` +
      `Exit Code: ${exitCode || 'N/A'}\n` +
      `Verifique o output completo para mais detalhes do erro.`;

    return {
      category: subCategory,
      description: description,
      normalizedOutput: normalizeOutput(originalOutput),
      scenarios: scenarios
    };
  }

  // Categoria padrão para casos não identificados
  return {
    category: 'erro_desconhecido',
    description: 'Erro não categorizado',
    normalizedOutput: normalizeOutput(originalOutput)
  };
}

/**
 * Normaliza o output removendo informações variáveis (timestamps, IDs, caminhos absolutos)
 * para permitir agrupamento de logs com outputs semelhantes
 * @param {string} output - Output original
 * @returns {string} Output normalizado
 */
function normalizeOutput(output) {
  if (!output) return '';

  let normalized = output;

  // Remove timestamps (vários formatos)
  normalized = normalized.replace(/\d{4}-\d{2}-\d{2}[T\s]\d{2}:\d{2}:\d{2}[.\d]*[Z]?/g, '[TIMESTAMP]');
  normalized = normalized.replace(/\d{2}\/\d{2}\/\d{4}\s+\d{2}:\d{2}:\d{2}/g, '[TIMESTAMP]');
  
  // Remove caminhos absolutos (substitui por [PATH])
  normalized = normalized.replace(/[A-Z]:\\[^\s]+/g, '[PATH]');
  normalized = normalized.replace(/\/[^\s]+/g, '[PATH]');
  
  // Remove IDs e hashes
  normalized = normalized.replace(/[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}/gi, '[UUID]');
  normalized = normalized.replace(/[a-f0-9]{32,}/gi, '[HASH]');
  
  // Remove números de porta específicos
  normalized = normalized.replace(/:\d{4,5}/g, ':[PORT]');
  
  // Remove números de processo/PID
  normalized = normalized.replace(/pid\s*[:=]\s*\d+/gi, 'pid=[PID]');
  normalized = normalized.replace(/process\s+id\s*[:=]\s*\d+/gi, 'process id=[PID]');
  
  // Normaliza espaços múltiplos
  normalized = normalized.replace(/\s+/g, ' ');
  
  // Remove linhas vazias duplicadas
  normalized = normalized.replace(/\n\s*\n\s*\n/g, '\n\n');

  return normalized.trim();
}

/**
 * Cria uma chave de agrupamento baseada no output normalizado
 * Usa um hash simples para agrupar logs semelhantes
 * @param {string} normalizedOutput - Output normalizado
 * @returns {string} Chave de agrupamento
 */
function createGroupKey(normalizedOutput) {
  if (!normalizedOutput) return 'empty';
  
  // Pega as primeiras 500 caracteres e as últimas 500 para criar uma chave
  const start = normalizedOutput.substring(0, 500);
  const end = normalizedOutput.length > 500 ? normalizedOutput.substring(normalizedOutput.length - 500) : '';
  const keyContent = (start + end).toLowerCase();
  
  // Cria um hash simples (soma de caracteres)
  let hash = 0;
  for (let i = 0; i < keyContent.length; i++) {
    const char = keyContent.charCodeAt(i);
    hash = ((hash << 5) - hash) + char;
    hash = hash & hash; // Convert to 32bit integer
  }
  
  return `group_${Math.abs(hash).toString(36)}`;
}

module.exports = {
  categorizeExecution,
  normalizeOutput,
  createGroupKey,
  extractScenarios
};


