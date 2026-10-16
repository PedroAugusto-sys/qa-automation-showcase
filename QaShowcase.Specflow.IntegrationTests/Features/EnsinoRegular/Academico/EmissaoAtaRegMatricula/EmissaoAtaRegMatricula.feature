#language: pt-br
Funcionalidade: Emitir Ata de Registro de Matricula

@Academico
@EmissãoAtaRegistroMatricula
@EmissãoAtaRegistroMatriculaFiltroEmpresa
Cenario: Emitir ata de registro de matricula por empresa
Dado que desejo emitir a ata de registro de matriculas por empresa 
E clico em 'Filtro'  
E seleciono o ano '2023' 
E filtro por 'Empresa' 
E seleciono a empresa 'COLÉGIO SANTA ROSA'
E defino o título no leiaute 'Ata de Registro de Matrícula 123' 
E defino o leiaute 'Mostrar' 
E defino as Assinaturas
E defino as Assinaturas para mostrar o 'Diretor(a)' 
E defino as Assinaturas para mostrar o 'Secretário(a)' 
Quando clico em 'Emitir' 
Entao é gerado um relatório



@Academico
@EmissãoAtaRegistroMatricula
@EmissãoAtaRegistroMatriculaFiltroSerieTurma
Cenario: Emitir ata de registro de matricula por serie/turma
Dado que desejo emitir a ata de registro de matriculas por série/turma 
E clico em 'Filtro'  
E seleciono o ano '2023' 
E filtro por 'série/turma' 
E seleciono a série '1º ANO [Ensino Fundamental]' 
E seleciono a turma 'TURMA B'
E defino o título no leiaute 'Ata de Registro de Matrícula 123'
E defino o leiaute 'Mostrar' 
E defino as Assinaturas 
Quando clico em 'Emitir' 
Entao é gerado um relatório

@Academico
@EmissãoAtaRegistroMatricula
@EmissãoAtaRegistroMatriculaApresentandoAssinaturas
Cenario: Emitir ata de registro de matricula com assinaturas
Dado que desejo emitir a ata de registro de matriculas por série/turma  
E clico em 'Filtro'  
E seleciono o ano '2023' 
E filtro por 'série/turma' 
E seleciono a série '1º ANO [Ensino Fundamental]' 
E seleciono a turma 'TURMA B'
E defino o título no leiaute 'Ata de Registro de Matrícula 123' 
E defino o leiaute 'Mostrar' 
E defino as Assinaturas
E defino as Assinaturas para mostrar o 'Diretor(a)' 
E defino as Assinaturas para mostrar o 'Secretário(a)' 
Quando clico em 'Emitir' 
Entao é gerado um relatório