# Notas Manuais e Arquiteturais (Equipe / Humano / IA)

Esta pasta reúne a documentação perene e detalhada de arquitetura, código e design de jogo de *Santa Rita — A Noite das 03:17*.

Nenhum arquivo nesta pasta é sobrescrito por scripts automáticos do Graphify (`./scripts/graphify-refresh.sh` atualiza apenas `generated/`), mas **os agentes de IA têm a obrigação de consultar e manter estas notas atualizadas** sempre que a arquitetura evoluir.

## Índice de Notas Principais
- [[Visao_Geral_Arquitetura]] — Mapa dos 5 subsistemas principais, contratos de domínio e relações centrais do Capítulo 1.
- [[Catalogo_Completo_Scripts]] — Inventário detalhado de todos os **44 scripts C#** (~15.010 linhas), suas classes, structs, enums e responsabilidades.
- [[Mapa_Cenas_Zonas_E_Fluxo_Cap1]] — Topologia completa das **5 cenas Unity**, das **9 zonas/interiores** do `[[Capitulo1Director]]`, coordenadas UV do mapa e cadeia de investigação/flags (`[[Flags]]`).
- [[Guia_Trabalho_Com_IA]] — Como a equipe e os agentes de IA utilizam o Graphify, o Obsidian e a memória persistente no fluxo de desenvolvimento.
