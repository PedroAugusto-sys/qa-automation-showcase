Feature: Editar Responsavel

@Academico
@Responsavel
@EditarResponsavelNomeCadastrando
Scenario: Editar responsavel cadastrando
	Given que desejo editar um responsavel
	And acesso o sistema utilizando o operador 'OPERADOR QA'
	And pesquiso pelo responsavel 'Responsavel QA'
	And clico em 'Editar'
	And altero o nome do responsavel para 'Responsavel QA editado'
	When clico em 'Gravar'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
	#Then removo o operador

@Academico
@Responsavel
@ValidarBotaoCancelarEditarResponsavel
Scenario: Cancelar edicao de responsavel
	Given que desejo editar um responsavel
	And acesso o sistema utilizando o operador 'OPERADOR QA'
	And pesquiso pelo responsavel 'Responsavel QA'
	And clico em 'Editar'
	And clico em 'Cancelar'
	When clico em 'Sim'
	Then e apresentada a tela de aterrissagem
	#Then removo o operador