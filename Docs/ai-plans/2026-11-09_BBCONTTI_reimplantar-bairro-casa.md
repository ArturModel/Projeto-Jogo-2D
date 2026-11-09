# Nova implantação do Bairro e da sala da casa nº 40

Autor @BBCONTTI. 09/11/2026. Unity 6000.6.0f1, URP 2D e interior 3D. Implementado e verificado no Editor.

O autor pediu liberdade para reconstruir a implantação de casas, ruas, postes, detalhes e mobiliário. O diagnóstico foi feito sobre o mapa do C1, memória, Obsidian, código do diretor e capturas da versão anterior em Docs/Evidencias/BairroEstradaMata.

- A estrada contínua faz a ligação da Encruzilhada à subida para a Rua Norte. Os ramais servem os lotes da Lurdes e da van, a casa 22, a casa 32 e o recuo da casa 48. A casa 40 fica no ponto de inflexão da via principal.
- Casas secundárias, muros, vasos, luminárias, poças, drenagem, van e pistas foram reposicionados. Os pátios de pedra são locais. A malha de 15 segmentos definida em SantaRitaRuaNorte.Bairro.cs serve o shader, o piso de contingência, a distribuição de vegetação e o detalhe de barro. 244 árvores e submata cercam os lotes e ramais.
- Na casa nº 40, mesa, tapete, cartas e poltrona formam o núcleo da sala à esquerda. Divisória do quarto, telefone e banco foram reposicionados para abrir a circulação central. As pistas mantêm seus tipos de interação.
- Arquivos principais: Capitulo1Director.cs, SantaRitaRuaNorte.Bairro.cs, LuzVolumetrica.shader e Posto3D.CasaDoAvo.cs.

## Verificação

Capturas em Docs/Evidencias/BairroReimplantacao, com cinco trechos em escala normal, vista geral e interior 3D. A transição para o Bairro foi provocada pelo diretor. Os quatro pontos de interação e três gatilhos foram encontrados. A amostragem do eixo viário encontrou apenas o perímetro depois da faixa do gatilho norte e a van na extremidade da própria garagem. Shader sem erros reportados. O interior foi instanciado pelo mesmo construtor do jogo em fixture, com cartas e telefone no lugar e passagem central livre em teste de esfera. Não houve validação de executável standalone nem campanha completa.

Graphify query e refresh indisponíveis por falta da CLI. O validador apontou arquivos ausentes no índice existente.
