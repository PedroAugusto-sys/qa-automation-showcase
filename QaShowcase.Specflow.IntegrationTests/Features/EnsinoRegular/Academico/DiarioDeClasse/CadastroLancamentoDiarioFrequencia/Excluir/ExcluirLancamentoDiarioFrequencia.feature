Feature: Excluir Lancamento de Frequencia

@Academico
@CadastroLancamentoDiarioFrequencia
@ExcluindoLancamentoDeFrequenciaParaSeriesComTipoDeFrequenciaPorDiaCadastrando
Scenario: Excluindo Lancamento de Frequencia para Series com Tipo de Frequencia por dia
  Given que preciso realizar a 'exclusao' do lancamento de frequencia
  And acesso o sistema utilizando o operador 'TESTE JADER'
  And seleciono o ano letivo : '2025'
  And seleciono o mes : 'agosto'
  And seleciono a Serie/Turma : '2º ANO [Ensino Fundamental] / A - ANGLO'
  And clico em 'Carregar'
  And seleciono o dia especifico : '5'
  And clico em 'Excluir'
  And o sistema apresenta a mensagem : 'Deseja excluir os lançamentos do dia 5 do mês de agosto?'
  When clico em 'Sim'
  Then o sistema apresenta a mensagem : 'Lançamento excluído com sucesso'
  #Then removo o operador


@Academico
@CadastroLancamentoDiarioFrequencia
@ExcluindoLancamentoDeFrequenciaParaSeriesComTipoDeFrequenciaPorDisciplinaCadastrando
Scenario: Excluindo Lançamento de Frequência para Séries com Tipo de Frequência por disciplina
  Given que preciso realizar a 'exclusao' do lancamento de frequencia
  And acesso o sistema utilizando o operador 'TESTE JADER'
  And seleciono o ano letivo : '2024'
  And seleciono o mes : 'setembro'
  And seleciono a Serie/Turma : '10 - 7º ANO [Ensino Fundamental] / 7 - A - CJ'
  And seleciono a disciplina : 'GEOGRAFIA'
  And clico em 'Carregar'
  And seleciono o dia especifico : '17'
  And clico em 'Excluir'
  And o sistema apresenta a mensagem : 'Deseja excluir os lançamentos do dia 17 do mês de setembro?'
  When clico em 'Sim'
  Then o sistema apresenta a mensagem : 'Lançamento excluído com sucesso'
  #Then removo o operador