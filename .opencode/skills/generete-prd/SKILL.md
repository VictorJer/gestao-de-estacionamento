---
name: generete-prd
description: Gerar um documento de requisitos de produto (PRD) completo a partir de um prompt inicial, considerando o contexto do projeto. Use ao receber /generete-prd ou pedidos para elaborar um PRD.
---

# Gerar PRD

Produza um PRD em português, pronto para revisão por produto, design, engenharia e demais envolvidos. Esta skill pode ser usada diretamente com uma descrição do usuário ou por meio de `/generete-prd <prompt inicial>`.

## Fluxo

1. Identifique o produto ou funcionalidade, o problema e o público a partir do prompt. Se não houver descrição utilizável, faça uma pergunta ao usuário antes de criar qualquer arquivo.
2. Consulte apenas o contexto relevante disponível no repositório (por exemplo, `README.md`, `docs/`, especificações e interfaces existentes). Separe fatos confirmados no projeto, informações fornecidas pelo usuário, hipóteses e decisões pendentes. Não apresente métricas, pesquisas, prazos, exigências legais ou funcionalidades inexistentes como fatos.
3. **Entreviste o usuário antes de finalizar o PRD.** Levante ambiguidades, contradições e lacunas nas especificações (público, problema, escopo, prioridades, fluxos, regras de negócio e critérios de aceite). Use a ferramenta `question` do ambiente para fazer perguntas objetivas, agrupadas por tema, com alternativas claras quando fizer sentido; preserve a opção de resposta livre para decisões que não caibam nas alternativas. Aguarde as respostas e faça novas rodadas se surgirem dúvidas relevantes. Não resolva ambiguidades de especificação por suposição silenciosa nem finalize o artefato enquanto houver ambiguidades que alterem requisitos, comportamento ou critérios de aceite. Se o usuário não souber decidir, registre explicitamente a questão como pendente, seu impacto e que o requisito afetado não está aprovado; não invente uma resposta.
4. Derive um identificador curto em kebab-case para o assunto e grave o documento em `docs/prd/<identificador>.md`. Se o usuário indicar um caminho de saída, use-o em vez do padrão, desde que seja um arquivo Markdown dentro do projeto. Antes de escrever, verifique se o arquivo já existe; não substitua nem altere um PRD existente sem autorização explícita. Se houver conflito, use `question` para perguntar se deve atualizar o existente ou usar outro nome.
5. Redija um PRD completo e proporcional ao tema, incorporando as respostas da entrevista. Inclua as seções abaixo; quando faltar informação que o usuário não conseguiu fornecer, marque `A definir` com a pergunta concreta e o impacto correspondente, em vez de inventar detalhes:
   - **Resumo executivo**: visão do produto, proposta de valor e status do documento.
   - **Contexto e problema**: situação atual, evidências disponíveis e impacto para o público.
   - **Objetivos e métricas de sucesso**: resultados desejados, indicadores, forma de medição e metas (ou `A definir`).
   - **Público-alvo e envolvidos**: perfis, necessidades e responsáveis conhecidos (ou `A definir`).
   - **Escopo**: entregas do MVP, fases futuras quando aplicáveis e explicitamente fora de escopo.
   - **Jornadas e cenários de uso**: fluxos principais, exceções relevantes e experiência esperada.
   - **Requisitos funcionais**: itens identificados (`RF-01`, ...), prioridade, descrição e critérios de aceite verificáveis por requisito.
   - **Requisitos não funcionais**: itens identificados (`RNF-01`, ...), cobrindo, conforme relevância, segurança, privacidade, acessibilidade, desempenho, disponibilidade, observabilidade e compatibilidade; critérios mensuráveis onde houver base para defini-los.
   - **Regras de negócio e dados**: regras, entidades e restrições importantes, sem inventar integrações ou arquitetura.
   - **Dependências, premissas e restrições**: integrações e condicionantes confirmados, separados dos supostos.
   - **Riscos e mitigação**: riscos de produto e de entrega com ações propostas.
   - **Questões em aberto e decisões pendentes**: perguntas específicas, responsáveis quando conhecidos e impacto no escopo.
   - **Critérios de lançamento e validação**: condições de pronto, estratégia de verificação e acompanhamento após lançamento.
6. Revise a coerência entre objetivos, escopo, jornadas, requisitos, critérios de aceite e métricas. Se a revisão revelar novas ambiguidades de especificação, retome a entrevista com `question` e atualize o PRD antes de encerrá-lo. Diferencie claramente requisito aprovado de proposta ou hipótese. Não implemente código nem altere outros documentos como parte desta geração.
7. Confirme que o arquivo foi criado e informe o caminho, um resumo breve e as principais pendências ao usuário.
