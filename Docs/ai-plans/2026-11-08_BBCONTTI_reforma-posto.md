# Reforma visual do posto

Autor: @BBCONTTI (identidade Git local), com Codex. Data: 08/11/2026, horário de Brasília. Ambiente: Unity 6000.6.0f1, projeto Santa Rita, cena EntradaDaCidade.

O pedido foi levar o acabamento da Entrada da Cidade e da Rua Norte do Bar do Zé ao quarteirão do posto e reformar o interior com a mesma linguagem híbrida, incluindo a soleira na chuva e as legendas.

A memória do projeto e o mapa de cenas do Obsidian situam o posto no mundo gerado em tempo de execução por Capitulo1Director. SantaRitaRuaNorte concentra a preparação de piso, luz e sprites. A consulta ao Graphify ficou indisponível porque o executável local não está instalado. A captura enviada pelo autor orientou a projeção, o tamanho dos objetos e o desenho da fachada.

- A zona do posto usa o Tilemap, as texturas de superfícies e a iluminação local da Entrada. O passeio mantém os recuos do posto e da venda e recebe ralos. Os quatro postes de luz usam o sprite da Entrada.
- Quatro sprites novos, registrados em ASSET_SOURCES.csv, substituem a fachada, a venda, a picape e o sedã. O banco vem da Entrada. As posições das interações e as rotas entre bairros permanecem no diretor.
- Arquivos do exterior: Capitulo1Director.cs, SantaRitaRuaNorte.cs, LuzVolumetrica.shader e Resources/Art/RuaNorte/Posto.
- O agente dedicado ao interior criou a conveniência com geometria 3D e personagens em sprites, animou a abertura da porta e conectou as conversas com o frentista e o motorista à apresentação compartilhada de legendas. O fluxo passa pelo diretor e pelo SceneFader. As alterações de interior ficam em Posto3D.cs, Posto3D.Cenario.cs, Capitulo1Director.cs, InteracaoCapitulo1.cs e SceneFader.cs.

O Unity compilou o exterior sem erros. A montagem foi conferida em Play Mode com Marcus posicionado por fixture em dois pontos do quarteirão. As capturas mostram o [posto e a picape](../Evidencias/PostoReforma/posto-exterior.png) e a [venda e o sedã](../Evidencias/PostoReforma/venda-e-sedan.png). A fixture usa a cena normal e não usa slots pessoais.

No Play Mode, uma fixture posicionou Marcus diante da porta. A tecla E acionou a transição normal, com chuva e mão na maçaneta. O teste percorreu 1,72 m com W, abriu as legendas do frentista e do motorista com E e voltou à calçada pela porta. A câmera 3D foi removida na saída e o personagem 2D foi reativado. A compilação terminou sem erros. Capturas da [porta](../Evidencias/PostoReforma/03-posto-porta.png), do [interior](../Evidencias/PostoReforma/04-posto-interior.png) e da [legenda](../Evidencias/PostoReforma/05-posto-legenda.png).

O refresh do Graphify falhou porque o executável exigido está indisponível. O validador identificou fontes ausentes no índice, incluindo os novos scripts do posto. A avaliação artística do conjunto fica com o autor.

