# SPEC-002 — Requisitos

  ## REQ-001 — Cadastro de usuário

  O sistema deve permitir que um usuário crie uma conta.

  ### Dados

  - Nome
  - E-mail
  - Senha

  ### Regras

  - Nome obrigatório
  - E-mail obrigatório
  - E-mail deve possuir formato válido
  - E-mail deve ser único
  - Senha deve possuir no mínimo 8 caracteres

  ### Critérios de aceitação

  **CA-001**

  Dado que o usuário forneceu dados válidos,
  quando realizar o cadastro,
  então a conta deve ser criada.

  **CA-002**

  Dado que o e-mail já existe,
  quando realizar o cadastro,
  então o sistema deve retornar erro.

  **CA-003**

  Dado que a senha possui menos de 8 caracteres,
  quando realizar o cadastro,
  então o sistema deve rejeitar a operação.

  ## REQ-002 —  Login;

    ### Regras

    - E-mail obrigatório
    - Senha obrigatório

    ### Critérios de aceitação

    **CA-001**

    Dado que o usuário forneceu dados válidos,
    quando realizar o login,
    então permitir a visualização e criacão das açoes no software

    **CA-002**

    Ao logar na aplicação o usuário só poderá visualizar suas informações

    **CA-003**

    Dado o login via api, o front end deve replicar suas permissões, acessar a aplicação, permitir as ações. 


  ## REQ-003 —  logout;
    ### Regras

  - Ao deslogar não permitir ação nenhuma que não seja login, alteração de senha ou cadastro

  ### Critérios de aceitação

  **CA-001**

  Dado que o usuário deslogou da aplicação o sistema deve voltar para tela de login (front end)

  ## REQ-004 — refresh token;
    ### Regras

  - Permitir refresh token depois de 30 min
  - se o usuário permanecer mais de 40 minutos sem acesso, deslogar da aplicação

  ### Critérios de aceitação

  **CA-001**

  - não solicitar email e senha novamente
  - somente fazer o refresh caso o usuário ainda esteja em uso com a aplicação

  ## REQ-005 - recuperação de senha;
    ### Regras

  - A recuperação de senha será gerada uma nova senha via email cadastrado
  - a senha regerada deve ser trocada em até 10 minutos

  ### Critérios de aceitação

  **CA-001**
    Ao solicitar o reset de senha o sistema envia um email para o email do usuario cadastrado.
    a senha enviada deverá ser trocada em até 10 minutos senão será invalida.
  
  ## REQ-006 - dashboard financeiro;
    ### Regras

  - Dashboards responsivos
  - Usar modelos de pizza, barras, barras com linha de oscilação

  ### Critérios de aceitação

  **CA-001**

  Os dashs devem ter seu consumo por procedures

  **CA-002**

  Os dashs devem ter cores diferentes por categoria

  ## REQ-007 - relatórios;
    ### Regras

  - Gerar relatório do filtros : Data inicio, Data fim
  - Permitir Export PDF a partir do filtro

  ### Critérios de aceitação

  **CA-001**

  Dado o relatório o sistema exibe conforme os filtros estabelecidos.

  ## REQ-008 - categorias;
    ### Regras

  - Nome obrigatório
  - Deve ter no máximo 100 Caracteres
  - deve ter no mínimo 3

  ### Critérios de aceitação

  **CA-001**

  Dado que o usuário forneceu dados válidos,
  quando realizar o cadastro,
  então a conta deve ser criada.

  **CA-002**

  Dado que o e-mail já existe,
  quando realizar o cadastro,
  então o sistema deve retornar erro.

  **CA-003**

  Dado que a senha possui menos de 8 caracteres,
  quando realizar o cadastro,
  então o sistema deve rejeitar a operação.

  ## REQ-009 - transações;
    ### Regras

  - Nome obrigatório
  - E-mail obrigatório
  - E-mail deve possuir formato válido
  - E-mail deve ser único
  - Senha deve possuir no mínimo 8 caracteres

  ### Critérios de aceitação

  **CA-001**

  Dado que o usuário forneceu dados válidos,
  quando realizar o cadastro,
  então a conta deve ser criada.

  **CA-002**

  Dado que o e-mail já existe,
  quando realizar o cadastro,
  então o sistema deve retornar erro.

  **CA-003**

  Dado que a senha possui menos de 8 caracteres,
  quando realizar o cadastro,
  então o sistema deve rejeitar a operação.

  ## REQ-010 - consultas e filtros;
    ### Regras

  - Nome obrigatório
  - E-mail obrigatório
  - E-mail deve possuir formato válido
  - E-mail deve ser único
  - Senha deve possuir no mínimo 8 caracteres

  ### Critérios de aceitação

  **CA-001**

  Dado que o usuário forneceu dados válidos,
  quando realizar o cadastro,
  então a conta deve ser criada.

  **CA-002**

  Dado que o e-mail já existe,
  quando realizar o cadastro,
  então o sistema deve retornar erro.

  **CA-003**

  Dado que a senha possui menos de 8 caracteres,
  quando realizar o cadastro,
  então o sistema deve rejeitar a operação.

  ## REQ-011 - regras de paginação, ordenação e erros.
    ### Regras

  - Nome obrigatório
  - E-mail obrigatório
  - E-mail deve possuir formato válido
  - E-mail deve ser único
  - Senha deve possuir no mínimo 8 caracteres

  ### Critérios de aceitação

  **CA-001**

  Dado que o usuário forneceu dados válidos,
  quando realizar o cadastro,
  então a conta deve ser criada.

  **CA-002**

  Dado que o e-mail já existe,
  quando realizar o cadastro,
  então o sistema deve retornar erro.

  **CA-003**

  Dado que a senha possui menos de 8 caracteres,
  quando realizar o cadastro,
  então o sistema deve rejeitar a operação.
