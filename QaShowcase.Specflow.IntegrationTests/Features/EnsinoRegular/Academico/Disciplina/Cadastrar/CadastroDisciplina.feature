Feature: Cadastro de Disciplina

@Academico
@Disciplinas
@CadastrarDisciplinasObrigatorio
Scenario: Cadastrar disciplina 
Given que desejo cadastrar uma disciplina comum 
And acesso o sistema utilizando o operador 'TESTE QA'
And clico em 'Cadastrar'
And informo o nome 'DISCIPLINA TESTE QA'
And seleciono a situacao 'Inativo'
And informo a area de conhecimento 'Area de conhecimento teste qa'
And seleciono o tipo 'Comum'
And seleciono o grupo de historico escolar 'Parte nacional comum'
And informo o Tipo de agregacao 'Nao ha agregacao'
And em Historico escolar seleciono a opcao 'Nao'
When clico em 'Gravar'
Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
Then removo o operador