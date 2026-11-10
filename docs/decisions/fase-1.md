# Decisões e pendências — Fase 1

**Referências:** `docs/prd/PRD.md`, `docs/tdd/tdd.md` (aprovado em 10/11/2026), `openspec/changes/fundacao-infraestrutura-fase-1/`.

Esta fase usa desenvolvimento e testes locais/CI com dados sintéticos; **não provisiona nem publica recursos na Azure**. Os itens abaixo não impedem executar testes locais, mas não podem ser apresentados como decisões aprovadas de implantação.

| Decisão pendente | Responsável pela decisão | Estado e impacto |
| --- | --- | --- |
| Domínios da SPA e da API, configuração do proxy e topologia/rede Azure | Infraestrutura/segurança — pessoas a designar | Pendente; exigido antes de homologação hospedada na fase 7. O contrato local `/api` não prova isolamento Azure. |
| Fuso da unidade e formato de datas da operação | Operação/gestão — pessoas a designar | Pendente; testes usam instantes UTC e dados sintéticos, sem assumir fuso de produção. |
| Moeda, tarifa inicial, precisão e arredondamento | Negócio/financeiro — pessoas a designar | Pendente; bloqueia faturamento real, não o scaffold ou teste de sessão. |
| Volume, capacidade, dispositivos, disponibilidade e metas de latência | Operação/infraestrutura — pessoas a designar | Pendente; cenários e metas para piloto/carga hospedados na fase 7. |
| Política de coleta, acesso, retenção e descarte de dados | Responsável por privacidade/gestão — pessoas a designar | Pendente; não usar dados pessoais reais em testes e não liberar produção. |
| Backups, recuperação de chaves, RPO/RTO e contingência | Operação/infraestrutura/privacidade — pessoas a designar | Pendente; ensaios em Azure e aprovação são gates da fase 7. |
| Finalidade e base legal por categoria de dado; acesso e segregação de perfis; retenção/descarte e atendimento de titulares | Privacidade/gestão — pessoas a designar | Pendente; dados sintéticos apenas; bloqueia dados reais e produção. |
| Proteção, backup, restauração e rotação de certificados e chave de Data Protection | Segurança/infraestrutura — pessoas a designar | Pendente; volume Docker é apenas persistência local, não proteção/recuperação de produção. |
| Tempo máximo de sessão, política de bloqueio após tentativas, recuperação de acesso e revogação de contas | Segurança/gestão de pessoas — pessoas a designar | Pendente; valor de 8 horas no ambiente de desenvolvimento é técnico, não aprovado para produção. |
| Processo de backup, ensaio de restore, RPO/RTO e contingência com responsáveis de plantão | Operação/infraestrutura — pessoas a designar | Pendente; recuperação não foi homologada. |

## Checklist de liberação — assinaturas pendentes

| Gate de produção | Responsável pela aprovação | Assinatura/data | Situação |
| --- | --- | --- | --- |
| Política de dados (finalidade, base legal, acesso, retenção e descarte) | Privacidade/gestão | — | Pendente; bloqueia produção |
| Tarifa/moeda, arredondamento e regras comerciais | Negócio/financeiro | — | Pendente; bloqueia operação financeira |
| Fuso, capacidade e parâmetros de sessão/bloqueio | Operação/segurança | — | Pendente; bloqueia produção |
| Backup/restore, proteção/recuperação de chaves, RPO/RTO e contingência | Infraestrutura/segurança/operação | — | Pendente; bloqueia produção |
| Topologia Azure, ingress restrito, TLS, domínios e homologação real | Infraestrutura/segurança | — | Pendente; bloqueia publicação e produção |

Nenhuma linha foi assinada. Este checklist não constitui aceite tácito: responsáveis nominais e evidências devem ser anexados antes de mudar qualquer situação.

As decisões de negócio/privacidade/infraestrutura exigem validação pelos respectivos responsáveis; a aprovação do TDD pelo solicitante não as substitui. Atualizar esta tabela quando forem designados responsáveis e houver decisão documentada; não converter automaticamente campos pendentes em valores padrão.
