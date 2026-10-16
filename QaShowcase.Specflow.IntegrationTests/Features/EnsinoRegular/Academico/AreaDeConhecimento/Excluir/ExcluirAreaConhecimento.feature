Feature: Excluir Area de Conhecimento Disciplinas

@Academico
@AreaConhecimentoDisciplinas
@ExcluirAreadeConhecimentoSemUsoCadastrando
Scenario: Excluir Area de Conhecimento sem uso
	Given que desejo excluir uma area de conhecimento que nao esta em uso
	And acesso o sistema utilizando o operador 'OPERADOR QA'
	And pesquiso pela area de conhecimento 'area de conhecimento teste qaaa'
	And clico no botao 'Excluir'
	When clico em 'Sim' para confirmar a operacao
	Then e apresentada a mensagem 'Item excluido com sucesso'
	Then removo o operador


## O cenário precisa da automação de disciplinas para ser construído
@Academico
@AreaConhecimentoDisciplinas
@ExcluirAreaConhecimentoEmUsoCadastrando
Scenario: Excluir Area de Conhecimento em uso
	Given que desejo excluir uma area de conhecimento que esta em uso
	And acesso o sistema utilizando o operador 'OPERADOR QA'
	And pesquiso pela area de conhecimento 'area de conhecimento teste qaaa'
	And clico no botao 'Excluir'
	When clico em 'Sim' para confirmar a operacao
	Then e apresentada a mensagem 'Disciplina: Não é possível excluir o registro, pois há vinculo com Disciplinas.'
	Then removo o operador