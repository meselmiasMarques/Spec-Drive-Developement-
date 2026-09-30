
---

# 8. A parte mais importante: Definition of Done

Crie um arquivo:

`07-definition-of-done.md`

```markdown
# Definition of Done

Uma funcionalidade somente pode ser considerada concluída
quando todos os critérios abaixo forem atendidos.

## Código

- [ ] Código implementado
- [ ] Segue arquitetura definida
- [ ] Segue princípios SOLID
- [ ] Sem código duplicado desnecessário
- [ ] Tratamento adequado de erros

## Banco

- [ ] Migration criada
- [ ] Relacionamentos configurados
- [ ] Índices avaliados

## API

- [ ] Endpoint implementado
- [ ] Status HTTP correto
- [ ] Validações implementadas
- [ ] Contrato respeitado

## Segurança

- [ ] Autenticação validada
- [ ] Autorização validada
- [ ] Isolamento de dados validado

## Testes

- [ ] Teste unitário
- [ ] Teste de integração quando necessário
- [ ] Casos de erro testados
- [ ] Critérios de aceitação validados

## Documentação

- [ ] SPEC atualizada
- [ ] README atualizado quando necessário
- [ ] ADR criada quando houver decisão arquitetural relevante