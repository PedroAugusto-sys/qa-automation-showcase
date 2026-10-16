# Ícone da Aplicação

## Como adicionar o ícone personalizado

Para adicionar um ícone personalizado à aplicação, siga os passos abaixo:

### 1. Preparar o arquivo de ícone

- **Formato**: PNG
- **Tamanho recomendado**: 
  - Windows: 256x256 pixels (ou múltiplos tamanhos: 16x16, 32x32, 48x48, 256x256)
  - macOS: 512x512 pixels (ou múltiplos tamanhos: 16x16, 32x32, 128x128, 256x256, 512x512, 1024x1024)
  - Linux: 512x512 pixels

### 2. Nomear o arquivo

Nomeie o arquivo como `icon.png` e coloque-o neste diretório:
```
electron-app/assets/icon.png
```

### 3. Estrutura de diretórios

```
electron-app/
  └── assets/
      ├── icon.png  ← Coloque seu ícone aqui
      └── README.md (este arquivo)
```

### 4. Formatos alternativos

Se preferir usar formatos específicos por plataforma:

- **Windows**: `icon.ico` (formato ICO com múltiplos tamanhos)
- **macOS**: `icon.icns` (formato ICNS)
- **Linux**: `icon.png` (PNG de alta resolução)

### 5. Verificação

Após adicionar o ícone, a aplicação irá:
- Usar o ícone na barra de tarefas (Windows)
- Usar o ícone no dock (macOS)
- Usar o ícone na janela da aplicação
- Usar o ícone nas notificações

### 6. Build

Após adicionar o ícone, você pode precisar reconstruir a aplicação para que o ícone seja incluído no executável:

```bash
npm run build
```

### Notas

- O ícone será carregado automaticamente se estiver no caminho correto
- Se o ícone não for encontrado, a aplicação usará o ícone padrão do Electron
- Para melhor qualidade, use ícones vetoriais convertidos para PNG em alta resolução
