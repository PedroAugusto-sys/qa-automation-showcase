## DIRETRIZ DE HOMOLOGAÇÃO

Foram identificados baseados nas diretrizes de homologação as seguintes Estórias de usuário

- Emitir ata de registro

## CASOS DE TESTE ABORDADOS

As diretrizes abordam os seguintes casos de teste para a tela de Emissão da da ata de registro de matricula

- Emitir ata de registro filtrando por empresa
- Emitir ata de registro filtrando por série-turma
- Emitir ata de registro adicionando assinaturas

Necessitades identificadas

- Criar um script básico e simples, levando em consideração todos campos obrigatórios
- Por padrão o leiaute esta configurado para mostrar em todos casos
- Validar de forma automatizada o resultado da emissão da melhor forma possível

| ID | RW.ARM.Emitir.C1 |
|----| ------- |
|NOME|Emitir ata de registro de matricula por empresa|
|AMBIENTE|REGULAR WEB|
|AUTOR|JADER AMORIM|
|PRE-CONDIÇÕES|Sem-Pré Condições</br>|
|Cenário| **Dado** que desejo emitir a ata de registro de matriculas por empresa </br> **E** clico em 'Filtro'</br> **E** seleciono o ano '2023' </br>**E** filtro por 'Empresa'</br>**E** seleciono a empresa 'COLÉGIO SANTA ROSA'</br>**E** defino o título no leiaute 'Ata de Registro de Matrícula 123'</br>**E** defino o leiaute para 'Mostrar' </br>**E** defino as Assinaturas </br>**Quando** clico em 'Emitir' </br>**Então** é gerado um relatório|
|RESULTADO ESPERADO|Ata de Registro emitida|

| ID | RW.ARM.Emitir.C2 |
|----| ------- |
|NOME|Emitir ata de registro de matricula por série/turma|
|AMBIENTE|REGULAR WEB|
|AUTOR|JADER AMORIM|
|PRE-CONDIÇÕES|Série/Turma cadastrados</br>|
|Cenário| **Dado** que desejo emitir a ata de registro de matriculas por série/turma </br> **E** clico em 'Filtro' </br> **E** seleciono o ano '2023' </br>**E** filtro por 'série/turma' </br>**E** seleciono a série '1º ANO [Ensino Fundamental]' </br>**E** seleciono a turma 'TURMA B'</br>**E** defino o título no leiaute 'Ata de Registro de Matrícula 123'</br>**E** defino o leiaute 'Mostrar' </br>**E** defino as Assinaturas </br>**Quando** clico em 'Emitir' </br>**Então** é gerado um relatório|
|RESULTADO ESPERADO|Ata de Registro emitida|


| ID | RW.ARM.Emitir.C3 |
|----| ------- |
|NOME|Emitir ata de registro de matricula por série/turma|
|AMBIENTE|REGULAR WEB|
|AUTOR|JADER AMORIM|
|PRE-CONDIÇÕES|Série/Turma cadastrados</br>|
|Cenário| **Dado** que desejo emitir a ata de registro de matriculas por série/turma </br> **E** clico em 'Filtro' </br> **E** seleciono o ano '2023' </br>**E** filtro por 'série/turma' </br>**E** seleciono a série '1º ANO [Ensino Fundamental]' </br>**E** seleciono a turma 'TURMA B'</br>**E** defino o título no leiaute 'Ata de Registro de Matrícula 123' </br>**E** defino o leiaute 'Mostrar' </br>**E** defino as Assinaturas**E** defino as Assinaturas para mostrar o 'Diretor(a)' </br>**E** defino as Assinaturas para mostrar o 'Secretário(a)' </br>**Quando** clico em 'Emitir' </br>**Então** é gerado um relatório|
|RESULTADO ESPERADO|Ata de Registro emitida|