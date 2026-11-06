# Sinuca — remover assentos vazios

Autor: @BBCONTTI, autoria assistida por Codex. Data: 06/11/2026, America/Sao_Paulo. Ambiente: Windows/Unity 6000.6.0f1, melhoria-minigames. Status: implementado.

Pedido: retirar cadeiras 3D vazias e manter apenas as ocupadas pelos personagens.

Contexto: memória/cofre, direção/skills artísticas e QA já consultados na revisão da sala; fonte atual confirma assentos ocupados incorporados nos sprites dos NPCs. Graphify query indisponível por CLI ausente.

- [x] BuildRoom ignora Kind.Chair, incluindo banquetas vazias; conserva todos os NPCs sentados e suas cadeiras desenhadas.
- [x] Walkable ignora assentos removidos para não deixar obstáculos invisíveis. Colisão dos atores e mesas continua vigente.
- [x] Unity recompilou sem erros. Inspeção nativa da sala atual: zero anchors de cadeiras independentes, quatro personagens sentados presentes.

Arquivos: SinucaRoom3D.Scenery.cs e SinucaRoom3D.cs; documentação/memória. Layout da exploração 2D preservado. Sem alterações em saves/cenas. Refresh Graphify indisponível; validate não pode certificar índice antigo. Validação numérica da circulação executada; sem nova build Player nesta alteração pequena.
