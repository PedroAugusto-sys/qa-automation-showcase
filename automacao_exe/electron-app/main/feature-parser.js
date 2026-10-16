const fs = require('fs');
const path = require('path');

/**
 * Parseia um arquivo .feature e extrai Feature, Scenario e steps BDD
 * @param {string} filePath - Caminho para o arquivo .feature
 * @returns {Object|null} Objeto com feature, scenarios e steps, ou null se não conseguir parsear
 */
function parseFeatureFile(filePath) {
  if (!filePath || !fs.existsSync(filePath)) {
    return null;
  }

  try {
    const content = fs.readFileSync(filePath, 'utf8');
    const lines = content.split('\n');
    
    let feature = null;
    let currentScenario = null;
    const scenarios = [];
    
    for (let i = 0; i < lines.length; i++) {
      const line = lines[i];
      const trimmedLine = line.trim();
      
      // Ignora linhas vazias e comentários
      if (!trimmedLine || trimmedLine.startsWith('#')) {
        continue;
      }
      
      // Detecta Feature
      const featureMatch = trimmedLine.match(/^Feature:\s*(.+)/i);
      if (featureMatch) {
        feature = featureMatch[1].trim();
        continue;
      }
      
      // Detecta Scenario ou Scenario Outline
      const scenarioMatch = trimmedLine.match(/^Scenario(?: Outline)?:?\s*(.+)/i);
      if (scenarioMatch) {
        // Salva cenário anterior se existir
        if (currentScenario) {
          scenarios.push(currentScenario);
        }
        
        currentScenario = {
          name: scenarioMatch[1].trim(),
          steps: [],
          lineNumber: i + 1
        };
        continue;
      }
      
      // Detecta steps BDD (Given, When, Then, And, But)
      // Suporta tanto inglês quanto português
      const stepMatch = trimmedLine.match(/^(Given|When|Then|And|But|Dado|Quando|Então|E|Mas)\s+(.+)/i);
      if (stepMatch) {
        const keyword = stepMatch[1];
        const text = stepMatch[2].trim();
        
        // Normaliza keyword para inglês
        const normalizedKeyword = normalizeKeyword(keyword);
        
        if (currentScenario) {
          currentScenario.steps.push({
            keyword: normalizedKeyword,
            originalKeyword: keyword,
            text: text,
            lineNumber: i + 1,
            fullLine: `${normalizedKeyword} ${text}`
          });
        }
        continue;
      }
      
      // Detecta Background (steps que se aplicam a todos os cenários)
      if (trimmedLine.match(/^Background:/i)) {
        // Por enquanto, ignora Background - pode ser implementado depois se necessário
        continue;
      }
      
      // Detecta Examples (para Scenario Outline)
      if (trimmedLine.match(/^Examples?:/i)) {
        // Para de coletar steps quando encontra Examples
        if (currentScenario) {
          scenarios.push(currentScenario);
          currentScenario = null;
        }
        continue;
      }
    }
    
    // Adiciona o último cenário se existir
    if (currentScenario) {
      scenarios.push(currentScenario);
    }
    
    return {
      feature: feature || 'Feature não identificada',
      filePath: filePath,
      scenarios: scenarios
    };
  } catch (error) {
    console.error('[feature-parser] Erro ao parsear arquivo .feature:', error);
    return null;
  }
}

/**
 * Normaliza keyword BDD para inglês
 * @param {string} keyword - Keyword original (pode ser em português ou inglês)
 * @returns {string} Keyword normalizado em inglês
 */
function normalizeKeyword(keyword) {
  const normalized = keyword.trim();
  
  const translations = {
    'Dado': 'Given',
    'Quando': 'When',
    'Então': 'Then',
    'E': 'And',
    'Mas': 'But'
  };
  
  return translations[normalized] || normalized;
}

/**
 * Encontra um cenário específico no arquivo .feature
 * @param {string} filePath - Caminho para o arquivo .feature
 * @param {string} scenarioName - Nome do cenário a procurar
 * @returns {Object|null} Objeto com o cenário e seus steps, ou null se não encontrado
 */
function findScenarioInFeature(filePath, scenarioName) {
  const parsed = parseFeatureFile(filePath);
  if (!parsed) {
    return null;
  }
  
  const scenario = parsed.scenarios.find(s => 
    s.name.toLowerCase() === scenarioName.toLowerCase() ||
    s.name.toLowerCase().includes(scenarioName.toLowerCase()) ||
    scenarioName.toLowerCase().includes(s.name.toLowerCase())
  );
  
  if (scenario) {
    return {
      feature: parsed.feature,
      scenario: scenario,
      filePath: filePath
    };
  }
  
  return null;
}

/**
 * Extrai todos os steps BDD de um cenário específico
 * @param {string} filePath - Caminho para o arquivo .feature
 * @param {string} scenarioName - Nome do cenário
 * @returns {Array} Array de steps BDD
 */
function getScenarioSteps(filePath, scenarioName) {
  const result = findScenarioInFeature(filePath, scenarioName);
  if (result && result.scenario) {
    return result.scenario.steps;
  }
  return [];
}

module.exports = {
  parseFeatureFile,
  findScenarioInFeature,
  getScenarioSteps,
  normalizeKeyword
};
