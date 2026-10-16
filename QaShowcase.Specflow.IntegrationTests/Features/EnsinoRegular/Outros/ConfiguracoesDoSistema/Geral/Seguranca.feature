Feature: Alterar configuracoes de seguranca

@Seguranca
@AlterarOpcaoDeSenhaPadrao
Scenario: Alterar selecao da senha padrao
Given que desejo alterar a configuracao da senha padrao
And seleciono a opcao 'Sim'
When clico em Gravar
Then apresenta a mensagem de confirmacao

@Seguranca
@AlterarOpcaoDePrimeiroLogin
Scenario: Alterar selecao de primeiro login
Given que desejo alterar a configuracao do primeiro login
And seleciono a opcao 'Nao'
When clico em Gravar
Then apresenta a mensagem de confirmacao