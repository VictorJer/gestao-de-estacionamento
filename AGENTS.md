# Instruções para o agentes

## Geral

- Sempre utilizar o sistema de questões do ambiente (harness) ao invés de fazer perguntas na resposta.

## Fluxo de trabalho

Seguir a ordem **PRD → TDD → SDD → Pull Request** para mudanças no produto:

1. **PRD:** definir o problema, escopo, requisitos e critérios de aceite.
2. **TDD:** detalhar as decisões técnicas e o roadmap de implementação com base no PRD.
3. **SDD:** detalhar a solução a ser implementada, mantendo rastreabilidade com PRD e TDD. Se ainda não existir, elaborá-lo antes de abrir o PR; não presumir sua aprovação.
4. **Pull Request:** apresentar a implementação, vincular PRD, TDD e SDD, registrar testes e riscos e solicitar revisão. Não tratar a abertura ou aprovação do PR como substituto das validações pendentes nos documentos.

Se uma etapa exigir revisão, atualizar os artefatos afetados antes de avançar, preservando a coerência entre eles.
