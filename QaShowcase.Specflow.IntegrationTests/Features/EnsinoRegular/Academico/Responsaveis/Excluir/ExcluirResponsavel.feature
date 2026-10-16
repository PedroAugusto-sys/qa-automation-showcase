Feature: Exclusao de responsavel

@Academico
@Responsavel
@ExcluirResponsavelCadastrando
Scenario: Excluir dados de um responsavel
	Given que desejo excluir um responsavel
	And acesso o sistema utilizando o operador 'OPERADOR QA'
	And pesquiso pelo responsavel 'Responsavel QA'
	And clico em 'Remover'
	When clico em 'Sim'
	Then e apresentada a mensagem 'Item excluído com sucesso.'
	#Then removo o operador

@Academico
@Responsavel
@ExcluirResponsavelCadastrandoVinculado
Scenario: Excluir dados de um responsavel cadastrando
	Given que desejo excluir um responsavel
	And acesso o sistema utilizando o operador 'OPERADOR QA'
	And o responsavel 'Responsavel QA' possui vinculo com o aluno 'aluno teste'
	And pesquiso o responsavel 'Responsavel QA'
	And clico em 'Remover'
	When clico em 'Sim'
	Then e apresentada a mensagem 'Responsável não pode ser excluído, pois existe relação com as seguinte(s) matrícula(s):'
	#Then removo o operador