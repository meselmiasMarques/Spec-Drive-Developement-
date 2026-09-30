# SPEC-FUNC-001 — Categorias

## Objetivo

Permitir que usuários Crie suas categorias para organizar as transações

---

## Funcionalidades

- Criar Categoria
- Consultar Categoria
- Consultar Categoria por ID
- Alterar Categoria
- Excluir Categoria

---

## Regras de negócio

### RN-001

O nome da categoria deve ser de no mínimo 3 caracteres

### RN-002

Uma categoria que tiver transações associadas não pode ser excluida

### RN-003

A categoria deve pertencer ao usuário.

### RN-004

O usuário somente pode alterar suas próprias
categorias.
---

# Criar categoria

## Endpoint

POST /api/v1/categories

## Entrada

```json
{
  "name": "Contas fixas",
  "type": "EXPENSE"
}
```

> **Decisão (2026-09-25):** `type` é obrigatório e aceita `INCOME` ou `EXPENSE`
> (mesmo enum das transações), conforme `Category.Type` em SPEC-004.

## Respostas

| Operação | Sucesso | Erros |
|---|---|---|
| POST /api/v1/categories | 201 | 400 validação, 401 |
| GET /api/v1/categories | 200 | 401 |
| GET /api/v1/categories/{id} | 200 | 401, 404 |
| PUT /api/v1/categories/{id} | 200 | 400, 401, 404 |
| DELETE /api/v1/categories/{id} | 204 | 401, 404, 409 (RN-002) |
