Feature: Logar operador

@LogarOperador
@LogarSelecionandoOperador
Scenario: Logar selecionando um usuario
Given que desejo logar como um usuario
And seleciono o operador 'TESTE QA CRISTIANO'
When clico em 'Entrar'
Then o sistema apresenta a tela inicial 
Then removo o operador





@LogarOperador
@LogarInfomandoOperador
Scenario: Logar informando um usuario
Given que desejo logar como um usuario
And informo o operador 'TESTE QA CRISTIANO'
When clico em 'Entrar'
Then o sistema apresenta a tela inicial
Then removo o operador