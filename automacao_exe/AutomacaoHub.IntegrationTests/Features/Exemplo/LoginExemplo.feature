Feature: Login no Sistema

Descrição de exemplo para login no sistema

@Login
@Exemplo
Scenario: Realizar login como administrador
	Given que eu acesso o sistema
	When eu realizo o login com 'ADMINISTRADOR'
	Then o sistema apresenta a tela inicial

