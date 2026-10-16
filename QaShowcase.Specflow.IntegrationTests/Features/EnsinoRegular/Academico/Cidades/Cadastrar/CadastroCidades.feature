Feature: Cadastro de cidades

@Academico
@Cidades
@CadastrarCidadeComCodigoIbge
Scenario: Cadastrar cidade com codigo IBGE
	Given que desejo realizar o cadastro de cidades
	And seleciono o operador 'OPERADOR QA'
	And informo o codigo do IBGE '5208707'
	And noto que o nome da cidade 'GOIANIA' e UF 'GO' sao apresentados
	When clico em 'Cadastrar'
	Then e apresentada a mensagem 'Registro cadastrado com sucesso!'
	Then removo o operador

@Academico
@Cidades
@CadastrarCidadeSemCodigoIbge
Scenario: Cadastrar cidade sem codigo IBGE
	Given que desejo realizar o cadastro de cidades
	And seleciono o operador 'OPERADOR QA'
	And insiro o nome da cidade 'Cidade teste'
	And seleciono a UF 'GO'
	When clico em 'Cadastrar'
	Then e apresentada a mensagem 'Registro cadastrado com sucesso!'
	Then removo o operador