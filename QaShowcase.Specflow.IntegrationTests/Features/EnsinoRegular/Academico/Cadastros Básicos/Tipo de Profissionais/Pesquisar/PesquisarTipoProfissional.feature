Feature: Pesquisar um tipo de Profissional

@Academico
@TiposProfissionais
@PesquisarTipoProfissionalCadastrando
Scenario: Pesquisar um tipo de profissional

Given que desejo pesquisar um tipo de profissional
And acesso o sistema utilizando o operador 'teste qa pedro a a'
When pesquiso pelo tipo de profissional 'teste'
Then e apresentada somente os tipos de profissional que contenham no nome o termo informado
Then removo o operador