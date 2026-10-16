Feature: Validar Situacao

@Logins
@ValidarSituacao
Scenario: Validar situacao do operador
Given que desejo validar a situacao de um operador ativo
And digito o nome do operador
When seleciono o filtro 'Ativas'
Then o operador e apresentado