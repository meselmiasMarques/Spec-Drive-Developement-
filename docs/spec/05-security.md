
---

# 6. `05-security.md`

Muito importante para projetos reais.

```markdown
# SPEC-006 — Segurança

## Autenticação

O sistema deverá utilizar JWT.

## Senha

Senhas nunca devem ser armazenadas em texto puro.

Utilizar hash seguro.

## Autorização

Usuários somente podem acessar recursos pertencentes
ao próprio usuário.

### Regra crítica

Toda consulta de dados deve considerar o UserId
do usuário autenticado.

Exemplo:

INCORRETO:

SELECT * FROM Transactions
WHERE Id = @id

CORRETO:

SELECT *
FROM Transactions
WHERE Id = @id
AND UserId = @userId

## API

Endpoints protegidos devem exigir autenticação.

## Dados sensíveis

Tokens, senhas e secrets não devem ser armazenados
no código-fonte.