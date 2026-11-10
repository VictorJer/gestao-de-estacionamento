# PRD — Plataforma de Gestão de Estacionamento e Valet

**Status:** aprovado pelo solicitante em 07/11/2026 como documento revisado de requisitos do produto. Não equivale à aprovação de implementação ou lançamento; pendências comerciais, operacionais e de privacidade continuam abertas e dependem dos respectivos responsáveis.  
**Fontes:** `docs/entrevista.md` (entrevista fictícia), `README.md` (contexto), versão anterior deste PRD, escolhas do solicitante na entrevista sobre `docs/tdd/tdd.md`. O TDD contém propostas técnicas e referencia um PRD com caminho e numeração diferentes; esta revisão não o altera nem assume que seus contratos técnicos já foram aprovados.  
**Escopo:** MVP de uma unidade de valet; não há evidência de implantação, métricas históricas ou validação com usuários reais.  
**Resumo das mudanças desta revisão:** vistoria textual obrigatória na entrada e saída, sem fotos; titular e documento obrigatórios em cada entrada; tarifa versionada administrável por administrador com vigência e snapshot na entrada; valor fixado na solicitação de retirada; vaga liberada na entrega; entrega normal registrada pelo caixa ou administrador; possibilidade de entrega excepcional com saldo mediante autorização segregada, sem política comercial aprovada; relatórios básicos movidos para P1. Ticket físico com ID permanece.

## 1. Resumo executivo

Centralizar o recebimento, localização, devolução e cobrança de veículos em uma operação digital. Pretende-se reduzir procura manual por carros, divergências de cobrança e esperas na retirada, com rastreabilidade de funcionários, vagas e receitas. O MVP inclui operação e financeiro básico, ticket físico e registro textual das condições do veículo. Valor da tarifa inicial, política de exceções financeiras, privacidade e procedimentos de contingência precisam de aprovação antes da operação real.

## 2. Contexto e problema

Segundo Alexandre Rech, o processo atual usa papel e planilhas: entrada anotada à mão, ticket numerado, verificação informal de vagas, solicitações transmitidas verbalmente e cálculo manual de diárias. Há relatos de ticket perdido, demora em localizar veículos, divergências de cobrança e reclamações de danos. São evidências qualitativas da entrevista fictícia, não taxas medidas. Operação, administração e clientes sofrem, respectivamente, com retrabalho, pouca confiança no caixa e espera/baixa transparência.

## 3. Objetivos e métricas de sucesso

| Objetivo | Indicador e medição proposta | Meta |
| --- | --- | --- |
| Reduzir espera na retirada | Tempo entre solicitação e entrega, obtido dos registros por atendimento; comparar com linha de base a coletar | A definir: meta e período |
| Reduzir divergências financeiras | Quantidade e valor de saldos pendentes, inclusive após entregas excepcionais, e diferenças entre cobrança ajustada e pagamentos, por período | A definir: meta e linha de base |
| Melhorar localização e ocupação | Percentual de atendimentos ativos com vaga identificada; ocupadas/total de vagas no momento da consulta | A definir: meta e capacidade |
| Acompanhar condições dos veículos | Percentual de atendimentos com vistorias textuais de entrada e saída | 100% das entradas e entregas concluídas no sistema; qualidade do conteúdo deve ser validada com operação |
| Reduzir erro de cobrança | Ajustes/contestações registrados, comparados ao histórico manual se disponível | A definir: medição e meta |
| Acompanhar atividade (P1) | Atendimentos por período e distribuição de entradas por faixa de horário | A definir após definição dos recortes |

## 4. Público-alvo e envolvidos

- **Cliente do valet:** recebe ticket físico com ID; não há autosserviço definido. Na entrada, informa titular e documento para registro; procedimentos de conferência presencial e aviso de privacidade precisam de validação.
- **Atendente/caixa:** registra entrada, aloca vaga, confere ticket/características, solicita retirada, registra cobrança/pagamento e confirma entrega normal; consulta operação.
- **Manobrista:** consulta veículos, vagas e fila; registra início da busca e vistoria textual de saída; não registra pagamento nem acessa relatórios financeiros. A responsabilidade física de deslocar o veículo não equivale, nesta revisão, a liberar a vaga no sistema antes da entrega.
- **Administrador:** acompanha operação, gerencia usuários e tarifas, acessa relatórios P1, concede autorizações excepcionais e pode executar ações operacionais autorizadas, respeitada a segregação de quem autoriza e quem solicita/confirma a entrega excepcional.
- **Alexandre Rech:** fonte da entrevista fictícia; sua responsabilidade formal por aprovações futuras é a definir. Tiago Santini é entrevistador, não patrocinador confirmado.

## 5. Escopo e prioridades

**MVP (P0):** atendimento por ID/placa, ticket físico com ID, registro obrigatório de titular/documento e vistorias textuais na entrada e saída (inclusive “sem observações”), vagas e ocupação até a entrega, fila e estados da retirada, conferência presencial, tarifa versionada pelo administrador com vigência e snapshot na entrada, diárias iniciadas de 24 horas até a solicitação, cobrança fixa nesse instante, pagamento integral normal, histórico e três perfis. Retirada sem ticket exige autorização administrativa segregada. A capacidade técnica de registrar entrega excepcional com saldo depende de política comercial aprovada; **sem essa política, não habilitar a exceção em produção**.

**P1, após P0:** relatórios básicos diário/semanal/mensal de volumes, faixas de entrada, ocupação, totais cobrados, pagos e pendentes; recortes e critérios contábeis precisam de aprovação. P1 não compõe o aceite funcional de P0; não anunciar capacidade de relatórios antes de sua entrega.

**Possíveis evoluções não aprovadas:** pagamentos parciais como fluxo normal, múltiplas regras de tarifa/descontos, ticket digital, integrações com pagamento/hardware e análises avançadas. Ajustes e estornos compensatórios são conceitos presentes no TDD, mas suas regras comerciais e aceitação funcional ainda não foram aprovadas neste PRD.

**Fora do MVP:** fotos/anexos de vistoria, aplicativo de cliente, reservas, reconhecimento automático de placas, comprovante fiscal eletrônico, processamento por provedor de pagamento e operação offline.

## 6. Jornadas e cenários de uso

1. **Entrada:** caixa registra placa, modelo, titular, documento, horário e vistoria textual, inclusive “sem observações” se aplicável; o sistema gera ID único para ticket físico, captura a versão vigente da tarifa e associa vaga livre. Vaga já ocupada não pode ser atribuída. O que fazer sem vagas é pendente.
2. **Localização e solicitação:** caixa localiza atendimento ativo por ticket/ID ou placa. Com ticket, confere ID e características do veículo; registra conferência presencial e pedido. Nesse instante o sistema fixa duração, diárias e cobrança conforme tarifa capturada na entrada. Fila mostra aguardando, em busca e entregue, com responsáveis e horários; manobrista registra início da busca. A vaga continua ocupada no sistema até a entrega.
3. **Pagamento e entrega normal:** caixa registra quitação integral do saldo exigível; vistoria textual de saída, inclusive “sem observações”, deve estar registrada. Caixa ou administrador confirma a entrega real, com horário e autor; sistema encerra ocupação e libera vaga na mesma operação. Tentativa de entrega sem quitação, vistoria ou conferência falha. A fila e a ocupação passam a refletir a conclusão.
4. **Sem ticket:** caixa encontra por placa e confronta documento/titular e características presencialmente; registra resultado. Administrador diferente do solicitante e do confirmador da entrega autoriza o caso, com justificativa e horário. A conferência não prova titularidade; forma adicional de verificar identidade permanece pendente. A entrega normal ainda requer quitação integral e vistoria de saída.
5. **Exceção de saldo:** possibilidade de administrador distinto da solicitação e da confirmação da entrega autorizar entrega com saldo e justificativa, mantendo saldo pendente e trilha auditável; motivos admissíveis, limites, cobrança posterior e efeitos de estorno ainda não definidos. Até aprovação da política, esse fluxo não tem aceite nem deve ser habilitado para uso real.
6. **Acompanhamento P1:** administrador consulta volumes, entradas por faixa horária, ocupação e totais cobrados, pagos e pendentes por dia, semana ou mês, inclusive saldos de entregas excepcionais, conforme recortes financeiros aprovados.

## 7. Requisitos funcionais

IDs RF-01 a RF-11 preservados para continuidade deste PRD; RF-12 foi adicionado para tarifas. A numeração citada no TDD atual **não corresponde** a esta tabela e deve ser reconciliada separadamente antes da implementação.

| ID / prioridade | Requisito | Critérios de aceite verificáveis |
| --- | --- | --- |
| RF-01 / P0 | Registrar entrada e identificar atendimento | Ao salvar placa, modelo, titular, documento, horário e vistoria textual de entrada, criar ID único por entrada e exibi-lo para ticket físico; permitir nova entrada histórica da mesma placa; buscar por ID ou placa. Sem campos obrigatórios, negar entrada. Formato, verificação e retenção do documento ainda dependem de política aprovada. |
| RF-02 / P0 | Registrar condições do veículo | Exigir observação textual de entrada e de saída, admitindo “sem observações”; vincular momento, horário e autor; impedir entrada sem vistoria inicial e entrega sem vistoria final. Não exigir nem oferecer fotos no MVP; manter registros consultáveis no histórico. |
| RF-03 / P0 | Gerenciar vagas | Mostrar vagas livres/ocupadas e veículo associado; impedir dupla atribuição mesmo em solicitações concorrentes; associar vaga livre a atendimento ativo e refletir a alteração após confirmação. |
| RF-04 / P0 | Liberar vaga somente na entrega | Início da busca não libera a vaga; ao confirmar entrega, encerrar vínculo de ocupação e mostrar vaga livre na consulta seguinte; impedir segunda liberação/entrega e preservar horário e autor. A divergência entre ocupação lógica e carro fisicamente removido durante a busca é risco a validar com operação. |
| RF-05 / P0 | Controlar solicitações de retirada | Exigir conferência presencial registrada; para ticket apresentado, comparar ID e características; criar no máximo um pedido ativo por atendimento, com horários de solicitação, início da busca e entrega, e estados aguardando → em busca → entregue. Possibilidade de entrega direta a partir de aguardando, cancelamento e reversão requerem validação operacional. |
| RF-06 / P0 | Calcular e fixar diárias na solicitação | Exibir prévia de tempo, diárias e total até a solicitação; para duração positiva, cada bloco iniciado de 24 h desde entrada conta uma diária (exatas 24 h = uma; acima de 24 h = duas). Na criação do pedido, fixar horário, quantidade, tarifa vigente **capturada na entrada** e total; espera posterior e hora do pagamento não alteram a base. Aceite do valor final depende de aprovação da tarifa inicial, moeda e arredondamento. |
| RF-07 / P0; exceção condicionada | Registrar cobrança, pagamento e saldo | Vincular cobrança fixa e pagamento ao atendimento, com horário e autor. Pagamento normal é integral do saldo devido; recusar parcial e duplicado; só concluir entrega normal com quitação integral. **Condicional:** entrega com saldo requer autorização administrativa segregada, justificativa e preservação do saldo pendente; critérios de uso e cobrança posterior não aprovados, portanto fluxo excepcional bloqueado para lançamento. Ajustes/estornos não são fluxo aprovado até definição comercial. |
| RF-08 / P0 | Controlar retirada sem ticket | Localizar por placa, confrontar documento/titular e características e registrar resultado; exigir aprovação de administrador distinto de quem solicitou e de quem confirma entrega, com justificativa/horário. Negar entrega sem autorização; a entrega normal também exige quitação e vistoria. Não tratar a conferência como prova suficiente de propriedade. |
| RF-09 / P0 | Confirmar entrega e manter histórico | Apenas caixa ou administrador confirma entrega real após pré-condições de conferência, vistoria, pagamento ou exceção aprovada; encerrar ocupação nesse ato; registrar horário e autor. Histórico consultável conserva entrada, tarifa capturada, vistoria, vaga, pedido, cobrança, pagamentos, aprovações e entrega. |
| RF-10 / P1 | Disponibilizar relatórios básicos | Somente administrador consulta recortes diário/semanal/mensal: quantidades, entradas por faixa horária, ocupação atual, cobrado, pago e pendente, inclusive saldo pós-entrega. Resultados reconciliáveis com lançamentos individuais. Datas de referência, faixas e fechamento ainda pendentes; não declarar este critério aceito antes de sua definição. |
| RF-11 / P0; relatórios P1 | Restringir acesso por perfil | Manobrista consulta operação e registra início de busca/vistoria de saída, sem cobrança, pagamento, documento completo ou relatórios financeiros; caixa registra entrada, vaga, pedido, conferência, pagamento e entrega normal; administrador gerencia usuários/tarifas, consulta operação e relatórios P1, autoriza exceções e pode operar, exceto aprovar a própria solicitação ou entrega excepcional. Negar operações fora do perfil no servidor e testar segregação por usuário. |
| RF-12 / P0 | Administrar tarifas versionadas | Apenas administrador cadastra novo valor e início de vigência; preservar versões anteriores e não alterar tarifa já capturada por atendimento. Ao registrar entrada, aplicar versão vigente e guardar o snapshot; em pedido posterior, usar o snapshot mesmo se existir tarifa nova. Valor/moeda iniciais, regra de vigência para conflitos e precisão monetária a aprovar antes da cobrança real. |

## 8. Requisitos não funcionais

| ID | Requisito | Verificação / pendência |
| --- | --- | --- |
| RNF-01 | Identificação e segregação | Mudanças exigem usuário identificado; testar matriz RF-11 e impedir que autorizador de exceção seja solicitante ou confirmador da entrega, inclusive quando todos têm papel de administrador. |
| RNF-02 | Integridade e rastreabilidade | Impedir dupla ocupação, cobrança/pagamento duplicados, dupla entrega e perda de autoria sob concorrência; auditoria consultável após entrega. Prazo de retenção a definir. |
| RNF-03 | Privacidade de dados e vistorias | Restringir consulta de placa, titular, documento e textos de vistoria a perfis autorizados; decidir acesso, proteção, base legal, retenção e descarte antes do lançamento. Não há fotos no MVP. |
| RNF-04 | Usabilidade e acessibilidade | Entrada, busca, cobrança e entrega utilizáveis por teclado, com estados/erros textuais; validar com operadores. Padrão e dispositivos-alvo a definir. |
| RNF-05 | Desempenho e disponibilidade | Consultas de vaga/fila refletem comandos confirmados inclusive entre usuários; testar concorrência. Metas de resposta, volume, disponibilidade e contingência de rede a definir. |
| RNF-06 | Observabilidade | Identificar falhas em entrada, vaga, transições, cálculo e pagamento sem expor documento ou vistoria indevidamente; mecanismo, alertas e indicadores a definir. |

## 9. Regras de negócio e dados

- **Atendimento:** ID único por entrada; placa não é chave única histórica. Placa, modelo, titular, documento, horário e vistoria textual de entrada são obrigatórios. Documento não é prova automática de titularidade; formato, coleta, acesso e conservação exigem definição de privacidade. Ticket físico contém ID gerado.
- **Vaga:** código único; no sistema, permanece ocupada do check-in até a confirmação da entrega, embora o carro possa já ter saído fisicamente da vaga. Impedir alocação simultânea e validar com operação o impacto dessa diferença na capacidade disponível.
- **Vistorias:** textos obrigatórios na entrada e saída, inclusive registro explícito “sem observações”; sem fotos. Registro de autor e instante em cada momento.
- **Retirada e conferência:** pedido após conferência presencial; fila aguardando → em busca → entregue, com marcos horários. Sem ticket, conferir documento/titular e características e exigir admin diferente do solicitante e do confirmador. Regras de divergência, entrega direta sem início de busca e reversões a definir.
- **Tarifa:** valor versionado pelo administrador com início de vigência; capturar versão/valor na entrada. Não aplicar retroativamente versão nova a atendimento aberto. Valor, moeda, arredondamento e regras de vigência para cadastros conflitantes dependem de aprovação.
- **Diárias:** para duração positiva, `diárias = máximo(1, teto((solicitação - entrada) / 24 horas))`; `cobrança base = diárias × tarifa capturada na entrada`. Pedido fixa esse valor; pagamento posterior não abre outra diária. Instantes devem permitir testar limites de 24 horas; política para horários inconsistentes a definir.
- **Pagamento e exceções:** fluxo normal quita integralmente a cobrança/saldo antes da entrega. Uma autorização distinta pode excepcionalmente permitir entrega com saldo, mantendo pendência e histórico, mas motivos, limites e procedimento de acerto precisam de aprovação para liberar esse fluxo. Estornos/ajustes compensatórios não são autorização implícita para pagamento parcial normal.
- **Relatórios P1:** derivam de atendimentos e lançamentos; atribuição temporal de cobranças, pagamentos e saldos e tratamento de períodos cruzados precisam ser aprovados por financeiro.
- **Auditoria:** registrar ator, horário e ação para entrada, vistoria, tarifa, vaga, conferência, pedido, cobrança, pagamento, autorização excepcional e entrega. Histórico deve sobreviver à desativação do usuário.

## 10. Dependências, premissas e restrições

**Confirmado pelo solicitante nesta revisão:** ticket físico; documento/titular em todas as entradas; vistoria textual obrigatória nos dois momentos, sem fotos; com ticket conferir ID e características; tarifa versionada administrada com vigência e snapshot na entrada; cálculo fixado na solicitação; ocupação até entrega; caixa/admin confirmam entrega; autorização excepcional segregada; relatórios P1. O TDD registra preferências técnicas, mas mudanças de produto acima são requisitos deste PRD, não aprovação automática de arquitetura/implantação.

**Premissas a validar:** vagas e usuários cadastrados, acesso ao sistema na entrada/saída, forma de emissão do ticket, identificação do cliente, dispositivos e conectividade. Sem provedor de pagamento, hardware ou integração fiscal definidos. Processo de migração de registros anteriores não definido.

## 11. Riscos e mitigação

| Risco | Mitigação proposta |
| --- | --- |
| Documento coletado sem política de privacidade definida | Designar responsável, decidir finalidade, acesso, retenção e descarte antes de produção; minimizar exposição. |
| Conferência presencial/administrativa não prova propriedade | Definir verificação adicional para retirada sem ticket e registrar quem realizou cada etapa. |
| Vistoria textual obrigatória sem fotos pode ser insuficiente em disputas | Validar roteiro de registro e conteúdo com operação; avaliar evidência adicional em evolução posterior. |
| Vaga marcada ocupada enquanto veículo está em busca pode ocultar capacidade física | Validar regra com operação e medir impacto; não mudar momento de liberação sem nova decisão de produto. |
| Tarifa muda durante atendimento e há disputa sobre cobrança | Preservar snapshot de entrada e vigência, comunicar regra ao cliente e aprovar valor inicial. |
| Entrega com saldo sem regras comerciais pode criar perdas | Manter recurso bloqueado até aprovar motivos, limites, acompanhamento e cobrança posterior. |
| Relatórios P1 indisponíveis no início e sem critério temporal | Não prometer relatórios no P0; definir recortes e reconciliação antes da entrega do P1. |
| Falha de rede e disputa concorrente de vagas/pagamentos | Testar conflitos e definir contingência e reconciliação antes do uso real. |

## 12. Questões em aberto e decisões pendentes

| Questão concreta | Responsável a confirmar | Impacto / estado |
| --- | --- | --- |
| Qual valor/moeda inicial, precisão, vigência e regra para tarifas conflitantes? | Gestão/financeiro | Bloqueia aceite do valor e faturamento real RF-06/12. |
| Quando e por quais motivos/limites pode haver entrega com saldo? Como cobrar depois e tratar ajuste/estorno? | Gestão/financeiro | **Bloqueia uso e aceite da exceção RF-07; não bloqueia entrega normal quitada.** |
| Como validar documento/titular e identidade de terceiros, especialmente sem ticket? | Gestão/privacidade/operação | Bloqueia política de conferência segura e mitigação da entrega indevida. |
| Que regra usar para conferência divergente, entrega direta sem início de busca e cancelamento/reversão? | Operação/gestão | Bloqueia critérios de aceite desses caminhos; não presumir permissões do TDD. |
| Como tratar ausência de vaga e diferença entre retirada física e vaga ocupada até entrega? | Operação/gestão | Bloqueia política de capacidade/espera e validação do RF-04. |
| Qual finalidade, base legal, política de acesso e retenção/descarte de documentos, vistorias e histórico? | Responsável por dados/gestão | Bloqueia aprovação de privacidade e lançamento. |
| Quais recortes, faixas, data de referência e fechamento dos relatórios P1? | Gestão/financeiro | Bloqueia aceite do RF-10, não o fluxo P0. |
| Quais dispositivos, capacidade, metas de desempenho, backup, contingência e critérios de implantação? | Operação/engenharia | Bloqueia prontidão de lançamento. |
| Como registrar meio de pagamento e conciliar caixa físico com pagamentos no sistema? | Gestão/financeiro | Não confundir registro de quitação com prova de recebimento externo. |
| Quem patrocina implantação e aprova metas e linha de base? | Patrocinador a confirmar | Impede afirmar resultados quantitativos ou aceite de produção. |

## 13. Critérios de lançamento e validação

- Apresentar o PRD aprovado pelo solicitante à operação, gestão, financeiro e responsável por dados para validar as respectivas políticas; esta aprovação não substitui definições de tarifa, documento, exceções e contingência. Não disponibilizar entrega com saldo antes de aprovar política comercial e testes de autorização segregada.
- Para **P0**, testar entrada com documento e vistoria textual (inclusive “sem observações”), ticket físico com ID, vigência e snapshot da tarifa, disputa de vaga, pesquisa ID/placa, conferência com/sem ticket, pedido em 24 h exatas e acima, valor congelado mesmo com pagamento tardio, pagamento normal integral e rejeição de parcial/duplicado, vistoria de saída, entrega por caixa/admin, liberação apenas na entrega, autorização por admin diferente dos operadores e histórico completo.
- Testar acessos dos três perfis e capacidade sob concorrência; obter aprovação dos responsáveis de privacidade, operação e negócio e um plano de continuidade de atendimento em falha de conectividade. P0 funcional em homologação não significa liberação de produção.
- Para **P1**, antes de anunciar relatórios, aprovar critérios temporais e reconciliar amostras de atendimentos, cobranças, pagamentos e pendências (inclusive eventuais entregas excepcionais).
- Após lançamento, coletar linhas de base de espera, ocupação, pendências financeiras, contestação de cobranças e qualidade da vistoria; só então estabelecer metas quantitativas com a gestão.
