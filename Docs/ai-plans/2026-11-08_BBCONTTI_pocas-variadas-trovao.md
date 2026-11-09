# Poças variadas e trovão da Entrada

Registro de @BBCONTTI em 08/11/2026, Unity 6000.6.0f1 e URP 2D.

A composição anterior repetia poças de tile semelhantes. O autor pediu mais variação de forma, escala, distância, brilho e azul, além de um trovão com cauda depois do clarão.

`SantaRitaRuaNorte.cs` monta onze poças em coordenadas da Rodovia, com contornos recortados, inclinações, tons e intensidades próprios. `LuzVolumetrica.shader` usa a nova máscara para cor, brilho, rugosidade e reflexo animado. A captura em Play está em `Projeto Jogo 2D/Docs/Evidencias/EntradaDaCidade/entrada-pocas-variadas.png`.

O evento de raio agenda o trovão com atraso de 0,20 s. Uma fonte local toca o recorte de 6,2 s, respeita o ajuste de efeitos, pausa com o jogo e é interrompida ao sair da Rodovia. O asset `TrovaoEntrada.ogg` veio de uma gravação de Bidgee, sob CC BY 3.0, com edição documentada em `ASSET_SOURCES.csv`. O pico medido depois da codificação foi de −2,2 dBFS.

O executável e a avaliação auditiva do autor permanecem pendentes. O Graphify segue indisponível no ambiente e o validador acusa lacunas no índice existente.
