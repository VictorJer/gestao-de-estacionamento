---
name: generete-tdd
description: Criar um documento técnico de design (TDD) detalhado para um sistema ou funcionalidade, entrevistando o usuário sobre arquitetura, estrutura, linguagem, framework e bibliotecas e centralizando as decisões técnicas. Use ao receber /generete-tdd ou pedidos para elaborar um design técnico.
---

# Gerar documento técnico de design

Produza um TDD (Technical Design Document) em português, pronto para revisão por engenharia. Esta skill pode ser usada diretamente ou por `/generete-tdd <descrição>`. TDD aqui significa **documento técnico de design**, não desenvolvimento orientado a testes.

## Fluxo

1. Entenda o sistema ou funcionalidade, seus limites e objetivos. Se faltar uma descrição utilizável, pergunte antes de criar qualquer arquivo. Consulte o contexto relevante do repositório (por exemplo, `README.md`, `docs/prd/`, código, configurações, especificações e designs existentes). Quando houver PRD, relacione o design aos requisitos identificados; não transforme hipóteses do PRD em decisões aprovadas. Diferencie fatos verificados no projeto, preferências do usuário, propostas e pontos ainda em aberto. Não trate menções a tecnologias no README como aprovação definitiva sem verificar ou perguntar.
2. **Entreviste o usuário antes de finalizar o TDD.** Use a ferramenta `question` para perguntas objetivas, agrupadas em rodadas curtas por tema, com alternativas e opção livre quando fizer sentido. Pergunte explicitamente, conforme o escopo:
   - Arquitetura: estilo (monólito modular, serviços etc.), limites de componentes, responsabilidades, comunicação, integrações e decisões de hospedagem.
   - Estrutura: organização de repositório e módulos, camadas, dependências permitidas, convenções e contratos entre partes.
   - Stack: linguagem e versões quando relevantes, runtime, frameworks (backend/frontend), banco de dados, bibliotecas e justificativas para escolhas significativas.
   - Qualidades e operação: segurança e autorização, privacidade, persistência, concorrência, desempenho, escalabilidade, observabilidade, implantação e testes, na medida pertinente.
   Apresente opções compatíveis com o contexto, com trade-offs concretos, sem impor preferência arbitrária nem introduzir bibliotecas só para preencher a lista. Pergunte quais tecnologias existentes são restrições obrigatórias e quais podem mudar. Se uma escolha estiver delegada a você, marque-a como **proposta técnica** e explique o critério; se o usuário não souber decidir, registre como **A definir** com impacto e responsável quando conhecido. Faça outra rodada se uma resposta mudar materialmente o desenho. Não silencie conflitos entre fontes ou finja que uma decisão pendente foi aprovada.
3. Grave o documento por padrão em `docs/tdd/tdd.md`, independentemente do assunto. Só use outro nome ou caminho se o usuário pedir explicitamente, desde que seja Markdown dentro do projeto. Antes de escrever, confira se o arquivo já existe; não sobrescreva nem modifique um TDD existente sem autorização explícita. Em caso de conflito, pergunte com `question` se deve atualizar o arquivo ou usar outro nome.
4. Escreva um design detalhado, mas proporcional ao escopo, incorporando as respostas. Organize pelo menos:
   - **Resumo e status**: objetivo, escopo, público técnico, data se pertinente e estado (rascunho/revisão/aprovado somente com aprovação explícita).
   - **Contexto e rastreabilidade**: referências a PRD/requisitos/especificações existentes, restrições, premissas, fora de escopo e situação atual versus proposta.
   - **Decisões técnicas centralizadas**: tabela ou registro único com IDs (`DT-01`, ...), tema, decisão, status (confirmada pelo usuário, proposta ou pendente), origem, alternativas consideradas, justificativa/trade-offs e impacto. Inclua linguagem, frameworks, bibliotecas, arquitetura e estrutura; indique versão apenas se confirmada ou justificada. Referencie os IDs nas demais seções para evitar decisões contraditórias espalhadas pelo texto.
   - **Arquitetura e estrutura**: componentes, responsabilidades, limites, dependências, organização de módulos/pastas e fluxos principais. Sugira diagramas Mermaid para tornar o design mais claro; quando úteis, inclua diagramas de componentes, sequência ou fluxo, com legendas e nomes consistentes com o texto.
   - **Contratos, dados e integrações**: interfaces e eventos relevantes, modelos/entidades e relacionamentos, persistência, migrações, consistência, erros e idempotência quando aplicáveis. Sugira diagramas Mermaid de entidades e relacionamentos ou fluxos de dados quando ajudarem a explicar essas partes. Diferencie exemplos propostos de contratos aprovados.
   - **Requisitos de qualidade**: segurança/autorização, privacidade, desempenho, disponibilidade, observabilidade e acessibilidade conforme pertinência, com critérios verificáveis apenas onde houver base para defini-los.
   - **Entrega e validação**: implantação/configuração, compatibilidade e migração, estratégia de testes, cenários críticos, rollout e reversão quando aplicáveis.
   - **Riscos, alternativas e questões pendentes**: riscos técnicos e mitigação; para cada pendência, pergunta específica, impacto nas partes do design e próximo passo. Não invente números, SLAs, versões, tecnologias ou integração inexistentes.
5. Revise a coerência entre decisões, diagramas, contratos, requisitos e testes. Se houver dúvida que altere a arquitetura ou a stack, retome a entrevista. Não apresente o documento como pronto para implementação sem destacar bloqueios técnicos pendentes. Não implemente código nem edite PRDs, especificações ou outros arquivos como parte desta geração.
6. Confirme o caminho do TDD criado e resuma decisões confirmadas, propostas e pendências mais importantes.
