# Instruções para o agentes

## Geral

- Sempre utilizar o sistema de questões do ambiente (harness) ao invés de fazer perguntas na resposta.

## Fluxo de trabalho

Executar o fluxo completo em uma **feature branch**, seguindo a ordem **PRD → TDD → SDD → Pull Request → integração na `main`** para mudanças no produto. Não desenvolver diretamente na `main`:

1. **PRD:** definir o problema, escopo, requisitos e critérios de aceite.
2. **TDD:** detalhar as decisões técnicas e o roadmap de implementação com base no PRD.
3. **SDD:** detalhar a solução a ser implementada, mantendo rastreabilidade com PRD e TDD. Se ainda não existir, elaborá-lo antes de abrir o PR; não presumir sua aprovação.
4. **Pull Request:** implementar na feature branch, apresentar a mudança, vincular PRD, TDD e SDD, registrar testes e riscos e solicitar revisão. Não tratar a abertura ou aprovação do PR como substituto das validações pendentes nos documentos.
5. **Integração:** após a revisão, as aprovações e as verificações necessárias, integrar a feature branch na `main` por meio do Pull Request. Não fazer merge com bloqueios pendentes nem contornar as proteções do repositório; se não for possível concluir a integração, informar o impedimento e o estado do PR.

Se uma etapa exigir revisão, atualizar os artefatos afetados antes de avançar, preservando a coerência entre eles.
