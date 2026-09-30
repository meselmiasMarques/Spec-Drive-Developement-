

Aqui você define o banco **antes de sair criando entidades aleatoriamente**.

```markdown
# SPEC-004 — Modelo de Dados

## User

| Campo | Tipo | Obrigatório |
|---|---|---|
| Id | UUID | Sim |
| Name | varchar(150) | Sim |
| Email | varchar(200) | Sim |
| PasswordHash | varchar(500) | Sim |
| CreatedAt | timestamp | Sim |

---

## Category

| Campo | Tipo | Obrigatório |
|---|---|---|
| Id | UUID | Sim |
| UserId | UUID | Sim |
| Name | varchar(100) | Sim |
| Type | enum | Sim |

---

## Transaction

| Campo | Tipo | Obrigatório |
|---|---|---|
| Id | UUID | Sim |
| UserId | UUID | Sim |
| CategoryId | UUID | Sim |
| Description | varchar(200) | Sim |
| Amount | decimal | Sim |
| Type | enum | Sim |
| TransactionDate | date | Sim |
| CreatedAt | timestamp | Sim |

---

## Relacionamentos

User 1:N Category

User 1:N Transaction

Category 1:N Transaction