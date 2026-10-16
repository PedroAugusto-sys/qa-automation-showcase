Feature: Cadastrar Responsavel

@Academico
@Responsavel
@CadastrarResponsavelCamposObrigatorios
Scenario: Cadastro de um responsavel
	Given que desejo cadastrar um responsavel
	And acesso o sistema utilizando o operador 'OPERADOR QA'
	And clico em 'Cadastrar'
	And informo o nome 'Responsável teste qa'
	And informo a data de nascimento '01/01/1980'
	And seleciono o sexo 'Masculino'
	And informo o estado civil 'Casado(a)'
	And informo o cep '74375-230'
	And informo o cpf
	And preencho os atributos adicionais obrigatorios
	When clico em 'Gravar'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
	#Then removo o operador

@Academico
@Responsavel
@ValidarBotaoCancelarCadastroResponsavel
Scenario: Cancelar a operação de cadastro
	Given que desejo cadastrar um responsavel
	And acesso o sistema utilizando o operador 'OPERADOR QA'
	And clico em 'Cadastrar'
	And clico em 'Cancelar'
	When clico em 'Sim'
	Then e apresentada a tela de aterrissagem
	#Then removo o operador