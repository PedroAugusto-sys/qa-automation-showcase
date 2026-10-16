Feature: Lançamento de Frequencia


@Academico
@CadastroLancamentoDiarioFrequencia
@CadastrarLancamentoDeFrequenciaParaSeriesComTipoDeFrequenciaPorDiaParaTodosOsAlunos
Scenario: Cadastrar Lancamento De Frequencia Para Series Com Tipo De Frequencia Por Dia Para Todos Os Alunos

Given que preciso realizar o 'lancamento' de frequencia
And acesso o sistema utilizando o operador 'HUGO QA KKK'
And seleciono o ano letivo : '2025'
And seleciono o mes : 'janeiro'
And seleciono a Serie/Turma : '2º ANO [Ensino Fundamental] / A - ANGLO'
And clico em 'Carregar'
And seleciono o dia especifico : 'Todos'
And lanco presenca todos os alunos
When clico em 'Gravar'
Then o sistema apresenta a mensagem : 'Cadastro realizado com sucesso!'
Then removo o operador


@Academico
@CadastroLancamentoDiarioFrequencia
@CadastrarLancamentoDeFrequenciaParaSeriesComTipoDeFrequenciaPorDisciplinaParaTodosOsAlunos
Scenario: Cadastrar Lancamento De Frequencia Para Series Com Tipo De Frequencia Por Disciplina Para Todos Os Alunos
  Given que preciso realizar o 'lancamento' de frequencia
  And acesso o sistema utilizando o operador 'HUGO QA KKK'
  And seleciono o ano letivo : '2024'
  And seleciono o mes : 'setembro'
  And seleciono a Serie/Turma : '10 - 7º ANO [Ensino Fundamental] / 7 - A - CJ'
  And seleciono a disciplina : 'GEOGRAFIA'
  And clico em 'Carregar'
  And seleciono o dia especifico : 'Todos'
  And lanco presenca todos os alunos
  When clico em 'Gravar'
  Then o sistema apresenta a mensagem : 'Cadastro realizado com sucesso!'
  Then removo o operador