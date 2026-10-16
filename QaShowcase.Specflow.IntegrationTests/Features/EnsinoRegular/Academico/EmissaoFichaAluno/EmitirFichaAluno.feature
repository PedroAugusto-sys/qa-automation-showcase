#language: pt-br
Funcionalidade:: Emitir ficha do aluno

@Academico
@EmissãoFichaAluno
@EmissãoFichaAlunoFiltroEmpresa
Cenário: Emitir ficha com aluno filtrando por empresa
Dado emitir a ficha do aluno  
E filtro por 'Empresa' 
E seleciono a empresa 'COLÉGIO SANTA ROSA'
E clico em 'Mostrar' 
Quando clico em 'Emitir' 
Então é gerado um relatório

@Academico
@EmissãoFichaAluno
@EmissãoFichaAlunoFiltroSerie
Cenário: Emitir ficha com aluno filtrando por série
Dado emitir a ficha do aluno  
E filtro por 'Série' 
E seleciono a série '3º ANO [Ensino Fundamental]'
E clico em 'Mostrar' 
Quando clico em 'Emitir' 
Então é gerado um relatório

@Academico
@EmissãoFichaAluno
@EmissãoFichaAlunoFiltroSerieTurma
Cenário: Emitir ficha com aluno filtrando por série/turma
Dado emitir a ficha do aluno  
E filtro por 'Série/Turma' 
E seleciono a série '1º ANO [Ensino Fundamental]'
E seleciono a turma 'TURMA A'
E clico em 'Mostrar' 
Quando clico em 'Emitir' 
Então é gerado um relatório

@Academico
@EmissãoFichaAluno
@EmissãoFichaAlunoFiltroAluno
Cenário: Emitir ficha com aluno filtrando por aluno
Dado emitir a ficha do aluno  
E filtro por 'Aluno' 
E seleciono o aluno 240476
E clico em 'Mostrar' 
Quando clico em 'Emitir' 
Então é gerado um relatório


@Academico
@EmissãoFichaAluno
@EmissãoFichaAlunoFiltroEmpresaOcultando
Cenário: Emitir ficha com aluno filtrando por empresa e ocultando
Dado emitir a ficha do aluno  
E filtro por 'Empresa' 
E seleciono a empresa 'COLÉGIO SANTA ROSA'
E clico em 'Ocultar' 
Quando clico em 'Emitir' 
Então é gerado um relatório

@Academico
@EmissãoFichaAluno
@EmissãoFichaAlunoFiltroSerieOcultando
Cenário: Emitir ficha com aluno filtrando por série e ocultando
Dado emitir a ficha do aluno  
E filtro por 'Série' 
E seleciono a série '1º ANO [Ensino Fundamental]'
E clico em 'Ocultar' 
Quando clico em 'Emitir' 
Então é gerado um relatório

@Academico
@EmissãoFichaAluno
@EmissãoFichaAlunoFiltroSerieTurmaOcultando
Cenário: Emitir ficha com aluno filtrando por série/turma e ocultando
Dado emitir a ficha do aluno  
E filtro por 'Série/Turma' 
E seleciono a série '1º ANO [Ensino Fundamental]'
E seleciono a turma 'TURMA A'
E clico em 'Ocultar' 
Quando clico em 'Emitir' 
Então é gerado um relatório

@Academico
@EmissãoFichaAluno
@EmissãoFichaAlunoFiltroAlunoOcultando
Cenário: Emitir ficha com aluno filtrando por aluno e ocultando
Dado emitir a ficha do aluno  
E filtro por 'Aluno' 
E seleciono o aluno '240476'
E clico em 'Ocultar' 
Quando clico em 'Emitir' 
Então é gerado um relatório

@Academico
@EmissãoFichaAluno
@EmissãoFichaAlunoGenerica
Cenário: Emitir ficha com aluno
Dado emitir a ficha do aluno  
E aplico um filtro 
E seleciono um item do filtro
E seleciono uma visibilidade para empresa
Quando clico em 'Emitir' 
Então é gerado um relatório