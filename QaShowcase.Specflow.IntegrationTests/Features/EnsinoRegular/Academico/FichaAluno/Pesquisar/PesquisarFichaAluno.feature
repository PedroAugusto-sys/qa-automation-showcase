Feature: Pesquisar ficha do aluno

@Academico
@FichaAluno
@PesquisarFichaAlunoNomeCadastrando
Scenario: Pesquisar ficha do aluno por nome cadastrando
	Given que preciso pesquisar um aluno
	And acesso o sistema utilizando o operador 'TESTE QA'
	And digito em 'Nome ou número de matrícula' o nome do aluno 'ALUNO TESTE QA'
	When pressiono a tecla 'Enter'
	Then e apresentado somente alunos que contenham em seu nome o nome digitado
	Then removo o operador

@Academico
@FichaAluno
@PesquisarPorNomeRespFichaAlunoCadastrando
Scenario: Pesquisar ficha do aluno pelo nome do responsavel
	Given que preciso pesquisar um aluno
	And acesso o sistema utilizando o operador 'OPERADOR QA'
	And pesquiso por responsavel o aluno 'ALUNO TESTE QA'
	When pressiono a tecla 'Enter'
	Then e apresentado somente alunos que contenham nome do responsavel digitado, vinculado a sua ficha do aluno
	Then removo o operador

@Academico
@FichaAluno
@PesquisarAlunoPorIdCadastrando
Scenario: Pesquisar aluno por ID cadastrando
	Given que preciso pesquisar um aluno
	And acesso o sistema utilizando o operador 'OPERADOR QA'
	When pesquiso o ID do aluno 'ALUNO TESTE QA'
	Then o aluno e apresentado na grid
	Then removo o operador