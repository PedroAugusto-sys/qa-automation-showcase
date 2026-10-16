Feature: Editar o apelido da empresa.

@Academico
@Empresas
@EditarApelidoEmpresa
Scenario: Edicao de empresa
	Given que desejo editar uma empresa
	And acesso o sistema utilizando o operador 'TESTE QA'
	And clico em editar no cadastro de empresa
	And altero o apelido para 'CIN EDUCACIONAL LTDA'
	When clico em gravar
	Then o cadastro e alterado com sucesso
	Then removo o operador
