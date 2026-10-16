Feature: Pesquisar operador

@Logins
@PesquisarOperadorInformarNome
Scenario: Pesquisar operador
Given que desejo pesquisar um operador
And clico no campo pesquisar
When digito 'ANA'
Then o operador e apresentado

@Logins
@PesquisarOperadorCadastrando
Scenario: Pesquisar operador cadastrando
Given que desejo pesquisar um operador
And cadastro um operador
And clico no campo pesquisar
When digito operador
Then o operador e apresentado
