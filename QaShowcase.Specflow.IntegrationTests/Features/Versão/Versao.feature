Feature: Homologação - Diretrizes de Versão

@EnsinoRegular
@Versao

# ============================================
# 1. CONFIGURAÇÕES DO SISTEMA
# Estrutura: EnsinoRegular/Outros/ConfiguracoesDoSistema/Geral/Seguranca.feature
# ============================================

@Seguranca
@AlterarOpcaoDeSenhaPadrao
Scenario: Alterar selecao da senha padrao
	Given que desejo alterar a configuracao da senha padrao
	And seleciono a opcao 'Sim'
	When clico em Gravar
	Then apresenta a mensagem de confirmacao

@Seguranca
@AlterarOpcaoDePrimeiroLogin
Scenario: Alterar selecao de primeiro login
	Given que desejo alterar a configuracao do primeiro login
	And seleciono a opcao 'Nao'
	When clico em Gravar
	Then apresenta a mensagem de confirmacao

# ============================================
# 2. LOGINS - CADASTRAR
# Estrutura: EnsinoRegular/Logins/Cadastrar/CadastroOperador.feature
# ============================================

@Logins
@CadastroOperador
 Scenario: Cadastrar usuario
	Given que desejo cadastrar um operador
	And clico em cadastrar
	And preencho o campo nome 'TESTE VERSAO COMPLETO 123'
	And preencho o campo login 'TESTE VERSAO COMPLETO 123'
	And preencho o campo senha '<DEFAULT_PASSWORD>'
	And habilito o usuario 'sim'
	And seleciono a empresas visiveis para este operador
	And seleciono empresa padrao
	And seleciono as permissoes essenciais
	When clico em 'Gravar'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'

# ============================================
# 2. LOGINS - EDITAR
# Estrutura: EnsinoRegular/Logins/Editar/EditarOperador.feature
# ============================================

@Logins
@EditarOperador
Scenario: Editar Operador cadastrado
	Given que desejo editar Operador
	And pesquiso pelo operador 'TESTE VERSAO COMPLETO'
	And clico em editar
	And altero a descricao incluindo
	When clico em Gravar
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'

@Logins
@EditarOperadorCadastrando
Scenario: Editar Operador
	Given que desejo editar Operador
	And pesquiso pelo operador
	And clico em editar
	And altero a descricao incluindo
	When clico em Gravar
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'

@Logins
@EditarOperadorCadastrando
Scenario: Editar Operador Cancelar
	Given que desejo editar Operador
	And pesquiso pelo operador
	And clico em editar
	And altero a descricao incluindo
	When clico em 'Cancelar'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'

# ============================================
# 2. LOGINS - EXCLUIR
# Estrutura: EnsinoRegular/Logins/Excluir/ExcluirUsuario.feature
# ============================================

@Logins
@ExcluirUsuario
@ExcluirUsuarioCadastrado
Scenario: Excluir usuario cadastrado
	Given que desejo excluir usuario
	And pesquiso pelo 'TESTE VERSAO COMPLETO'
	And clico em Remover
	When seleciono a opcao 'Sim'
	Then e apresentada a mensagem de exclusao 'Item excluído com sucesso.'

@Logins
@ExcluirUsuario
@ExcluirUsuarioCadastrando
Scenario: Excluir usuario
	Given que desejo excluir usuario
	And seleciono o operador
	And clico em Remover
	When seleciono a opcao 'Sim'
	Then e apresentada a mensagem de exclusao 'Item excluído com sucesso.'

# ============================================
# 2. LOGINS - PESQUISAR
# Estrutura: EnsinoRegular/Logins/Pesquisar/PesquisarOperador.feature
# ============================================

@Logins
@PesquisarOperadorInformarNome
Scenario: Pesquisar operador
	Given que desejo pesquisar um operador
	And clico no campo pesquisar
	When digito 'ANA'
	Then o operador e apresentado

@Logins
@PesquisarOperadorCadastrando
Scenario: Pesquisar operador cadastrando
	Given que desejo pesquisar um operador
	And cadastro um operador
	And clico no campo pesquisar
	When digito operador
	Then o operador e apresentado

# ============================================
# 2. LOGINS - VALIDAR SITUACAO
# Estrutura: EnsinoRegular/Logins/ValidarSituacao/ValidarSituacao.feature
# ============================================

@Logins
@ValidarSituacao
Scenario: Validar situacao do operador
	Given que desejo validar a situacao de um operador ativo
	And digito o nome do operador
	When seleciono o filtro 'Ativas'
	Then o operador e apresentado       

# ============================================
# 2. LOGINS - LOGAR OPERADOR
# Estrutura: EnsinoRegular/LoginOperador.feature
# ============================================

@LogarOperador
@LogarSelecionandoOperador
Scenario: Logar selecionando um usuario
	Given que desejo logar como um usuario
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	When clico em 'Entrar'
	Then o sistema apresenta a tela inicial
	#Then removo o operador

@LogarOperador
@LogarInfomandoOperador
Scenario: Logar informando um usuario
	Given que desejo logar como um usuario
	And informo o operador 'TESTE VERSAO COMPLETO'
	When clico em 'Entrar'
	Then o sistema apresenta a tela inicial
	##Then removo o operador

# ============================================
# 3. ACADÊMICO - CADASTROS BÁSICOS - EMPRESAS
# Estrutura: EnsinoRegular/Academico/Empresa/Editar/*.feature
# ============================================

@Academico
@Empresa
Scenario: Editar empresa
	Given que desejo editar uma empresa
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And clico em editar no cadastro de empresa
	And altero o apelido para 'CIN EDUCACIONAL LTDA'
	When clico em gravar
	Then o cadastro e alterado com sucesso
	##Then removo o operador

# ============================================
# 3. ACADÊMICO - CADASTROS BÁSICOS - CIDADES
# Estrutura: EnsinoRegular/Academico/Cidades/Cadastrar/CadastroCidades.feature
# ============================================

@Academico
@Cidades
@CadastrarCidadeComCodigoIbge
Scenario: Cadastrar cidade com codigo IBGE
	Given que desejo realizar o cadastro de cidades
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	And informo o codigo do IBGE '5208707'
	And noto que o nome da cidade 'GOIANIA' e UF 'GO' sao apresentados
	When clico em 'Cadastrar'
	Then e apresentada a mensagem 'Registro cadastrado com sucesso!'
	##Then removo o operador

@Academico
@Cidades
@CadastrarCidadeSemCodigoIbge
Scenario: Cadastrar cidade sem codigo IBGE
	Given que desejo realizar o cadastro de cidades
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	And insiro o nome da cidade 'Cidade teste'
	And seleciono a UF 'GO'
	When clico em 'Cadastrar'
	Then e apresentada a mensagem 'Registro cadastrado com sucesso!'
	##Then removo o operador

# ============================================
# 3. ACADÊMICO - CADASTROS BÁSICOS - PROFESSOR
# Estrutura: EnsinoRegular/Academico/Professor/Cadastrar/CadastroProfessor.feature
# ============================================

@Academico
@Professor
@CadastrarProfessorCamposObrigatorios
Scenario: Cadastro de novo professor
	Given que desejo cadastrar um professor no sistema
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And clico em 'Cadastrar'
	And preencho o nome 'PROFESSOR TESTE AUTOMATIZADO'
	And seleciono o sexo 'M'
	And insiro o nascimento '07/07/1990'
	And seleciono a naturalidade 'BR'
	And seleciono uma formacao 'Ensino Superior (Completo)'
	And informo o CPF '<GERAR_CPF>'
	And seleciono o estado civil 'casado'
	And preencho o CEP '74313380'
	And informo o atributo adicional obrigatorio
	When clico em 'gravar'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'

@Academico
@Professor
@CadastrarProfessorSemFormacao
Scenario: Cadastro de novo professor sem informar a Formacao
	Given que desejo cadastrar um professor no sistema
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And clico em 'Cadastrar'
	And preencho o nome 'PROFESSOR TESTE AUTOMATIZADO'
	And seleciono o sexo 'F'
	And insiro o nascimento '07/07/1990'
	And seleciono a naturalidade 'BR'
	And informo o CPF '<GERAR_CPF>'
	And seleciono o estado civil 'casado'
	And preencho o CEP '74313380'
	And informo o atributo adicional obrigatorio
	When clico em 'gravar'
	Then o botao 'Trabalho' é destacado em vermelho

@Academico
@Professor
@ValidarBotaoCancelarCadastro
Scenario: Cancelar a operação de cadastro de professor
	Given que desejo cadastrar um professor no sistema
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And clico em 'Cadastrar'
	And clico em 'Cancelar'
	When clico em 'Sim'
	Then é apresentada a tela de aterrizagem

@Academico
@Professor
@ValidarAnexoCadastroProfessor
Scenario: Anexar arquivo na professor
	Given que preciso adiconar uma anexo a uma professor
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo professor 'PROFESSOR TESTE JADER'
	And clico em 'Ver anexos'
	And adiciono um anexo do tipo 'pdf'
	And informo o titulo do anexo 'Anexo do tipo PDF'
	When clico em 'Adicionar'
	Then ele é apresentado na grid de anexos

# Estrutura: EnsinoRegular/Academico/Professor/Editar/EditarProfessor.feature

@Academico
@Professor
@EditarProfessorNomeCadastrando
Scenario: Editar o nome de um professor
	Given que desejo editar o nome de um professor no sistema
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo professor 'PROFESSOR TESTE'
	And clico em 'editar'
	And altero o 'PROFESSOR TESTE' do professor para 'PROFESSOR TESTE ALTERADO'
	When clico em 'gravar'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'

@Academico
@Professor
@EditarProfessorCadastrandoERemovendoCEP
Scenario: Editar o nome de um professor removendo dados obrigatorios
	Given que desejo editar o cep de um professor no sistema
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo professor 'PROFESSOR TESTE'
	And clico em editar
	And removo os dados existente para o 'CEP'
	When clico em gravar
	Then o campo 'CEP' é destacado em vermelho

@Academico
@Professor
@ValidarBotãoCancelarEdicao
Scenario: Cancelar a operação de edicao de professor
	Given que preciso editar um professor
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And clico em 'Cadastrar'
	And clico em 'Cancelar'
	When clico em 'Sim'
	Then é apresentada a tela de aterrizagem

# Estrutura: EnsinoRegular/Academico/Professor/Excluir/ExcluirProfessor.feature

@Academico
@Professor
@ExcluirProfessorSemVinculoCadastrando
Scenario: Excluir professor
	Given que preciso excluir um professor
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo professor 'Professor Teste Jader'
	And clico em 'Excluir'
	When clico em 'Sim'
	Then e apresentada a mensagem 'Item excluído com sucesso.'

@Academico
@Professor
@ExcluirProfessorComVinculoCadastrando
Scenario: Excluir professor com vinculo em histórico escolares
	Given que preciso excluir um professor
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo professor 'Professor Teste Jader'
	And clico em 'Excluir'
	When clico em 'Sim'
	Then e apresentada a mensagem 'Horários escolares: O professor possui vínculo com os horários escolares.'

# Estrutura: EnsinoRegular/Academico/Professor/Pesquisar/PesquisarProfessor.feature

@Academico
@Professor
@PesquisarProfessorNomeCadastrando
Scenario: Pesquisar professor
	Given que preciso pesquisar um professor
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	When pesquiso pelo professor 'PROFESSOR TESTE JADER'
	Then é apresentado somente professores que contenham em seu nome 'PROFESSOR TESTE JADER'

# ============================================
# 3. ACADÊMICO - CADASTROS BÁSICOS - TIPOS DE PROFISSIONAIS
# Estrutura: EnsinoRegular/Academico/Cadastros Básicos/Tipo de Profissionais/Cadastrar/*.feature
# ============================================

@Academico
@TiposProfissionais
@CadastrarTipoProfissionais
Scenario: Cadastrar tipos de profissionais
	Given que desejo cadastrar um tipo de profissional
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And informo a descricao 'TIPO DE PROFISSIONAL'
	When clico em 'Cadastrar'
	Then e apresentada a mensagem 'Registro cadastrado com sucesso!'
	##Then removo o operador

@Academico
@TiposProfissionais
@EditarTipoProfissionalCadastrando
Scenario: Editar um tipo de profissional
	Given que desejo editar um tipo de profissional
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo tipo de profissional 'TIPO DE PROFISSIONAL'
	And clico em 'Editar'
	When clico em 'Editar' para confirmar a operacao
	Then e apresentada a mensagem 'Registro editado com sucesso!'
	##Then removo o operador

@Academico
@TiposProfissionais
@ExcluirTipoProfissionalSemUsoCadastrando
Scenario: Excluir um tipo de profissional
	Given que desejo excluir um tipo de profissional que nao esta em uso
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo tipo de profissional 'teste qa jader'
	And clico em 'Excluir'
	When clico em 'Sim'
	Then e apresentada a mensagem 'Item excluído com sucesso'
	#Then removo o operador

@Academico
@TiposProfissionais
@ExcluirTipoProfissionalEmUsoCadastrando
Scenario: Excluir um tipo de profissional em uso
	Given que desejo excluir um tipo de profissional que esta em uso
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo tipo de profissional em uso 'ABCDEF'
	And clico em 'Excluir'
	When clico em 'Sim'
	Then e apresentada a mensagem 'Tipo de profissional: Exclusão não permitida! Existe profissional cadastrado com esse tipo.'
	#Then removo o operador

@Academico
@TiposProfissionais
@PesquisarTipoProfissionalCadastrando
Scenario: Pesquisar um tipo de profissional
	Given que desejo pesquisar um tipo de profissional
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	When pesquiso pelo tipo de profissional 'teste'
	Then e apresentada somente os tipos de profissional que contenham no nome o termo informado
	#Then removo o operador

# ============================================
# 3. ACADÊMICO - CADASTROS BÁSICOS - OUTROS PROFISSIONAIS
# Estrutura: EnsinoRegular/Academico/Cadastros Básicos/Outros Profissionais/Cadastrar/*.feature
# ============================================

@Academico
@OutrosProfissionais
@CadastrarOutrosProfissionaisObrigatorio
Scenario: Cadastrar outros profissionais somente com dados obrigatorios
	Given que desejo cadastrar um profissional que nao seja um professor
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And informo o nome 'nome_profissional'
	And informo a data de nascimento
	And informo a naturalidade
	And seleciono o sexo
	And informo a nacionalidade
	And seleciono a etnia
	And seleciono o tipo de profissional 'tipo_profissional'
	And informo o endereco
	And preencho os atributos adicionais obrigatorios
	When clico em 'Gravar'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
	#Then removo o operador

@Academico
@OutrosProfissionais
@EditarNomeOutrosProfissionaisCadastrando
Scenario: Editar nome do profissional
	Given que desejo alterar o nome de um profissional que nao seja um professor
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo profissional 'TESTE FEATURE EDITAR NOME OUTROS PROFISSIONAIS CADASTRANDO'
	And clico em 'Editar'
	And altero o nome do profissional para 'NOME ALTERADOOOOO'
	When clico em 'Gravar'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
	#Then removo o operador

@Academico
@OutrosProfissionais
@AlterarSituacaoOutrosProfissionaisCadastrando
 Scenario: Alterar situacao do profissional
	Given que desejo alterar a situacao de um profissional
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo profissional 'TESTE FEATURE ALTERAR SITUACAO'
	And clico em 'Editar'
	And altero a situacao para 'Inativo'
	When clico em 'Gravar'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
	#Then removo o operador

@Academico
@OutrosProfissionais
@ExcluirOutrosProfissionaisSemVinculosCadastrando
Scenario: Excluir um profissional
	Given que desejo excluir um profissional
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo profissional 'nome_profissional'
	And clico em 'Excluir'
	When confirmo a operacao
	Then e apresentada a mensagem 'Item excluído com sucesso.'
	#Then removo o operador

@Academico
@OutrosProfissionais
@ExcluirOutrosProfissionaisComVinculosCadastrando
Scenario: Excluir um profissional Vinculado
	Given que desejo excluir um profissional
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo profissional 'JADER TESTING LANCAMENTO CAIXA'
	And clico em 'Excluir'
	When confirmo a operacao
	Then e apresentada a mensagem 'Lançamentos: Não é possível excluir profissional, pois o mesmo possui registros em caixa.'
	#Then removo o operador

@Academico
@OutrosProfissionais
@PesquisarOutrosProfissionaisCadastrando
Scenario: Pesquisar por um Profissional
	Given que desejo localizar um profissional
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	When pesquiso pelo profissional 'nome'
	Then e apresentada somente profissionais que contenham o texto informado na pesquisa
	#Then removo o operador

@Academico
@OutrosProfissionais
@FiltrarGridOutrosProfissionaisSemVinculosCadastrando
Scenario: Filtrar apresentacao de um profissional
	Given que desejo localizar um profissional
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	When altero o filtro de situacao para 'inativo'
	Then e apresentada somente profissionais 'inativo'
	#Then removo o operador

# ============================================
# 3. ACADÊMICO - SÉRIE, CURSOS, TURMAS E DISCIPLINAS - DISCIPLINAS
# Estrutura: EnsinoRegular/Academico/Disciplina/Cadastrar/CadastroDisciplina.feature
# ============================================

@Academico
@Disciplinas
@CadastrarDisciplinasObrigatorio
Scenario: Cadastrar disciplina
	Given que desejo cadastrar uma disciplina comum
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
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
	#Then removo o operador

# Estrutura: EnsinoRegular/Academico/Disciplina/Editar/EditarDisciplina.feature

@Academico
@Disciplinas
@EditarNomeDisciplinasCadastrando
Scenario: Editar nome da disciplina
	Given que desejo alterar o nome de uma disciplina
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pela disciplina 'DISCIPLINA TESTE QA'
	And clico em 'Editar'
	And altero o nome da disciplina para 'MATEMATICA ALTERADA'
	When clico em 'Gravar'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
	#Then removo o operador

@Academico
@Disciplinas
@AlterarSituacaoDisciplinasCadastrando
Scenario: Alterar situacao da disciplina
	Given que desejo alterar a situacao de uma disciplina
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pela disciplina 'MATEMATICA ALTERADA'
	And clico em 'Editar'
	And altero a situacao para 'Inativo'
	When clico em 'Gravar'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
	#Then removo o operador

# Estrutura: EnsinoRegular/Academico/Disciplina/Excluir/ExcluirDisciplina.feature

@Academico
@Disciplinas
@ExcluirDisciplinasSemVinculosCadastrando
Scenario: Excluir uma disciplina
	Given que desejo excluir uma disciplina
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pela disciplina 'DISCIPLINA TESTE QA'
	And clico em 'Excluir'
	When confirmo a operacao clicando em 'Sim'
	Then e apresentada a mensagem 'Item excluido com sucesso.'

@Academico
@Disciplinas
@ExcluirDisciplinasComVinculosCadastrando
Scenario: Excluir uma disciplina vinculada
	Given que desejo excluir uma disciplina
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pela disciplina 'TRILHAS - APROF. EM FISICA'
	And clico em 'Excluir'
	When confirmo a operacao clicando em 'Sim'
	Then e apresentada a mensagem 'A disciplina nao pode ser excluida pois ja esta sendo utilizada por alguma das funcoes ou conceitos:'

# Estrutura: EnsinoRegular/Academico/Disciplina/Pesquisar/PesquisarDisciplina.feature

@Academico
@Disciplinas
@PesquisarDisciplinasCadastrando
Scenario: Pesquisar por uma disciplina
	Given que desejo localizar uma disciplina
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	When pesquiso pela disciplina 'DISCIPLINA TESTE QA'
	Then e apresentada somente disciplinas que contenham o texto informado na pesquisa
	#Then removo o operador

@Academico
@Disciplinas
@FiltrarGridDisciplinasCadastrando
Scenario: Filtrar apresentação de uma disciplina
	Given que desejo localizar uma disciplina
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	When altero o filtro de situacao para 'Inativas'
	Then e apresentada somente disciplinas 'Inativas'
	#Then removo o operador

# ============================================
# 3. ACADÊMICO - SÉRIE, CURSOS, TURMAS E DISCIPLINAS - ÁREA DE CONHECIMENTO
# Estrutura: EnsinoRegular/Academico/AreaDeConhecimento/Cadastrar/CadastroAreaConhecimento.feature
# ============================================

@Academico
@AreaConhecimentoDisciplinas
@CadastrarAreaConhecimento
Scenario: Cadastrar Area de Conhecimento
	Given que desejo cadastrar uma area de conhecimento
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And informo a descricao 'area de conhecimento teste qa'
	When clico em 'Cadastrar'
	Then e apresentada a mensagem 'Registro cadastrado com sucesso!'
	#Then removo o operador

# Estrutura: EnsinoRegular/Academico/AreaDeConhecimento/Editar/EditarAreaConhecimento.feature

@Academico
@AreaConhecimentoDisciplinas
@EditarAreaConhecimentoCadastrando
Scenario: Editar Area de Conhecimento
	Given que desejo editar uma area de conhecimento
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pela area de conhecimento 'area de conhecimento teste qa'
	And clico no botao 'Icone Editar'
	When clico em 'Editar' para confirmar a operacao
	Then e apresentada a mensagem 'Registro editado com sucesso!'
	#Then removo o operador

# Estrutura: EnsinoRegular/Academico/AreaDeConhecimento/Excluir/ExcluirAreaConhecimento.feature

@Academico
@AreaConhecimentoDisciplinas
@ExcluirAreadeConhecimentoSemUsoCadastrando
Scenario: Excluir Area de Conhecimento sem uso
	Given que desejo excluir uma area de conhecimento que nao esta em uso
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pela area de conhecimento 'area de conhecimento teste qaaa'
	And clico no botao 'Excluir'
	When clico em 'Sim' para confirmar a operacao
	Then e apresentada a mensagem 'Item excluido com sucesso'
	#Then removo o operador

@Academico
@AreaConhecimentoDisciplinas
@ExcluirAreaConhecimentoEmUsoCadastrando
Scenario: Excluir Area de Conhecimento em uso
	Given que desejo excluir uma area de conhecimento que esta em uso
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pela area de conhecimento 'area de conhecimento teste qaaa'
	And clico no botao 'Excluir'
	When clico em 'Sim' para confirmar a operacao
	Then e apresentada a mensagem 'Disciplina: Não é possível excluir o registro, pois há vinculo com Disciplinas.'
	#Then removo o operador

# Estrutura: EnsinoRegular/Academico/AreaDeConhecimento/Pesquisar/PesquisaAreaConhecimento.feature

@Academico
@AreaConhecimentoDisciplinas
@PesquisarAreaConhecimentoCadastrando
Scenario: Pesquisar Area de Conhecimento
	Given que desejo pesquisar uma area de conhecimento
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	When pesquiso pela area de conhecimento 'area de conhecimento teste qaaa'
	Then e apresentada somente as areas de conhecimentos que contenham no nome o termo informado
	#Then removo o operador

# ============================================
# 3. ACADÊMICO - SÉRIE, CURSOS, TURMAS E DISCIPLINAS - TURMAS
# Estrutura: EnsinoRegular/Academico/Turma/Cadastrar/CadastroTurmas.feature
# ============================================

@Academico
@Turmas
@CadastrarTurmas
Scenario: Cadastrar uma turma
	Given que desejo cadastrar uma turma
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And clico para gerar um codigo
	And informo a descricao 'TURMA TESTE'
	When clico em 'Cadastrar'
	Then e apresentada a mensagem 'Registro cadastrado com sucesso!'
	#Then removo o operador

# Estrutura: EnsinoRegular/Academico/Turma/Editar/EditarTurmas.feature

@Academico
@Turmas
@EditarTurmasCadastrando
Scenario: Editar uma turma
	Given que desejo editar um turma
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pela turma 'TURMA TESTE'
	And clico no botao 'Editar'
	And altero a descricao para 'TESTE AAAA'
	When clico em 'Editar'
	Then e apresentada a mensagem 'Registro editado com sucesso!'
	#Then removo o operador

# Estrutura: EnsinoRegular/Academico/Turma/Excluir/ExcluirTurmas.feature

@Academico
@Turmas
@ExcluirTurmaSemUsoCadastrando
Scenario: Excluir uma turma
	Given que desejo excluir uma turma
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pela turma 'MARISA'
	And clico no botao 'Excluir'
	When clico em 'Sim'
	Then e apresentada a mensagem 'Item excluido com sucesso'
	#Then removo o operador

@Academico
@Turmas
@ExcluirTurmaEmUsoCadastrando
Scenario: Excluir Turma Em Uso Cadastrando
	Given que desejo excluir uma turma
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pela turma 'EF2VB_4_1_4_19'
	And clico no botao 'Excluir'
	When clico em 'Sim'
	Then e apresentada a mensagem 'Turmas: Nao e possivel excluir a turma, pois ha um coordenador do portal do professor associado a ela.Turmas: Nao e possivel excluir a turma, pois ha vinculos com series.'
	#Then removo o operador

# Estrutura: EnsinoRegular/Academico/Turma/Pesquisar/PesquisaTurmas.feature

@Academico
@Turmas
@PesquisarTurmasCadastrando
Scenario: Pesquisar por uma turma
	Given que desejo pesquisar uma turma
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	When pesquiso pela turma 'TTTTTTTTT'
	Then e apresentada somente as turmas que contenham no nome o termo informado
	#Then removo o operador

# ============================================
# 3. ACADÊMICO - SÉRIE, CURSOS, TURMAS E DISCIPLINAS - SÉRIE
# Estrutura: EnsinoRegular/Academico/Serie/Cadastrar/CadastrarSerie.feature
# ============================================

@Academico
@Serie
@SerieCadastrar
Scenario: Cadastro de serie
	Given que desejo cadastrar uma serie
	And clico em cadastrar a serie
	And informo a descricao "Serie  Data_Atual "
	And informo a carga horaria
	When clico em gravar o cadastro da serie
	Then o cadastro da serie eh realizado com sucesso

@Academico
@Serie
@SerieCadastrar
Scenario: Cadastro de serie informando disciplinas
	Given que desejo cadastrar uma serie
	And clico em cadastrar a serie
	And informo a descricao "Serie  Data_Atual "
	And informo a carga horaria
	And adiciono disciplinas no cadastro da serie
	When clico em gravar o cadastro da serie
	Then o cadastro da serie eh realizado com sucesso

# Estrutura: EnsinoRegular/Academico/Serie/Editar/EditarSerie.feature

@Academico
@Serie
@SerieEditar
Scenario: Editar serie
	Given que desejo editar o cadastro uma serie
	And clico em editar a serie
	And edito a descricao da serie
	When clico em gravar o cadastro da serie
	Then serie eh editada com sucesso

# Estrutura: EnsinoRegular/Academico/Serie/Excluir/ExcluirSerie.feature

@Academico
@Serie
@SerieExcluir
Scenario: Exclusao de serie
	Given que desejo excluir uma serie
	And pesquiso a serie que desejo excluir
	And clico em excluir a serie
	When clico em sim, para excluir a serie
	Then serie eh excluida com sucesso

# ============================================
# 3. ACADÊMICO - SÉRIE, CURSOS, TURMAS E DISCIPLINAS - MONTAGEM DE TURMA
# Estrutura: EnsinoRegular/Academico/MontagemDeTurmas/Cadastrar/CadastrarMontagemDeTurma.feature
# ============================================

@Academico
@MontagemDeTurma
@MontagemDeTurmaCadastrar
Scenario: Cadastrar montagem de turma
	Given Que desejo cadastrar uma montagem de turma
	And clico em cadastrar
	And seleciono a serie
	And seleciono a turma
	And informo o turno
	And informo a lotacao
	When clico em gravar
	Then o cadastro eh realizado com sucesso

# Estrutura: EnsinoRegular/Academico/MontagemDeTurmas/Editar/EditarMontagemDeTurma.feature

@Academico
@MontagemDeTurma
@MontagemDeTurmaEditar
Scenario: Editar montagem de turma
	Given Que desejo editar uma montagem de turma
	And seleciono a montagem de turma que desejo editar
	And informo a nova lotacao
	When clico em gravar
	Then a edicao eh realizada com sucesso

# Estrutura: EnsinoRegular/Academico/MontagemDeTurmas/Excluir/ExcluirMontagemDeTurma.feature

@Academico
@MontagemDeTurma
@MontagemDeTurmaExcluir
Scenario: Excluir montagem de turma
	Given Que desejo excluir uma montagem de turma
	And clico em excluir
	When clico em sim
	Then a exclusao eh realizada com sucesso

# ============================================
# 3. ACADÊMICO - ALUNOS - FICHA DO ALUNO
# Estrutura: EnsinoRegular/Academico/FichaAluno/Cadastrar/CadastroFichaAluno.feature
# ============================================

@Academico
@FichaAluno
@CadastrarFichaAlunoCamposObrigatorios
Scenario: Cadastro de nova ficha de aluno
	Given que preciso cadastrar um novo aluno
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And clico em 'Cadastrar'
	And preecho o Nome civil 'ALUNO TESTE QA'
	And seleciono o sexo 'Masculino'
	And insiro o nascimento '01/01/2000'
	And seleciono a naturalidade 'ABADIA DE GOIAS'
	And preencho o CEP '74375-230'
	And preencho o CPF '<GERAR_CPF>'
	And preencho os atributos adicionais obrigatorios
	When clico em 'Gravar'
	Then e apresentada a mensagem 'Cadastro realizado, deseja ir para a tela de matrícula?'
	#Then removo o operador

@Academico
@FichaAluno
@CadastrarFichaAlunoSemCEP
Scenario: Cadastro de nova ficha de aluno sem informar CEP
	Given que preciso cadastrar um novo aluno
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And clico em 'Cadastrar'
	And preecho o Nome civil 'ALUNO TESTE QA'
	And seleciono o sexo 'Masculino'
	And insiro o nascimento '01/01/2000'
	And seleciono a naturalidade 'ABADIA DE GOIAS'
	And preencho o CPF '<GERAR_CPF>'
	When clico em 'Gravar'
	Then os campos obrigatorios pendentes do CEP sao destacadados
	#Then removo o operador

@Academico
@FichaAluno
@ValidarBotaoCancelarCadastro
Scenario: Cancelar a operação de cadastro de ficha do aluno
	Given que preciso cadastrar um novo aluno
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And clico em 'Cadastrar'
	And clico em 'Cancelar'
	When clico em 'Sim' para confirmar a operacao
	Then e apresentada a tela de aterrissagem
	#Then removo o operador

@Academico
@FichaAluno
@ValidarAnexoCadastro
Scenario: Anexar arquivo na ficha do aluno
	Given que preciso adiconar uma anexo a uma ficha do aluno
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And clico em anexo na ficha do aluno 'ALUNO TESTE QA'
	And insiro o titulo 'anexo teste'
	And seleciono um anexo
	When clico em 'Adicionar'
	Then o anexo e apresentado na grid de anexos
	#Then removo o operador

# Estrutura: EnsinoRegular/Academico/FichaAluno/Editar/EditarFichaAluno.feature

@Academico
@FichaAluno
@EditarFichaAlunoNomeCadastrando
Scenario: Editar o nome de um aluno
	Given que desejo editar um aluno
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo aluno 'ALUNO TESTE QA'
	And clico em 'Editar'
	And altero o nome do aluno para 'ALUNO TESTE NOME ALTERADO'
	When pesquiso pelo aluno alterado
	Then o aluno e apresentado na grid
	#Then removo o operador

@Academico
@FichaAluno
@EditarFichaAlunoCadastrandoERemovendoCEP
Scenario: Editar ficha do aluno cadastrando e removendo CEP
	Given que desejo editar um aluno
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo aluno 'ALUNO TESTE QA'
	And clico em 'Editar'
	And removo os dados existente para o 'CEP'
	When clico em 'Gravar'
	Then os campos obrigatorios pendentes do 'CEP' sao destacadados
	#Then removo o operador

@Academico
@FichaAluno
@ValidarBotaoCancelarEdicao
Scenario: Cancelar a operacao de edicao de ficha do aluno
	Given que desejo editar um aluno
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo aluno 'ALUNO TESTE QA'
	And clico em 'Editar'
	And clico em 'Cancelar'
	When clico em 'Sim'
	Then e apresentada a tela de aterrissagem
	#Then removo o operador

# Estrutura: EnsinoRegular/Academico/FichaAluno/Excluir/ExcluirFichaAluno.feature

@Academico
@FichaAluno
@ExcluirFichaAlunoSemVinculoCadastrando
Scenario: Excluir ficha do aluno
	Given que preciso excluir um aluno
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo aluno 'ALUNO TESTE QA'
	And clico em 'Remover'
	When clico em 'Sim'
	Then e apresentada a mensagem 'Item excluído com sucesso.'
	#Then removo o operador

@Academico
@FichaAluno
@ExcluirFichaAlunoComVinculoFinanceiroCadastrando
Scenario: Excluir ficha do aluno com vinculo financeiro
	Given que preciso excluir um aluno
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo aluno 'ALUNO TESTE QA'
	And o aluno possui vinculos financeiros
	And clico em 'Excluir'
	When clico em 'Sim'
	Then e apresentada a mensagem 'Existe lançamento no caixa vinculado a esta pessoa, portanto a mesma não poderá ser excluída.'
	#Then removo o operador

@Academico
@FichaAluno
@ExcluirFichaAlunoComMatriculaAtivaCadastrando
Scenario: Excluir ficha do aluno com matricula ativa
	Given que preciso excluir um aluno
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo aluno 'ALUNO TESTE QA'
	And o aluno possui matricula ativa
	And clico em 'Excluir'
	When clico em 'Sim'
	Then e apresentada a mensagem 'Esta pessoa já foi matriculada, portanto não poderá ser excluída.'
	#Then removo o operador

# Estrutura: EnsinoRegular/Academico/FichaAluno/Pesquisar/PesquisarFichaAluno.feature

@Academico
@FichaAluno
@PesquisarFichaAlunoNomeCadastrando
Scenario: Pesquisar ficha do aluno por nome cadastrando
	Given que preciso pesquisar um aluno
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And digito em 'Nome ou número de matrícula' o nome do aluno 'ALUNO TESTE QA'
	When pressiono a tecla 'Enter'
	Then e apresentado somente alunos que contenham em seu nome o nome digitado
	#Then removo o operador

@Academico
@FichaAluno
@PesquisarPorNomeRespFichaAlunoCadastrando
Scenario: Pesquisar ficha do aluno pelo nome do responsavel
	Given que preciso pesquisar um aluno
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso por responsavel o aluno 'ALUNO TESTE QA'
	When pressiono a tecla 'Enter'
	Then e apresentado somente alunos que contenham nome do responsavel digitado, vinculado a sua ficha do aluno
	#Then removo o operador

@Academico
@FichaAluno
@PesquisarAlunoPorIdCadastrando
Scenario: Pesquisar aluno por ID cadastrando
	Given que preciso pesquisar um aluno
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	When pesquiso o ID do aluno 'ALUNO TESTE QA'
	Then o aluno e apresentado na grid
	#Then removo o operador

# ============================================
# 3. ACADÊMICO - ALUNOS - RESPONSÁVEIS
# Estrutura: EnsinoRegular/Academico/Responsaveis/Cadastrar/CadastroResponsavel.feature
# ============================================

@Academico
@Responsavel
@CadastrarResponsavelCamposObrigatorios
Scenario: Cadastro de um responsavel
	Given que desejo cadastrar um responsavel
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And clico em 'Cadastrar'
	And informo o nome 'Responsável teste qa'
	And informo a data de nascimento '01/01/1980'
	And seleciono o sexo 'Masculino'
	And informo o estado civil 'Casado(a)'
	And informo o cep '74375-230'
	And informo o cpf
	And preencho os atributos adicionais obrigatorios
	When clico em 'Gravar'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'

@Academico
@Responsavel
@ValidarBotaoCancelarCadastroResponsavel
Scenario: Cancelar a operação de cadastro de responsavel
	Given que desejo cadastrar um responsavel
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And clico em 'Cadastrar'
	And clico em 'Cancelar'
	When clico em 'Sim'
	Then e apresentada a tela de aterrissagem

# Estrutura: EnsinoRegular/Academico/Responsaveis/Editar/EditarResponsavel.feature

@Academico
@Responsavel
@EditarResponsavelNomeCadastrando
Scenario: Editar responsavel cadastrando
	Given que desejo editar um responsavel
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo responsavel 'Responsavel QA'
	And clico em 'Editar'
	And altero o nome do responsavel para 'Responsavel QA editado'
	When clico em 'Gravar'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'

@Academico
@Responsavel
@ValidarBotaoCancelarEditarResponsavel
Scenario: Cancelar edicao de responsavel
	Given que desejo editar um responsavel
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo responsavel 'Responsavel QA'
	And clico em 'Editar'
	And clico em 'Cancelar'
	When clico em 'Sim'
	Then e apresentada a tela de aterrissagem

# Estrutura: EnsinoRegular/Academico/Responsaveis/Excluir/ExcluirResponsavel.feature

@Academico
@Responsavel
@ExcluirResponsavelCadastrando
Scenario: Excluir dados de um responsavel
	Given que desejo excluir um responsavel
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo responsavel 'Responsavel QA'
	And clico em 'Remover'
	When clico em 'Sim'
	Then e apresentada a mensagem 'Item excluído com sucesso.'

@Academico
@Responsavel
@ExcluirResponsavelCadastrandoVinculado
Scenario: Excluir dados de um responsavel cadastrando
	Given que desejo excluir um responsavel
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And o responsavel 'Responsavel QA' possui vinculo com o aluno 'aluno teste'
	And pesquiso o responsavel 'Responsavel QA'
	And clico em 'Remover'
	When clico em 'Sim'
	Then e apresentada a mensagem 'Responsável não pode ser excluído, pois existe relação com as seguinte(s) matrícula(s):'

# ============================================
# 3. ACADÊMICO - ALUNOS - ATRIBUTOS ADICIONAIS
# Estrutura: EnsinoRegular/Academico/Atributos Adicionais/Cadastrar/CadastroAtributosAdicionais.feature
# ============================================

@Academico
@AtributoAdicional
@CadastrarAtributoAdicional.Conc.Aluno.Tp.Texto
Scenario: Cadastrar atributo adicional conceito aluno e tipo texto
	Given que desejo cadastrar um atributo adicional
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	And clico em 'Cadastrar'
	And insiro a descricao 'Atributo teste Aluno'
	And seleciono a obrigatoriedade 'Não'
	And seleciono a unicidade 'Não'
	And seleciono o conceito 'Aluno'
	And seleciono o tipo 'Texto'
	And defino o tamanho maximo '450'
	When clico em 'Gravar'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
	#Then removo o operador

@Academico
@AtributoAdicional
@CadastrarAtributoAdicional.Conc.FichaMedica.Tp.Texto
Scenario: Cadadastrar atributo adicional conceito Ficha Medica e tipo Texto
	Given que desejo cadastrar um atributo adicional
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	And clico em 'Cadastrar'
	And informo a descricao 'Atributo teste Ficha Medica'
	And seleciono a obrigatoriedade 'Não'
	And seleciono a unicidade 'Não'
	And seleciono o conceito 'Ficha Médica'
	And seleciono o tipo 'Texto'
	And defino o tamanho maximo '450'
	When clico em 'Gravar'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
	#Then removo o operador

@Academico
@AtributoAdicional
@CadastrarAtributoAdicional.Conc.Responsavel.Tp.Texto
Scenario: Cadadastrar atributo adicional conceito Responsavel e tipo Texto
	Given que desejo cadastrar um atributo adicional
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	And clico em 'Cadastrar'
	And informo a descricao 'Atributo teste Responsavel'
	And seleciono a obrigatoriedade 'Não'
	And seleciono a unicidade 'Não'
	And seleciono o conceito 'Responsável'
	And seleciono o tipo 'Texto'
	And defino o tamanho maximo '450'
	When clico em 'Gravar'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
	#Then removo o operador

@Academico
@AtributoAdicional
@CadastrarAtributoAdicional.Conc.Aluno.Tp.Texto.Unico
Scenario: Cadadastrar atributo adicional conceito Aluno e tipo Texto Unico
	Given que desejo cadastrar um atributo adicional
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	And clico em 'Cadastrar'
	And informo a descricao 'Atributo teste Aluno Unico'
	And seleciono a obrigatoriedade 'Não'
	And seleciono a unicidade 'Sim'
	And seleciono o conceito 'Aluno'
	And seleciono o tipo 'Texto'
	And defino o tamanho maximo '450'
	When clico em 'Gravar'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
	#Then removo o operador

@Academico
@AtributoAdicional
@CadastrarAtributoAdicional.Conc.Aluno.Tp.Texto.Obrigatorio
Scenario: Cadadastrar atributo adicional conceito Aluno e tipo Texto Obrigatorio
	Given que desejo cadastrar um atributo adicional
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	And clico em 'Cadastrar'
	And informo a descricao 'Atributo teste Aluno Obrigatorio'
	And seleciono a obrigatoriedade 'Sim'
	And seleciono a unicidade 'Não'
	And seleciono o conceito 'Aluno'
	And seleciono o tipo 'Texto'
	And defino o tamanho maximo '450'
	When clico em 'Gravar'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
	#Then removo o operador

@Academico
@AtributoAdicional
@CadastrarAtributoAdicional.Conc.Aluno.Tp.Texto.Obrigatório.Unico
Scenario: Cadadastrar atributo adicional conceito Aluno e tipo Texto Obrigatorio Unico
	Given que desejo cadastrar um atributo adicional
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	And clico em 'Cadastrar'
	And informo a descricao 'Atributo teste Aluno Obrigatorio Unico'
	And seleciono a obrigatoriedade 'Sim'
	And seleciono a unicidade 'Sim'
	And seleciono o conceito 'Aluno'
	And seleciono o tipo 'Texto'
	And defino o tamanho maximo '450'
	When clico em 'Gravar'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
	#Then removo o operador

@Academico
@AtributoAdicional
@CadastrarAtributoAdicional.Conc.Aluno.Tp.Lista
Scenario: Cadastrar atributo adicional conceito aluno e tipo lista
	Given que desejo cadastrar um atributo adicional
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	And clico em 'Cadastrar'
	And informo a descricao 'Atributo teste Aluno Lista'
	And seleciono a obrigatoriedade 'Não'
	And seleciono a unicidade 'Não'
	And seleciono o conceito 'Aluno'
	And seleciono o tipo 'Lista'
	And adiciono '3' itens na lista
	When clico em 'Gravar'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
	#Then removo o operador

@Academico
@AtributoAdicional
@CadastrarAtributoAdicional.Conc.Aluno.Tp.Lista.Obrigatorio
Scenario: Cadastrar atributo adicional conceito aluno e tipo lista obrigatorio
	Given que desejo cadastrar um atributo adicional
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	And clico em 'Cadastrar'
	And informo a descricao 'Atributo teste Aluno Lista Obrigatorio'
	And seleciono a obrigatoriedade 'Sim'
	And seleciono a unicidade 'Não'
	And seleciono o conceito 'Aluno'
	And seleciono o tipo 'Lista'
	And adiciono '3' itens na lista
	When clico em 'Gravar'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
	#Then removo o operador

@Academico
@AtributoAdicional
@CadastrarAtributoAdicional.Conc.Aluno.Tp.Lista.Unico
Scenario: Cadastrar atributo adicional conceito aluno e tipo lista unico
	Given que desejo cadastrar um atributo adicional
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	And clico em 'Cadastrar'
	And informo a descricao 'Atributo teste Aluno Lista Unico'
	And seleciono a obrigatoriedade 'Não'
	And seleciono a unicidade 'Sim'
	And seleciono o conceito 'Aluno'
	And seleciono o tipo 'Lista'
	And adiciono '3' itens na lista
	When clico em 'Gravar'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
	#Then removo o operador

@Academico
@AtributoAdicional
@CadastrarAtributoAdicional.Conc.Aluno.Tp.Lista.Obrigatorio.Unico
Scenario: Cadastrar atributo adicional conceito aluno e tipo lista obrigatorio unico
	Given que desejo cadastrar um atributo adicional
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	And clico em 'Cadastrar'
	And informo a descricao 'Atributo teste Aluno Lista Obrigatorio Unico'
	And seleciono a obrigatoriedade 'Sim'
	And seleciono a unicidade 'Sim'
	And seleciono o conceito 'Aluno'
	And seleciono o tipo 'Lista'
	And adiciono '3' itens na lista
	When clico em 'Gravar'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
	#Then removo o operador

@Academico
@AtributoAdicional
@EditarAtributoAdicionalCadastrando
Scenario: Editar atributo adicional cadastrando
	Given que desejo editar um atributo adicional
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo atributo adicional 'Atributo teste Editar'
	And clico em 'Editar'
	And altero a descricao para 'Atributo editado!'
	When clico em 'Gravar'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
	#Then removo o operador

@Academico
@AtributoAdicional
@EditarAtributoAdicionalQuePossuiResposta
Scenario: Editar atributo adicional que possui resposta
	Given que desejo editar um atributo que possui resposta
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo atributo adicional com resposta 'Atributo teste com resposta'
	And clico em 'Editar'
	And altero a descricao para 'Atributo teste com resposta Editado'
	When clico em 'Gravar'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
	#Then removo o operador

@Academico
@AtributoAdicional
@ExcluirAtributoAdicionalCadastrando
Scenario: Excluir atributo adicional cadastrando
	Given que desejo excluir um atributo adicional
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo atributo adicional 'Atributo teste Excluir'
	And clico em 'Remover'
	When clico em 'Sim'
	Then e apresentada a mensagem 'Item excluído com sucesso.'
	#Then removo o operador

@Academico
@AtributoAdicional
@ExcluirAtributoAdicionalQuePossuiResposta
Scenario: Excluir atributo adicional que possui resposta
	Given que desejo excluir um atributo adicional
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo atributo adicional com resposta 'Atributo teste respondido'
	And clico em 'Remover'
	When clico em 'Sim'
	Then e apresentada a mensagem 'Codigo: Atributo adicional está em uso.'
	#Then removo o operador

@Academico
@AtributoAdicional
@PesquisarAtributoAdicionalCadastrando
Scenario: Pesquisar atributo adicional cadastrando
	Given que desejo pesquisar um atributo adicional
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	When pesquiso pelo atributo adicional 'Atributo teste Pesquisar'
	Then e apresentado o atributo adicinal pesquisado
	#Then removo o operador

# ============================================
# 3. ACADÊMICO - MATRÍCULAS - MATRÍCULA INDIVIDUAL
# Estrutura: EnsinoRegular/Academico/MatriculaIndividual/Matricular/MatricularMatriculaIndividual.feature
# ============================================

#@Academico
#@MatriculaIndividual
#Scenario: Matricular um aluno sem financeiro
#	Given que desejo matricular um aluno
#	And seleciono um aluno que nao possui matricula
#	And seleciono uma serie para matricular
#	And marco o aluno como proprio responsavel
#	When clico em gravar matricula
#	Then a matricula e realizada

#@Academico
#@MatriculaIndividual
#Scenario: Matricular um aluno com financeiro
#	Given que desejo matricular um aluno gerando financeiro
#	And seleciono um aluno que nao possui matricula ativa
#	And seleciono uma serie para matricular
#	And marco o aluno como proprio responsavel
#	And preencho os dados das duplicatas
#	When clico em gravar matricula
#	Then a matricula e realizada

#@Academico
#@MatriculaIndividual
#Scenario: Matricular um aluno com financeiro por plano de pagamento
#	Given que desejo matricular um aluno com financeiro por plano de pagamento
#	And seleciono um aluno que nao possui matricula no ano atual
#	And seleciono uma serie para matricular
#	And marco o aluno como proprio responsavel
#	And seleciono um plano de pagamento
#	When clico em gravar matricula
#	Then a matricula e realizada

# Estrutura: EnsinoRegular/Academico/MatriculaIndividual/Cancelar/CancelarMatriculaIndividual.feature

@Academico
@MatriculaIndividual
Scenario: Cancelar uma matrícula ativa
	Given que desejo cancelar a matricula de um aluno
	And seleciono um aluno com a matrícula ativa
	And clico para cancelar a matricula
	And confirmo as consequencias
	And insiro uma data de cancelamento
	And confirmo o cancelamento
	And marco sim para cancelar
	When clico em voltar
	Then noto que a matricula esta cancelada

# Estrutura: EnsinoRegular/Academico/MatriculaIndividual/Excluir/ExcluirMatriculaIndividual.feature

@Academico
@MatriculaIndividual
Scenario: Excluir uma matrícula
	Given que acesso a tela de matricula
	And seleciono um aluno
	And clico para excluir a matricula
	When confirmo a exclusão da matrícula
	Then a matricula e excluida com sucesso

@Academico
@MatriculaIndividual
Scenario: Excluir uma matrícula com vínculos
	Given que acesso a tela de matricula
	And seleciono um aluno
	And clico para excluir a matricula
	When confirmo a exclusão da matrícula
	Then e apresentada a mensagem que a matricula nao pode ser excluida

# ============================================
# 3. ACADÊMICO - MATRÍCULAS - REMANEJAMENTO INDIVIDUAL
# Estrutura: EnsinoRegular/Academico/RemanejamentoIndividual/1Remanejar/Remanejar.feature
# ============================================

@Academico
@RemanejamentoIndividual
Scenario: Remanejamento Individual
	Given que desejo remanejar um aluno
	And seleciono o aluno a ser remanejado
	And seleciono a serie/turma para remanejamento
	And clico em carregar remanejamento
	And E configuro as opcoes de remanejamento
	When clico em gravar remanejamento
	Then o remanejamento eh realizado com sucesso

# Estrutura: EnsinoRegular/Academico/RemanejamentoIndividual/2Estornar/EstornarRemanejamento.feature

@Academico
@RemanejamentoIndividual
Scenario: Estornar Remanejamento Individual
	Given que desejo estornar o remanejamento de um aluno
	And seleciono o aluno a ser estornado o remanejamento
	And clico em estornar o remanejamento
	When clico em sim
	Then o estorno do remanejamento eh realizado com sucesso

# Estrutura: EnsinoRegular/Academico/RemanejamentoIndividual/3Financeiro/FinanceiroRemanejamento.feature

@Academico
@RemanejamentoIndividual
Scenario: Atalho financeiro na tela de remanejamento individual
	Given que desejo ir para a tela de geracao de duplicatas
	When clico no botao financeiro, na tela de remanejamento individual
	Then a tela de geracao de duplicatas eh aberta

# ============================================
# 3. ACADÊMICO - AVALIAÇÕES - PROGRAMAÇÃO DE AVALIAÇÕES
# Estrutura: EnsinoRegular/Academico/ProgramacaoDeAvaliacoes/Cadastrar/CadastrarProgramacaoDeAvaliacoes.feature
# ============================================

@Academico
@ProgramacaoDeAvaliacoes
@ProgramacaoDeAvaliacoesCadastrar
Scenario: Cadastrar programacao de avaliacoes
	Given Que desejo cadastrar uma programacao de avaliacoes
	And clico em novo
	And seleciono a serie
	And seleciono a turma
	And informo a etapa
	And informo a tabela de avaliacao
	And informo a disciplina
	And clico em carregar
	And informo o professor
	When clico em gravar
	Then a programacao eh realizada com sucesso

# Estrutura: EnsinoRegular/Academico/ProgramacaoDeAvaliacoes/Excluir/ExcluirProgramacaoDeAvaliacoes.feature

@Academico
@ProgramacaoDeAvaliacoes
@ProgramacaoDeAvaliacoesExcluir
Scenario: Excluir programacao de avaliacoes
	Given Que desejo excluir uma programacao de avaliacoes
	And clico em carregar
	And clico em excluir, a programacao desejada
	When clico em sim
	Then a programacao eh removida com sucesso

# ============================================
# 3. ACADÊMICO - AVALIAÇÕES - MANUTENÇÃO DE TABELA DE AVALIAÇÃO
# Estrutura: EnsinoRegular/Academico/ManutencaoDeTabelasDeAvaliacao/Cadastrar/CadastrarTabelaDeAvaliacao.feature
# ============================================

@Academico
@Avaliacoes
@CadastrarTabelasDeAvaliacao
Scenario: Cadastrar uma tabela de avaliacao
	Given que desejo cadastrar uma tabela de avaliacao
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And clico em 'Cadastrar'
	And altero o tipo de tabela de avaliacao para 'Normal'
	And preencho com o nome de 'TABELA AVALIACAO TESTE'
	And altero a opcao de tabela ativa para 'Sim'
	And altero a tabela padrao para 'Não'
	And altero a opcao calcular avaliacoes analiticas para 'Não'
	And altero a opcao template portal de professor para 'Não'
	And clico em 'Avaliação'
	And insiro o tipo de avaliacao 'Normal'
	And insiro a descricao resumida 'N'
	And insiro a descricao 'Descrição N'
	And insiro o valor maximo '100'
	And clico em 'Confirmar'
	And repito a adicao de avaliacoes 'Normal' mais '2' vezes
	And altero a opcao formula aceita nulo para 'Sim'
	And clico em 'Editar fórmula'
	And insiro a formula '(N1+N2+N3)/4'
	And clico em 'Confirmar'
	When clico em 'Gravar'
	Then e trago a mensagem 'Cadastro realizado com sucesso!'
	#Then removo o operador

@Academico
@Avaliacoes
@CadastrarTabelasDeAvaliacaoPortalDoProfessor
Scenario: Cadastro de uma tabela de avaliacao com template no portal de professsor
	Given que desejo cadastrar uma tabela de avaliacao
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And clico em 'Cadastrar'
	And altero o tipo de tabela de avaliacao para 'Normal'
	And preencho com o nome de 'TABELA AVALIACAO TESTE'
	And altero a opcao de tabela ativa para 'Sim'
	And altero a opcao calcular avaliacoes analiticas para 'Não'
	And altero a opcao template portal de professor para 'Sim'
	And insiro o valor SAV '10'
	And clico em 'Avaliação'
	And insiro o tipo de avaliacao 'Normal'
	And insiro a descricao resumida 'N'
	And insiro a descricao 'Descricao N'
	And insiro o valor maximo '10'
	And clico em 'Confirmar'
	And repito a adicao de avaliacoes 'Normal' mais '2' vezes
	And altero a opcao formula aceita nulo para 'Sim'
	And clico em 'Editar fórmula'
	And insiro a formula '(N1+N2+N3)/4'
	And clico em 'Confirmar'
	When clico em 'Gravar'
	Then e trago a mensagem 'Cadastro realizado com sucesso!'
	#Then removo o operador

# Estrutura: EnsinoRegular/Academico/ManutencaoDeTabelasDeAvaliacao/Editar/EditarTabelaDeAvaliacao.feature

@Academico
@Avaliacoes
@EditarTabelasDeAvaliacaoCadastrando
Scenario: Editar uma tabela de avaliacao
	Given que desejo editar uma tabela de avaliacao cadastrando
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso a tabela 'tabela teste'
	And clico em 'Editar'
	And altero o nome para 'TABELA AVALIACAO TESTE ALTERADA'
	When clico em 'Gravar'
	Then e trago a mensagem 'Cadastro realizado com sucesso!'
	#Then removo o operador

@Academico
@Avaliacoes
@EditarStatusTabelasDeAvaliacaoCadastrando
Scenario: Inativar uma tabela de Avaliação Cadastrando
	Given que desejo editar uma tabela de avaliacao cadastrando
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso a tabela 'tabela teste'
	And clico em 'Editar'
	And altero a opcao de tabela ativa para 'Não'
	And clico em 'Gravar'
	When e trago a mensagem 'Cadastro realizado com sucesso!'
	Then valido a alteracao do status da tabela filtrando
	#Then removo o operador

# Estrutura: EnsinoRegular/Academico/ManutencaoDeTabelasDeAvaliacao/Excluir/ExcluirTabelaDeAvaliacao.feature

@Academico
@Avaliacoes
@ExcluirTabelasDeAvaliacaoCadastrando
Scenario: Excluir uma tabela de avaliacao cadastrando
	Given que desejo excluir uma tabela de avaliacao
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso a tabela 'TABELA TESTE QA'
	And clico em 'Remover'
	When clico em 'Sim'
	Then e trago a mensagem 'Item excluído com sucesso.'
	#Then removo o operador

@Academico
@Avaliacoes
@ExcluirTabelasDeAvaliacaoEmUsoCadastrando
Scenario: Excluir de uma tabela de avaliacao em uso
	Given que desejo excluir uma tabela de avaliacao em uso
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And pesquiso a tabela 'tabela teste'
	When clico em 'Remover'
	Then e trago a mensagem 'Exclusão não permitida pois há registros de distribuição inserido no sistema.'
	#Then removo o operador

# ============================================
# 3. ACADÊMICO - BOLETINS E FICHA DE APROVEITAMENTO - MANUTENÇÃO DE BOLETIM COLETIVO
# Estrutura: EnsinoRegular/Academico/ManutencaoBoletimColetivo/ManutencaoBoletimColetivo.feature
# ============================================

@Academico
@ManutencaoBoletimColetivo
@ManutencaoBoletimColetivoLancarNotas
Scenario: Validar lancamento de notas no boletim coletivo
	Given que acesso a tela de Manutencao de Boletim Coletivo
	And seleciono a Serie
	And seleciono a Turma
	And seleciono o Ano
	And seleciono a Disciplina
	And clico em Carregar
	And realizo o lancamento de notas
	And clico em Gravar
	When clico em Sim
	Then o boletim e gravado com sucesso

@Academico
@ManutencaoBoletimColetivo
@ManutencaoBoletimColetivoLancarNotas
Scenario: Validar lancamento de faltas no boletim coletivo
	Given que acesso a tela de Manutencao de Boletim Coletivo
	And seleciono a Serie
	And seleciono a Turma
	And seleciono o Ano
	And seleciono a Disciplina
	And clico em Carregar
	And realizo o lancamento de faltas
	And clico em Gravar
	When clico em Sim
	Then o boletim e gravado com sucesso

@Academico
@ManutencaoBoletimColetivo
@ManutencaoBoletimColetivoLancarNotas
Scenario: Validar a funcionalidade Recalcular
	Given que acesso a tela de Manutencao de Boletim Coletivo
	And seleciono a Serie
	And seleciono a Turma
	And seleciono o Ano
	And seleciono a Disciplina
	And clico em Carregar
	And realizo o lancamento de notas
	And realizo o lancamento de faltas
	When clico em Recalcular
	Then o recalculo do boletim e realizado

@Academico
@ManutencaoBoletimColetivo
@ManutencaoBoletimColetivoLancarNotas
Scenario: Validar atalho Emitir
	Given que acesso a tela de Manutencao de Boletim Coletivo
	When clico em Emitir
	Then a tela de Emissao do Boletim Escolar e carregada

@Academico
@ManutencaoBoletimColetivo
@ManutencaoBoletimColetivoLancarNotas
Scenario: Validar alerta ao clicar em Boletim individual sem selecionar aluno
	Given que acesso a tela de Manutencao de Boletim Coletivo
	When clico em Boletim individual
	Then e apresentado um alerta

@Academico
@ManutencaoBoletimColetivo
@ManutencaoBoletimColetivoLancarNotas
Scenario: Validar atalho Boletim individual selecionando um aluno
	Given que acesso a tela de Manutencao de Boletim Coletivo
	And seleciono a Serie
	And seleciono a Turma
	And seleciono o Ano
	And seleciono a Disciplina
	And clico em Carregar
	And seleciono um Aluno
	When clico em Boletim individual
	Then a tela de Manutencao de Boletim Individual e carregada

# ============================================
# 3. ACADÊMICO - BOLETINS E FICHA DE APROVEITAMENTO - MANUTENÇÃO DE BOLETIM INDIVIDUAL
# Estrutura: EnsinoRegular/Academico/ManutencaoBoletimIndividual/ManutencaoBoletimIndividual.feature
# ============================================

@Academico
@ManutencaoBoletimIndividual
@ManutencaoBoletimIndividualLancarNotas
Scenario: Validar lancamento de notas no boletim individual
	Given que preciso realizar o lancamento de notas na manutencao do boletim individual
	And seleciono o ano letivo
	And seleciono o aluno
	And clico em Carregar
	And realizo o lancamento de notas
	And clico em Gravar
	When clico em Sim
	Then o boletim e gravado com sucesso

@Academico
@ManutencaoBoletimIndividual
@ManutencaoBoletimIndividualLancarFaltas
Scenario: Validar lancamento de faltas no boletim individual
	Given que preciso realizar o lancamento de faltas na manutencao do boletim individual
	And seleciono o ano letivo
	And seleciono o aluno
	And clico em Carregar
	And realizo o lancamento de faltas
	And clico em Gravar
	When clico em Sim
	Then o boletim e gravado com sucesso

@Academico
@ManutencaoBoletimIndividual
@ManutencaoBoletimIndividualEmitirBoletim
Scenario: Validar a emissao do boletim individual
	Given que preciso realizar a emissao do boletim individual
	And seleciono o ano letivo
	And seleciono o aluno
	And clico em Carregar
	And clico em Emitir
	When clico em Emitir novamente
	Then o boletim e emitido com sucesso

@Academico
@ManutencaoBoletimIndividual
@ManutencaoBoletimIndividualRecalcularBoletim
Scenario: Validar o recalculo do boletim
	Given que preciso recalcular as nota e faltas do boletim
	And seleciono o ano letivo
	And seleciono o aluno
	And clico em Carregar
	And realizo o lancamento de notas
	And realizo o lancamento de faltas
	When clico em Recalcular
	Then o recalculo do boletim e realizado

# ============================================
# 3. ACADÊMICO - BOLETINS E FICHA DE APROVEITAMENTO - ASSOCIAÇÃO DE SÉRIE À CONFIGURAÇÃO DO BOLETIM
# Estrutura: EnsinoRegular/Academico/Boletins e Ficha de Aproveitamento/Associacao de serie a configuracao do boletim/AssociaSerieBoletim.feature
# ============================================

@Academico
@AssociacaoDeSerieAConfiguracaoDoBoletim
@CadastrarAssociacaoDaConfiguracaoComASerie
Scenario: Realizar a associacao de serie a configuracao do boletim
	Given que desejo cadastrar uma nova associacao de configuracao com uma serie
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And seleciono o ano letivo '2025'
	And clico em 'Carregar'
	And seleciono a serie '??CURSO TESET??'
	And seleciono a configuracao do boletim 'CONFIGURAÇÃO PADRÃO'
	And seleciono a progressao parcial como 'NÃO'
	When clico em 'Gravar'
	Then e apresentada a mensagem 'Operação realizada com sucesso!'
	#Then removo o operador

# ============================================
# 3. ACADÊMICO - DIÁRIO DE CLASSE - LANÇAMENTO DIÁRIO DE FREQUÊNCIA
# Estrutura: EnsinoRegular/Academico/DiarioDeClasse/CadastroLancamentoDiarioFrequencia/Cadastrar/CadastroLancamentoDiarioFrequencia.feature
# ============================================

@Academico
@CadastroLancamentoDiarioFrequencia
@CadastrarLancamentoDeFrequenciaParaSeriesComTipoDeFrequenciaPorDiaParaTodosOsAlunos
Scenario: Cadastrar Lancamento De Frequencia Para Series Com Tipo De Frequencia Por Dia Para Todos Os Alunos
	Given que preciso realizar o 'lancamento' de frequencia
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And seleciono o ano letivo : '2025'
	And seleciono o mes : 'janeiro'
	And seleciono a Serie/Turma : '2º ANO [Ensino Fundamental] / A - ANGLO'
	And clico em 'Carregar'
	And seleciono o dia especifico : 'Todos'
	And lanco presenca todos os alunos
	When clico em 'Gravar'
	Then o sistema apresenta a mensagem : 'Cadastro realizado com sucesso!'
	#Then removo o operador

@Academico
@CadastroLancamentoDiarioFrequencia
@CadastrarLancamentoDeFrequenciaParaSeriesComTipoDeFrequenciaPorDisciplinaParaTodosOsAlunos
Scenario: Cadastrar Lancamento De Frequencia Para Series Com Tipo De Frequencia Por Disciplina Para Todos Os Alunos
	Given que preciso realizar o 'lancamento' de frequencia
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And seleciono o ano letivo : '2024'
	And seleciono o mes : 'setembro'
	And seleciono a Serie/Turma : '10 - 7º ANO [Ensino Fundamental] / 7 - A - CJ'
	And seleciono a disciplina : 'GEOGRAFIA'
	And clico em 'Carregar'
	And seleciono o dia especifico : 'Todos'
	And lanco presenca todos os alunos
	When clico em 'Gravar'
	Then o sistema apresenta a mensagem : 'Cadastro realizado com sucesso!'
	#Then removo o operador

# Estrutura: EnsinoRegular/Academico/DiarioDeClasse/CadastroLancamentoDiarioFrequencia/Editar/EditarLancamentoDiarioFrequencia.feature

@Academico
@CadastroLancamentoDiarioFrequencia
@EditandoLancamentoDeFrequenciaParaSeriesComTipoDeFrequenciaPorDiaCadastrando
Scenario: Editando Lancamento de Frequencia para Series com Tipo de Frequencia por dia
	Given que preciso realizar a 'edicao' do lancamento de frequencia
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And seleciono o ano letivo : '2025'
	And seleciono o mes : 'janeiro'
	And seleciono a Serie/Turma : '2º ANO [Ensino Fundamental] / A - ANGLO'
	And clico em 'Carregar'
	And seleciono o dia especifico : 'Todos'
	And lanco falta para todos os alunos
	When clico em 'Gravar'
	Then o sistema apresenta a mensagem : 'Cadastro realizado com sucesso!'
	#Then removo o operador

@Academico
@CadastroLancamentoDiarioFrequencia
@EditandoLancamentoDeFrequenciaParaSeriesComTipoDeFrequenciaPorDisciplinaCadastrando
Scenario: Editando Lançamento de Frequência para Séries com Tipo de Frequência por disciplina
	Given que preciso realizar a 'edicao' do lancamento de frequencia
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And seleciono o ano letivo : '2024'
	And seleciono o mes : 'setembro'
	And seleciono a Serie/Turma : '10 - 7º ANO [Ensino Fundamental] / 7 - A - CJ'
	And seleciono a disciplina : 'GEOGRAFIA'
	And clico em 'Carregar'
	And seleciono o dia especifico : 'Todos'
	And lanco falta para todos os alunos
	When clico em 'Gravar'
	Then o sistema apresenta a mensagem : 'Cadastro realizado com sucesso!'
	#Then removo o operador

# Estrutura: EnsinoRegular/Academico/DiarioDeClasse/CadastroLancamentoDiarioFrequencia/Excluir/ExcluirLancamentoDiarioFrequencia.feature

@Academico
@CadastroLancamentoDiarioFrequencia
@ExcluindoLancamentoDeFrequenciaParaSeriesComTipoDeFrequenciaPorDiaCadastrando
Scenario: Excluindo Lancamento de Frequencia para Series com Tipo de Frequencia por dia
	Given que preciso realizar a 'exclusao' do lancamento de frequencia
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
	And seleciono o ano letivo : '2025'
	And seleciono o mes : 'agosto'
	And seleciono a Serie/Turma : '2º ANO [Ensino Fundamental] / A - ANGLO'
	And clico em 'Carregar'
	And seleciono o dia especifico : '5'
	And clico em 'Excluir'
	And o sistema apresenta a mensagem : 'Deseja excluir os lançamentos do dia 5 do mês de agosto?'
	When clico em 'Sim'
	Then o sistema apresenta a mensagem : 'Lançamento excluído com sucesso'

@Academico
@CadastroLancamentoDiarioFrequencia
@ExcluindoLancamentoDeFrequenciaParaSeriesComTipoDeFrequenciaPorDisciplinaCadastrando
Scenario: Excluindo Lançamento de Frequência para Séries com Tipo de Frequência por disciplina
	Given que preciso realizar a 'exclusao' do lancamento de frequencia
	And acesso o sistema utilizando o operador 'TESTE VERSAO COMPLETO'
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

# ============================================
# 3. ACADÊMICO - DIÁRIO DE CLASSE - CONTEÚDO MINISTRADO
# Estrutura: EnsinoRegular/Academico/DiarioDeClasse/ConteudoMinistrado/1Lancar/ConteudoMinistradoLancar.feature
# ============================================

@Academico
@DiarioDeClasse
Scenario: Lancar conteudo ministrado
	Given que desejo realizar lancamento de conteudo ministrado
	And configuro os filtros do conteudo ministrado
	And clico em carregar, o conteudo ministrado
	And realizo o lancamento do conteudo ministrado
	When clico em gravar, o conteudo ministrado
	Then a tela de conteudo ministrado apresenta a mensagem: "Conteudos ministrados gravados com sucesso!"

# ============================================
# 3. ACADÊMICO - DIÁRIO DE CLASSE - PROCESSAMENTO DE FALTAS PARA BOLETIM
# Estrutura: EnsinoRegular/Academico/DiarioDeClasse/ProcessamentoDeFaltasParaBoletim/*.feature
# ============================================

#@Academico
#@DiarioDeClasse
#Scenario: Processamento de faltas para boletim
#	Given que desejo processar faltas para boletim
#	And seleciono o operador 'TESTE VERSAO COMPLETO'
#	When executo o processamento de faltas para boletim
#	Then o processamento de faltas para boletim e realizado com sucesso

# ============================================
# 3. ACADÊMICO - HISTÓRICO E CERTIFICADO - EMISSÃO FICHA DO ALUNO
# Estrutura: EnsinoRegular/Academico/EmissaoFichaAluno/EmitirFichaAluno.feature
# ============================================

@Academico
@EmissãoFichaAluno
@EmissãoFichaAlunoFiltroEmpresa
Scenario: Emitir ficha com aluno filtrando por empresa
	Given emitir a ficha do aluno
	And filtro por 'Empresa'
	And seleciono a empresa 'COLÉGIO SANTA ROSA'
	And clico em 'Mostrar'
	When clico em 'Emitir'
	Then é gerado um relatório

@Academico
@EmissãoFichaAluno
@EmissãoFichaAlunoFiltroSerie
Scenario: Emitir ficha com aluno filtrando por série
	Given emitir a ficha do aluno
	And filtro por 'Série'
	And seleciono a série '3º ANO [Ensino Fundamental]'
	And clico em 'Mostrar'
	When clico em 'Emitir'
	Then é gerado um relatório

@Academico
@EmissãoFichaAluno
@EmissãoFichaAlunoFiltroSerieTurma
Scenario: Emitir ficha com aluno filtrando por série/turma
	Given emitir a ficha do aluno
	And filtro por 'Série/Turma'
	And seleciono a série '1º ANO [Ensino Fundamental]'
	And seleciono a turma 'TURMA A'
	And clico em 'Mostrar'
	When clico em 'Emitir'
	Then é gerado um relatório

@Academico
@EmissãoFichaAluno
@EmissãoFichaAlunoFiltroAluno
Scenario: Emitir ficha com aluno filtrando por aluno
	Given emitir a ficha do aluno
	And filtro por 'Aluno'
	And seleciono o aluno 240476
	And clico em 'Mostrar'
	When clico em 'Emitir'
	Then é gerado um relatório

# ============================================
# 3. ACADÊMICO - HISTÓRICO E CERTIFICADO - EMISSÃO ATA DE REGISTRO DE MATRÍCULA
# Estrutura: EnsinoRegular/Academico/EmissaoAtaRegMatricula/EmissaoAtaRegMatricula.feature
# ============================================

@Academico
@EmissãoAtaRegistroMatricula
@EmissãoAtaRegistroMatriculaFiltroEmpresa
Scenario: Emitir ata de registro de matricula por empresa
	Given que desejo emitir a ata de registro de matriculas por empresa
	And clico em 'Filtro'
	And seleciono o ano '2023'
	And filtro por 'Empresa'
	And seleciono a empresa 'COLÉGIO SANTA ROSA'
	And defino o título no leiaute 'Ata de Registro de Matrícula 123'
	And defino o leiaute 'Mostrar'
	And defino as Assinaturas
	And defino as Assinaturas para mostrar o 'Diretor(a)'
	And defino as Assinaturas para mostrar o 'Secretário(a)'
	When clico em 'Emitir'
	Then é gerado um relatório

@Academico
@EmissãoAtaRegistroMatricula
@EmissãoAtaRegistroMatriculaFiltroSerieTurma
Scenario: Emitir ata de registro de matricula por serie/turma
	Given que desejo emitir a ata de registro de matriculas por série/turma
	And clico em 'Filtro'
	And seleciono o ano '2023'
	And filtro por 'série/turma'
	And seleciono a série '1º ANO [Ensino Fundamental]'
	And seleciono a turma 'TURMA B'
	And defino o título no leiaute 'Ata de Registro de Matrícula 123'
	And defino o leiaute 'Mostrar'
	And defino as Assinaturas
	When clico em 'Emitir'
	Then é gerado um relatório

@Academico
@EmissãoAtaRegistroMatricula
@EmissãoAtaRegistroMatriculaApresentandoAssinaturas
Scenario: Emitir ata de registro de matricula com assinaturas
	Given que desejo emitir a ata de registro de matriculas por série/turma
	And clico em 'Filtro'
	And seleciono o ano '2023'
	And filtro por 'série/turma'
	And seleciono a série '1º ANO [Ensino Fundamental]'
	And seleciono a turma 'TURMA B'
	And defino o título no leiaute 'Ata de Registro de Matrícula 123'
	And defino o leiaute 'Mostrar'
	And defino as Assinaturas
	And defino as Assinaturas para mostrar o 'Diretor(a)'
	And defino as Assinaturas para mostrar o 'Secretário(a)'
	When clico em 'Emitir'
	Then é gerado um relatório

# ============================================
# 4. FINANCEIRO - PLANOS DE PAGAMENTO
# Estrutura: EnsinoRegular/Financeiro/PlanosDePagamentos/Cadastrar/CadastrarPlanosDePagamentos.feature
# ============================================

@Financeiro
@PlanosDePagamentos
@CadastrarPlanoDePagamentosInformandoTipoDeContaContabilBancaria
Scenario: Cadastrar Plano De Pagamentos Informando Tipo De Conta Contabil Bancaria
	Given que desejo cadastrar planos de pagamentos
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	And clico em 'Cadastrar'
	And informo a descricao 'Plano de pagamentos teste automatizado'
	And clico em Ativo 'Sim'
	And clico em 'Adicionar'
	And seleciono a Finalidade 'MENSALIDADE'
	And insiro o valor mensal '1000'
	And insiro as Parcelas '10'
	And seleciono a Data Primeira parcela 'Data Atual'
	And insiro a Qtde dias a mais '1'
	And seleciono Outros vencimentos 'Fixo, todo dia' Dia '1'
	And seleciono o Pagamento 'Manter o vencimento'
	And seleciono Conta contábil de conf 'Não'
	And seleciono o Tipo de conta contabil 'Bancária'
	And seleciono a Informacao bancaria '1 - BRADESCO PRINCIPAL - T9'
	And seleciono a Conta contabil 'MENSALIDADES - UNID - T9 [1 - BRADESCO PRINCIPAL - T9]'
	And clico em 'Confirmar'
	And seleciono a Serie '1º ANO [Ensino Fundamental]'
	And seleciono a Turma 'Todos'
	And clico em 'Associar'
	And valido a simulacao das '10' duplicatas de valor '1000,00'
	And clico em 'Gravar'
	And clico em 'Sim'
	When e apresentada a mensagem 'Cadastro realizado com sucesso!'
	Then clico em 'Voltar para a lista'
	#Then removo o operador

@Financeiro
@PlanosDePagamentos
@CadastrarPlanoDePagamentosInformandoTipoDeContaContabilTesouraria
Scenario: Cadastrar Plano De Pagamentos Informando Tipo De Conta Contabil Tesouraria
	Given que desejo cadastrar planos de pagamentos
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	And clico em 'Cadastrar'
	And informo a descricao 'Plano de pagamentos teste automatizado'
	And clico em Ativo 'Sim'
	And clico em 'Adicionar'
	And seleciono a Finalidade 'MENSALIDADE'
	And insiro o valor mensal '1000'
	And insiro as Parcelas '10'
	And seleciono a Data Primeira parcela 'Data Atual'
	And insiro a Qtde dias a mais '1'
	And seleciono Outros vencimentos 'Fixo, todo dia' Dia '1'
	And seleciono o Pagamento 'Manter o vencimento'
	And seleciono Conta contábil de conf 'Não'
	And seleciono o Tipo de conta contabil 'Tesouraria'
	And seleciono a Conta contabil 'MENSALIDADES - UNID - T9'
	And clico em 'Confirmar'
	And seleciono a Serie '1º ANO [Ensino Fundamental]'
	And seleciono a Turma 'Todos'
	And clico em 'Associar'
	And valido a simulacao das '10' duplicatas de valor '1000'
	And clico em 'Gravar'
	And clico em 'Sim'
	When e apresentada a mensagem 'Cadastro realizado com sucesso!'
	Then clico em 'Voltar'
	#Then removo o operador

@Financeiro
@PlanosDePagamentos
@CancelarCadastroDePlanoDePagamentos
Scenario: Cancelar Cadastro De Plano De Pagamentos
	Given que desejo cadastrar planos de pagamentos
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	And clico em 'Cadastrar'
	And clico em 'Gravar'
	And verifico a validacao de campo 'Descrição'
	And clico em 'Cancelar'
	When clico em 'Sim'
	Then e apresentada a tela de aterrissagem
	#Then removo o operador

# Estrutura: EnsinoRegular/Financeiro/PlanosDePagamentos/Editar/EditarPlanosDePagamentos.feature

@Financeiro
@PlanosDePagamentos
@EditarPlanosDePagamentosCadastrandoAlterandoFinalidadeETipoDeContaContabil
Scenario: Editar Planos De Pagamentos Cadastrando Alterando Finalidade E Tipo De Conta Contabil
	Given que desejo editar planos de pagamentos
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo plano de pagamentos 'plano de pagamento pesquisa'
	And clico em 'Editar' no plano de pagamentos
	And na grid Finalidade clico em 'Editar'
	And altero a Finalidade para 'MATRÍCULA'
	And seleciono o Tipo de conta contabil 'Tesouraria'
	And seleciono a Conta contabil 'MATRICULA - UNID - T9'
	And clico em 'Confirmar'
	And clico em 'Gravar'
	When clico em 'Sim'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
	#Then removo o operador

@Financeiro
@PlanosDePagamentos
@EditarPlanosDePagamentosCadastrandoAlterandoAnoReferenciaEDescricao
Scenario: Editar Planos De Pagamentos Cadastrando Alterando Ano Referencia E Descricao
	Given que desejo editar planos de pagamentos
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo plano de pagamentos 'plano de pagamento pesquisa'
	And clico em 'Editar' no plano de pagamentos
	And altero o ano de referencia para '2024'
	And altero a descricao para 'plano de pagamento editado'
	And clico em 'Gravar'
	When clico em 'Sim'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
	#Then removo o operador

@Financeiro
@PlanosDePagamentos
@EditarPlanosDePagamentosAlterandoStatus
Scenario: Editar Planos De Pagamentos Alterando Status
	Given que desejo editar planos de pagamentos
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo plano de pagamentos 'plano de pagamento pesquisa'
	And clico em 'Editar' no plano de pagamentos
	And altero o campo Ativo para 'Não'
	And clico em 'Gravar'
	And clico em 'Sim'
	And e apresentada a mensagem 'Cadastro realizado com sucesso!'
	When pesquiso pelo plano de pagamentos 'plano de pagamento pesquisa' com status alterado
	Then e apresentado o plano de pagamento com status 'Inativas'

# Estrutura: EnsinoRegular/Financeiro/PlanosDePagamentos/Excluir/ExcluirPlanosDePagamentos.feature

@Financeiro
@PlanosDePagamentos
@ExcluirPlanosDePagamentosCadastrandoDoTipoDeContaContabilBancaria
Scenario: Excluir Planos De Pagamentos Cadastrando Do Tipo De Conta Contabil Bancaria
	Given que desejo excluir planos de pagamentos
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo plano de pagamentos 'plano de pagamento pesquisa'
	And clico em 'Remover' no plano de pagamentos
	When clico em 'Sim'
	Then e apresentada a mensagem 'Item excluído com sucesso.'
	#Then removo o operador

@Financeiro
@PlanosDePagamentos
@ExcluirPlanosDePagamentosCadastrandoDoTipoDeContaContabilTesouraria
Scenario: Excluir Planos De Pagamentos Cadastrando Do Tipo De Conta Contabil Tesouraria
	Given que desejo excluir planos de pagamentos
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo plano de pagamentos 'plano de pagamento pesquisa'
	And clico em 'Remover' no plano de pagamentos
	When clico em 'Sim'
	Then e apresentada a mensagem 'Item excluído com sucesso.'
	#Then removo o operador

@Financeiro
@PlanosDePagamentos
@ExcluirPlanosDePagamentosCadastrandoComStatusInativo
Scenario: Excluir Planos De Pagamentos Cadastrando Com Status Inativo
	Given que desejo excluir planos de pagamentos
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo plano de pagamentos 'plano de pagamento inativo'
	And clico em 'Remover' no plano de pagamentos
	When clico em 'Sim'
	Then e apresentada a mensagem 'Item excluído com sucesso.'
	#Then removo o operador

@Financeiro
@PlanosDePagamentos
@ExcluirPlanosDePagamentosQuePossuiVinculoComMatriculas
Scenario: Excluir Planos De Pagamentos Que Possui Vinculo Com Matriculas
	Given que desejo excluir planos de pagamentos
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	And cadastro o plano de pagamentos 'plano de pagamento vinculado'
	And gero duplicatas para o aluno 'Aluno qa' utlizando o plano de pagamentos
	And pesquiso pelo plano de pagamentos 'plano de pagamento vinculado'
	And clico em 'Remover' no plano de pagamentos
	When clico em 'Sim'
	Then e apresentada a mensagem 'Não foi possível excuir, pois existem matrículas vinculada a este plano de pagamento.'

# Estrutura: EnsinoRegular/Financeiro/PlanosDePagamentos/Pesquisar/PesquisarPlanosDePagamentos.feature

@Financeiro
@PlanosDePagamentos
@PesquisarPlanosDePagamentosCadastrandoAtivo
Scenario: Pesquisar Planos De Pagamentos Cadastrando Ativo
	Given que desejo pesquisar planos de pagamentos
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	When pesquiso pelo plano de pagamentos ativo 'plano de pagamento pesquisa'
	Then e apresentado o plano de pagamentos pesquisado
	#Then removo o operador

@Financeiro
@PlanosDePagamentos
@PesquisarPlanosDePagamentosCadastrandoInativo
Scenario: Pesquisar Planos De Pagamentos Cadastrando Inativo
	Given que desejo pesquisar planos de pagamentos
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	When pesquiso pelo plano de pagamentos inativo 'plano de pagamento pesquisa'
	Then e apresentado o plano de pagamentos pesquisado
	#Then removo o operador

@Financeiro
@PlanosDePagamentos
@PesquisarPlanosDePagamentosCadastrandoClonado
Scenario: Pesquisar Planos De Pagamentos Cadastrando Clonado
	Given que desejo pesquisar planos de pagamentos
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	When pesquiso pelo plano de pagamentos clonado 'plano de pagamento clonado'
	Then e apresentado o plano de pagamentos pesquisado
	#Then removo o operador

@Financeiro
@PlanosDePagamentos
@PesquisarPlanosDePagamentosCadastrandoEmAnoDiferenteDoSelecionado
Scenario: Pesquisar Planos De Pagamentos Cadastrando Em Ano Diferente Do Selecionado
	Given que desejo pesquisar planos de pagamentos
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	When pesquiso pelo plano de pagamentos de ano diferente 'plano de pagamento ano diferente'
	Then nao e apresentado o plano de pagamentos pesquisado
	#Then removo o operador

# Estrutura: EnsinoRegular/Financeiro/PlanosDePagamentos/Clonar/ClonarPlanosDePagamentos.feature

@Financeiro
@PlanosDePagamentos
@ClonarPlanoDePagamentosCadastrandoComStatusAtivo
Scenario: Clonar Plano De Pagamentos Cadastrando Com Status Ativo
	Given que desejo clonar planos de pagamentos
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo plano de pagamentos ativo 'plano de pagamento ativo'
	And clico em 'Clonar'
	And clico em 'Gravar'
	When clico em 'Sim'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
	#Then removo o operador

@Financeiro
@PlanosDePagamentos
@ClonarPlanoDePagamentosCadastrandoComStatusInativo
Scenario: Clonar Plano De Pagamentos Cadastrando Com Status Inativo
	Given que desejo clonar planos de pagamentos
	And seleciono o operador 'TESTE VERSAO COMPLETO'
	And pesquiso pelo plano de pagamentos inativo 'plano de pagamento inativo'
	And clico em 'Clonar'
	And clico em 'Gravar'
	When clico em 'Sim'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
	#Then removo o operador

# ============================================
# 4. FINANCEIRO - RESUMO FINANCEIRO
# Estrutura: EnsinoRegular/Financeiro/ResumoFinanceiro/C02-0001-ResumoFinanceiro.feature
# ============================================

@Financeiro
@ResumoFinanceiro
Scenario: Filtros do Resumo Financeiro
	Given que preciso validar os filtros do resumo financeiro de um aluno
	And seleciono o aluno
	And clico em Carregar
	And seleciono uma finalidade
	When seleciono os filtros
	Then o sistema deve listar as informacoes de acordo com os filtros

@Financeiro
@ResumoFinanceiro
Scenario: Filtros do Resumo Financeiro sem finalidade
	Given que preciso validar os filtros do resumo financeiro de um aluno
	And seleciono o aluno
	And clico em Carregar
	When seleciono os filtros
	Then o sistema deve listar as informacoes de acordo com os filtros

