Feature: Excluir ficha do aluno

@Academico
@FichaAluno
@ExcluirFichaAlunoSemVinculoCadastrando
Scenario: Excluir ficha do aluno
	Given que preciso excluir um aluno
	And acesso o sistema utilizando o operador 'OPERADOR QA'
	And pesquiso pelo aluno 'ALUNO TESTE QA'
	And clico em 'Remover'
	When clico em 'Sim'
	Then e apresentada a mensagem 'Item excluído com sucesso.'
	Then removo o operador

# Este cenário será produzido após a automação de caixa estar pronta
@Academico
@FichaAluno
@ExcluirFichaAlunoComVinculoFinanceiroCadastrando
Scenario: Excluir ficha do aluno com vinculo financeiro
	Given que preciso excluir um aluno
	And acesso o sistema utilizando o operador 'OPERADOR QA'
	And pesquiso pelo aluno 'ALUNO TESTE QA'
	And o aluno possui vinculos financeiros
	And clico em 'Excluir'
	When clico em 'Sim'
	Then e apresentada a mensagem 'Existe lançamento no caixa vinculado a esta pessoa, portanto a mesma não poderá ser excluída.'
	Then removo o operador

# Este cenário será produzido após a automação de matrícula estar pronta
@Academico
@FichaAluno
@ExcluirFichaAlunoComMatriculaAtivaCadastrando
Scenario: Excluir ficha do aluno com matricula ativa
	Given que preciso excluir um aluno
	And acesso o sistema utilizando o operador 'OPERADOR QA'
	And pesquiso pelo aluno 'ALUNO TESTE QA'
	And o aluno possui matricula ativa
	And clico em 'Excluir'
	When clico em 'Sim'
	Then e apresentada a mensagem 'Esta pessoa já foi matriculada, portanto não poderá ser excluída.'
	Then removo o operador