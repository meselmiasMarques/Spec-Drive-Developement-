# SPEC-FUNC-001 — Transações

## Objetivo

Permitir que usuários registrem receitas e despesas.

---

## Funcionalidades

- Criar transação
- Consultar transações
- Consultar transação por ID
- Alterar transação
- Excluir transação

---

## Regras de negócio

### RN-001

O valor deve ser maior que zero.

### RN-002

A transação deve possuir uma categoria válida.

### RN-003

A categoria deve pertencer ao usuário.

### RN-004

O usuário somente pode alterar suas próprias
transações.

### RN-005

O tipo deve ser:

- INCOME
- EXPENSE

---

# Criar transação

## Endpoint

POST /api/v1/transactions

## Entrada

```json
{
  "description": "Supermercado",
  "amount": 250.50,
  "type": "EXPENSE",
  "categoryId": "uuid",
  "transactionDate": "2026-09-25"
}
```

> **Decisão (2026-09-25):** `transactionDate` (`yyyy-MM-dd`) é opcional;
> se ausente, a API usa a data atual (UTC).

## Respostas

| Operação | Sucesso | Erros |
|---|---|---|
| POST /api/v1/transactions | 201 | 400 validação/categoria inválida, 401 |
| GET /api/v1/transactions | 200 | 401 |
| GET /api/v1/transactions/{id} | 200 | 401, 404 |
| PUT /api/v1/transactions/{id} | 200 | 400, 401, 404 |
| DELETE /api/v1/transactions/{id} | 204 | 401, 404 |
| GET /api/v1/transactions/balance | 200 | 401 |

`balance` retorna `{ totalIncome, totalExpense, balance }` (REQ-004 da SPEC-001).
