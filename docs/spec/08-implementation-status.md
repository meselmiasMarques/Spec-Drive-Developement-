# SPEC-008 — Status da Implementação

Atualizado em 2026-09-26.

## Escopo implementado (versão 1.0.0 — núcleo)

| Requisito | Backend | Frontend |
|---|---|---|
| REQ-001 Cadastro de usuário | ✅ | ✅ `/register` |
| REQ-002 Login (JWT) | ✅ | ✅ `/login` |
| REQ-003 Logout | — (JWT stateless) | ✅ descarta o token e volta para `/login` |
| REQ-004 (SPEC-001) Saldo | ✅ `GET /transactions/balance` | ✅ `/` |
| REQ-005 (SPEC-001) Isolamento por usuário | ✅ toda consulta filtra por `UserId` | ✅ rotas exigem login |
| Categorias (SPEC-FUNC) | ✅ CRUD | ✅ `/categories` |
| Transações (SPEC-FUNC) | ✅ CRUD | ✅ `/transactions` |

## Decisões registradas

- `Category.Type` obrigatório: `INCOME` | `EXPENSE`.
- `Transaction.TransactionDate` opcional na entrada; padrão = data atual (UTC).
- A categoria da transação não precisa ter o mesmo tipo da transação (a SPEC não define essa regra).
- Frontend (MudBlazor PWA, Blazor WebAssembly):
  - Guarda o access token no `localStorage` e replica as permissões lendo as claims do JWT (REQ-002 CA-003).
    A assinatura é validada somente pela API.
  - Qualquer resposta 401 com sessão ativa encerra a sessão e redireciona para o login.
  - Validações de formulário espelham as regras da SPEC; a API continua sendo a fonte da verdade.
- Docker Compose (`db`, `api`, `web`): o nginx serve o PWA e faz proxy de `/api/` para a API
  (mesma origem, sem CORS). Secrets vêm do arquivo `.env` (ver `.env.example`), nunca do código (SPEC-006).

## Pendências de SPEC (não implementadas)

Aguardam definição antes da implementação, conforme `AGENTS.md`:

| Item | Inconsistência | Sugestão |
|---|---|---|
| REQ-003 | Cita "alteração de senha" sem sessão, mas não há requisito para isso. | Definir o fluxo ou remover a menção. |
| REQ-004 Refresh token | "Refresh depois de 30 min" e "deslogar após 40 min sem acesso" não definem validade do refresh token nem o que é "acesso". | Definir: access token 30 min, refresh token com expiração deslizante de 40 min por inatividade. |
| REQ-005 Recuperação de senha | Provedor de e-mail indefinido; enviar senha por e-mail é inseguro. | Trocar por link de redefinição com token de uso único válido por 10 min. |
| REQ-006 Dashboard | "Consumo por procedures" conflita com EF Core; biblioteca de gráficos indefinida; cores por categoria sem regra. | Definir se procedures são obrigatórias e quais métricas cada gráfico exibe. |
| REQ-007 Relatórios | Biblioteca/formato do PDF e colunas do relatório indefinidos. | Definir colunas, agrupamentos e layout do PDF. |
| REQ-008 Categorias | Critérios de aceitação copiados do cadastro de usuário. | Reescrever os CA (ex.: nome < 3 caracteres é rejeitado). |
| REQ-009 a REQ-011 | Regras e critérios copiados do cadastro de usuário; paginação/ordenação/filtros sem definição. | Definir filtros, tamanho de página, ordenação padrão e formato de erro. |
| SPEC-005 API | Documenta apenas `POST /auth/register`. | Documentar `POST /auth/login` e o formato de erro (ProblemDetails). |

## Como executar

```bash
cp .env.example .env   # preencha os valores
docker compose up --build
# Frontend: http://localhost:8080
```

Desenvolvimento local: suba um PostgreSQL e defina `ConnectionStrings__Default` e `Jwt__Key`
como variáveis de ambiente da API (`http://localhost:5270`) e rode o `WeMoney.Web` (`http://localhost:5088`).
