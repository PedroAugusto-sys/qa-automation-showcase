#language: pt-br
Funcionalidade: Cadastro de serie

@Academico
@Serie
@SerieCadastrar
Cenario: Cadastro de serie
	Dado que desejo cadastrar uma serie
	E clico em cadastrar a serie
	E informo a descricao "Serie <Data_Atual>"
	E informo a carga horaria
	Quando clico em gravar o cadastro da serie
	Entao o cadastro da serie eh realizado com sucesso

@Academico
@Serie
@SerieCadastrar
Cenario: Cadastro de serie informando disciplinas
	Dado que desejo cadastrar uma serie
	E clico em cadastrar a serie
	E informo a descricao "Serie <Data_Atual>"
	E informo a carga horaria
	E adiciono disciplinas no cadastro da serie
	Quando clico em gravar o cadastro da serie
	Entao o cadastro da serie eh realizado com sucesso
