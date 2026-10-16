Feature: Editar Operador

@Logins
@EditarOperador
Scenario: Editar Operador cadastrado
	Given que desejo editar Operador
	And pesquiso pelo operador 'PEDRO QA'
	And clico em editar
	And altero a descricao incluindo
	When clico em Gravar
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'

@Logins
@EditarOperadorCadastrando
Scenario: Editar Operador 
	Given que desejo editar Operador
	And pesquiso pelo operador
	And clico em editar
	And altero a descricao incluindo
	When clico em Gravar
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'


@Logins
@EditarOperadorCadastrando
Scenario: Editar Operador Cancelar
	Given que desejo editar Operador
	And pesquiso pelo operador
	And clico em editar
	And altero a descricao incluindo
	When clico em 'Cancelar'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'