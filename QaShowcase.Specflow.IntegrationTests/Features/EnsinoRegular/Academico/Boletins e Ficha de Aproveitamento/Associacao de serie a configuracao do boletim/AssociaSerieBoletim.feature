Feature: Associacao De Serie A Configuracao Do Boletim

@Academico
@AssociacaoDeSerieAConfiguracaoDoBoletim
@CadastrarAssociacaoDaConfiguracaoComASerie
Scenario: Realizar a associacao de serie a configuracao do boletim
	Given que desejo cadastrar uma nova associacao de configuracao com uma serie
	And acesso o sistema utilizando o operador 'OPERADOR QA AUTOMATION'
	And seleciono o ano letivo '2025'
	And clico em 'Carregar'
	And seleciono a serie '??CURSO TESET??'
	And seleciono a configuracao do boletim 'CONFIGURAÇÃO PADRÃO'
	And seleciono a progressao parcial como 'NÃO'
	When clico em 'Gravar'
	Then e apresentada a mensagem 'Operação realizada com sucesso!'
	Then removo o operador