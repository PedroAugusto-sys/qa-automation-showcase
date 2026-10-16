Feature: Editar Disciplina

@Academico
@Disciplinas
@EditarNomeDisciplinasCadastrando
Scenario: Editar nome da disciplina 
Given que desejo alterar o nome de uma disciplina 
And acesso o sistema utilizando o operador 'TESTE QA'
And pesquiso pela disciplina 'DISCIPLINA TESTE QA'
And clico em 'Editar'
And altero o nome da disciplina para 'MATEMATICA ALTERADA'
When clico em 'Gravar'
Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
Then removo o operador

@Academico
@Disciplinas
@AlterarSituacaoDisciplinasCadastrando
Scenario: Alterar situacao da disciplina
Given que desejo alterar a situacao de uma disciplina
And acesso o sistema utilizando o operador 'TESTE QA'
And pesquiso pela disciplina 'MATEMATICA ALTERADA'
And clico em 'Editar'
And altero a situacao para 'Inativo'
When clico em 'Gravar'
Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
Then removo o operador
