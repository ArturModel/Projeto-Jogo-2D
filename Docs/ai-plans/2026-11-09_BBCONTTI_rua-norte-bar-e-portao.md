# Rua Norte do Bar do Zé e acesso ao Centro

- Autor @BBCONTTI. 09/11/2026. Unity 6000.6.0f1, Windows, URP 2D.
- Status: composição implementada com validação em Play Mode e revisão de acabamento posterior.

## Solicitação e diagnóstico

Recompor todo o exterior conforme as quatro zonas anteriores, criar folha de sprites, integrar iluminação e mata e dar função espacial ao portão. A revisão do autor apontou sobreposição de objetos, o abrigo sem estrutura e o entorno imediato da subida. A arquitetura, o mapa e as skills de arte e QA foram consultados. O Graphify está indisponível no ambiente.

## Execução

- [x] Recuo para a picape, pátio à esquerda, faixa livre diante da porta, abrigo de serviço com dois apoios, orelhão independente e coleta atrás da ala leste.
- [x] Subida curvada no eixo do portão. Duas alas de pedra, pilares com bases físicas e bloqueio transversal ligado à condição existente da sinuca.
- [x] Quatro postes com feixes, sombra por silhueta e volume, reflexos molhados, duas camadas de clima e 65 árvores com material de copa e submata nas margens.
- [x] Seis recortes de uma folha própria e um portão adicional. Sete relevos de materiais, procedência e fontes salvas em Arte/Producao/EntradaDaCidade.
- [x] Correção da reaplicação dos sprites de reserva por SantaRitaPlaceDressing.

O diretor mantém geometria física, gatilhos e interação. SantaRitaRuaNorte.Bar define piso e composição. LuzVolumetrica modela as transições de terreno na aproximação e na curva. O interior e a lógica de progressão são os já usados pelo jogo.

## Validação

[Capturas](../Evidencias/RuaNorteBarPortao/) feitas com a zona real ativa em Play Mode. A versão de composição conferida carregou os sete recortes e mapas, recuperou Mesa e Cadeira, exibiu os dois renderers de clima e o mapa de sombras e manteve os cinco pontos de interação com aproximação livre. A amostragem de colisão no eixo viário e na aproximação da subida não encontrou obstáculos. As colisões encontradas no passeio pertencem à picape, aos postes e ao orelhão. A barreira fechada bloqueia no eixo do portão. Após as capturas, o barril foi movido para perto das mesas e a transição de barro da entrada oeste ganhou relevo e sulcos.

O refresh Graphify falhou por ausência da CLI. A validação do grafo acusa fontes ausentes no índice, incluindo a nova parcial e pendências anteriores. A verificação ocorreu no Editor, sem build executável ou execução completa da campanha.
