const fs = require('fs');
const path = require('path');

async function scanFeatures(projectPath) {
  const features = [];
  
  // Normaliza o caminho do projeto
  const normalizedProjectPath = path.resolve(projectPath);
  
  // Verifica se o diretório do projeto existe
  if (!fs.existsSync(normalizedProjectPath)) {
    console.error(`Diretório do projeto não encontrado: ${normalizedProjectPath}`);
    return features;
  }

  // Tenta diferentes locais possíveis para a pasta Features
  const possibleFeatureDirs = [
    path.join(normalizedProjectPath, 'Features'),
    path.join(normalizedProjectPath, 'features'),
    path.join(normalizedProjectPath, 'FEATURES'),
    normalizedProjectPath // Também busca diretamente no projeto se não houver pasta Features
  ];

  let featuresDir = null;
  for (const dir of possibleFeatureDirs) {
    if (fs.existsSync(dir) && fs.statSync(dir).isDirectory()) {
      featuresDir = dir;
      console.log(`Pasta de features encontrada: ${featuresDir}`);
      break;
    }
  }

  if (!featuresDir) {
    console.warn(`Nenhuma pasta Features encontrada. Tentando buscar arquivos .feature diretamente em: ${normalizedProjectPath}`);
    // Se não encontrou a pasta Features, busca recursivamente no projeto todo
    scanDirectoryRecursive(normalizedProjectPath, normalizedProjectPath, features);
    return features;
  }

  // Calcula o caminho relativo base (Features)
  const baseRelativePath = path.relative(normalizedProjectPath, featuresDir);
  
  function scanDirectory(dir, relativePath = '') {
    try {
      const items = fs.readdirSync(dir, { withFileTypes: true });

      items.forEach(item => {
        try {
          const fullPath = path.join(dir, item.name);
          const relativeFilePath = relativePath ? path.join(relativePath, item.name) : item.name;

          if (item.isDirectory()) {
            // Ignora diretórios comuns que não contêm features
            const dirName = item.name.toLowerCase();
            if (dirName === 'bin' || dirName === 'obj' || dirName === 'node_modules' || 
                dirName === '.git' || dirName === '.vs' || dirName.startsWith('.')) {
              return;
            }
            scanDirectory(fullPath, relativeFilePath);
          } else if (item.isFile() && item.name.endsWith('.feature')) {
            try {
              const content = fs.readFileSync(fullPath, 'utf8');
              const featureInfo = parseFeatureFile(content, relativeFilePath, fullPath);
              features.push(featureInfo);
              console.log(`Feature encontrada: ${relativeFilePath}`);
            } catch (error) {
              console.error(`Erro ao ler arquivo ${fullPath}:`, error.message);
            }
          }
        } catch (error) {
          console.error(`Erro ao processar item ${path.join(dir, item.name)}:`, error.message);
        }
      });
    } catch (error) {
      console.error(`Erro ao ler diretório ${dir}:`, error.message);
    }
  }

  // Função auxiliar para buscar recursivamente em qualquer diretório
  function scanDirectoryRecursive(dir, basePath, featuresArray) {
    try {
      const items = fs.readdirSync(dir, { withFileTypes: true });

      items.forEach(item => {
        try {
          const fullPath = path.join(dir, item.name);
          
          // Ignora diretórios comuns
          if (item.isDirectory()) {
            const dirName = item.name.toLowerCase();
            if (dirName === 'bin' || dirName === 'obj' || dirName === 'node_modules' || 
                dirName === '.git' || dirName === '.vs' || dirName.startsWith('.')) {
              return;
            }
            scanDirectoryRecursive(fullPath, basePath, featuresArray);
          } else if (item.isFile() && item.name.endsWith('.feature')) {
            try {
              const relativePath = path.relative(basePath, fullPath);
              const content = fs.readFileSync(fullPath, 'utf8');
              const featureInfo = parseFeatureFile(content, relativePath, fullPath);
              featuresArray.push(featureInfo);
              console.log(`Feature encontrada: ${relativePath}`);
            } catch (error) {
              console.error(`Erro ao ler arquivo ${fullPath}:`, error.message);
            }
          }
        } catch (error) {
          console.error(`Erro ao processar item ${path.join(dir, item.name)}:`, error.message);
        }
      });
    } catch (error) {
      console.error(`Erro ao ler diretório ${dir}:`, error.message);
    }
  }

  // Escaneia a partir do diretório Features
  scanDirectory(featuresDir);
  
  console.log(`Total de features encontradas: ${features.length}`);
  return features;
}

function parseFeatureFile(content, relativePath, fullPath) {
  const lines = content.split('\n');
  let featureName = '';
  let description = '';
  const tags = [];
  const scenarios = [];

  let currentScenario = null;
  let inScenario = false;

  for (let i = 0; i < lines.length; i++) {
    const line = lines[i].trim();

    // Parse tags
    if (line.startsWith('@')) {
      const tagLine = line.split(/\s+/);
      tagLine.forEach(tag => {
        if (tag.startsWith('@')) {
          tags.push(tag);
        }
      });
    }

    // Parse Feature
    if (line.startsWith('Feature:')) {
      featureName = line.replace('Feature:', '').trim();
      // Próxima linha pode ser descrição
      if (i + 1 < lines.length && !lines[i + 1].trim().startsWith('@') && 
          !lines[i + 1].trim().startsWith('Scenario:')) {
        description = lines[i + 1].trim();
      }
    }

    // Parse Scenario
    if (line.startsWith('Scenario:') || line.startsWith('Scenario Outline:')) {
      if (currentScenario) {
        scenarios.push(currentScenario);
      }
      currentScenario = {
        name: line.replace(/Scenario( Outline)?:/, '').trim(),
        type: line.startsWith('Scenario Outline:') ? 'outline' : 'scenario',
        steps: []
      };
      inScenario = true;
    }

    // Parse Steps
    if (inScenario && currentScenario) {
      if (line.match(/^(Given|When|Then|And|But)\s+/)) {
        currentScenario.steps.push(line);
      }
    }

    // Finaliza scenario em linha vazia ou nova tag/feature
    if (inScenario && (line === '' || line.startsWith('@') || line.startsWith('Feature:'))) {
      if (currentScenario) {
        scenarios.push(currentScenario);
        currentScenario = null;
      }
      inScenario = false;
    }
  }

  if (currentScenario) {
    scenarios.push(currentScenario);
  }

  return {
    name: featureName || path.basename(relativePath, '.feature'),
    description,
    tags: [...new Set(tags)], // Remove duplicatas
    scenarios,
    path: relativePath,
    fullPath,
    fileSize: fs.statSync(fullPath).size,
    lastModified: fs.statSync(fullPath).mtime
  };
}

module.exports = { scanFeatures, parseFeatureFile };

