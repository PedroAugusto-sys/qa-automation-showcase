Feature: Editar Lancamento de Frequencia

@Academico
@CadastroLancamentoDiarioFrequencia
@EditandoLancamentoDeFrequenciaParaSeriesComTipoDeFrequenciaPorDiaCadastrando
Scenario: Editando Lancamento de Frequencia para Series com Tipo de Frequencia por dia
  Given que preciso realizar a 'edicao' do lancamento de frequencia
  And acesso o sistema utilizando o operador 'HUGO QA KKK'
  And seleciono o ano letivo : '2025'
  And seleciono o mes : 'janeiro'
  And seleciono a Serie/Turma : '2º ANO [Ensino Fundamental] / A - ANGLO'
  And clico em 'Carregar'
  And seleciono o dia especifico : 'Todos'
  And lanco falta para todos os alunos
  When clico em 'Gravar'
  Then o sistema apresenta a mensagem : 'Cadastro realizado com sucesso!'
  Then removo o operador


@Academico
@CadastroLancamentoDiarioFrequencia
@EditandoLancamentoDeFrequenciaParaSeriesComTipoDeFrequenciaPorDisciplinaCadastrando
Scenario: Editando Lançamento de Frequência para Séries com Tipo de Frequência por disciplina
  Given que preciso realizar a 'edicao' do lancamento de frequencia
  And acesso o sistema utilizando o operador 'HUGO QA KKK'
  And seleciono o ano letivo : '2024'
  And seleciono o mes : 'setembro'
  And seleciono a Serie/Turma : '10 - 7º ANO [Ensino Fundamental] / 7 - A - CJ'
  And seleciono a disciplina : 'GEOGRAFIA'
  And clico em 'Carregar'
  And seleciono o dia especifico : 'Todos'
  And lanco falta para todos os alunos
  When clico em 'Gravar'
  Then o sistema apresenta a mensagem : 'Cadastro realizado com sucesso!'
  Then removo o operador