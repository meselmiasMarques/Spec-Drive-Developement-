# SPEC-001 — Visão Geral do Projeto

## 1. Identificação

**Nome:** WeMoney
**Tipo:** Sistema Web / SaaS
**Status:** Em desenvolvimento
**Versão:** 1.0.0

---

## 2. Objetivo

Desenvolver uma aplicação para controle financeiro pessoal,
permitindo que usuários registrem receitas, despesas,
categorias e acompanhem seus resultados financeiros.

---

## 3. Problema

Usuários precisam de uma maneira simples de acompanhar
suas movimentações financeiras e entender sua situação
financeira mensal.

---

## 4. Público-alvo

- Usuários individuais
- Casais
- Pequenas famílias

---

## 5. Escopo

### Incluído

- Cadastro de usuário
- Login
- Categorias
- Receitas
- Despesas
- Dashboard financeiro
- Relatórios

### Fora do escopo

- Integração bancária
- Open Finance
- Investimentos
- Emissão de boletos

---

## 6. Tecnologias

### Backend

- .NET 10
- ASP.NET Core
- Entity Framework Core

### Banco

- PostgreSQL

### Frontend

- MudBlazor pwa

### Infraestrutura

- Docker
- Docker Compose

---

## 7. Requisitos principais

REQ-001 — Usuário deve conseguir criar uma conta.

REQ-002 — Usuário deve conseguir realizar login.

REQ-003 — Usuário deve conseguir cadastrar uma transação.

REQ-004 — Usuário deve conseguir visualizar seu saldo.

REQ-005 — Usuário não pode acessar dados de outro usuário.