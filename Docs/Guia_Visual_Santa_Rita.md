# Guia visual de Santa Rita — Rodovia pós-prólogo

**Referência de produção · 08/11/2026 · Unity 6000.6.0f1 · URP 2D**

A Rodovia, primeira área após o prólogo, é a referência para iluminação, contraste e composição de **Santa Rita — A Noite das 03:17**. O exterior da Rua Norte documentado abaixo é um estudo de aplicação desse padrão. Reaplique a escala de Marcus, a continuidade dos materiais, a composição por função e a hierarquia de luz. Calibre a paleta, a umidade, a vegetação e a força dos efeitos conforme o lugar e a hora da cena.

![Rodovia após o prólogo, referência escolhida pelo autor](Referencias/EntradaDaCidade/rodovia-padrao.png)

![Rua Norte em Play Mode. A luz da porta recorta o bar e chega ao asfalto. Grama, calçada e pista mantêm leitura própria, com bordas irregulares.](Referencias/BarDoZe/rua-norte-final.png)

[Antes da revisão de continuidade e iluminação](Referencias/BarDoZe/rua-norte-antes.png). Os dois quadros são da câmera do jogo em posições próximas e em instantes diferentes da chuva. A referência complementa a [direção de arte e narrativa](Direcao_Arte_Narrativa_Audio.md) e os [critérios de produção](Producao_Artistica_Execucao.md).

## 1. O que faz a cena funcionar

| Decisão | Na Rua Norte | Regra para os próximos cenários |
| --- | --- | --- |
| Ponto de chegada visível | Letreiro, luminária, janela âmbar e soleira indicam a entrada. | Escolha um foco físico coerente com a função do lugar. Deixe acessos e pistas visíveis em movimento e nos cantos do quadro. |
| Contraste quente/frio | Reboco creme, telha de barro e luz âmbar contrastam com lambril azul-petróleo, asfalto carvão azulado e folhagem oliva. | Reserve calor e luminância para fontes e marcos. Faça sombra, matéria e reflexo responderem à mesma fonte. |
| Cotidiano brasileiro | Duas cadeiras de plástico e mesa redonda formam um grupo. Caixotes e contentor definem a área de serviço. Picape, poste e orelhão pertencem à rua. | Posicione cada objeto onde teria uso. Aproxime peças de um mesmo conjunto e deixe passagem entre os conjuntos. |
| Peso e profundidade | Raízes, rodas, pés do mobiliário e soleira assentam no piso. O bosque recua pela cor e pela névoa. | Combine pivô no apoio, sombra de contato, ordem por profundidade e contraste atmosférico. |
| Detalhe na escala do jogo | Rejunte, agregado, manchas, telhas e madeira continuam legíveis sem zoom. | Compare grupos de pixels, direção da luz e espessura dos contornos com Marcus. Use detalhe maior nas áreas que o jogador deve ler. |
| Material contínuo | Trincas, poças, desgaste e folhas quebram a repetição. O meio-fio continua legível. | O material deve atravessar células, e o detalhe deve aparecer onde existe uma causa física. Preserve desníveis e bordas construídas. |

**Composição.** De sul para norte, o quadro passa da rua e da picape para a faixa de circulação da calçada, o grupo da mesa à esquerda, a porta no centro, a área de serviço à direita e o bosque ao fundo. A fachada rural de 2004 é frontal elevada, com reboco gasto, telhado de barro, porta fechada de uma folha e maçaneta à direita. O orelhão laranja e os engradados têm cor própria, porém sua luz fica subordinada à da entrada. A vegetação alterna alturas e silhuetas. As moitas baixas aproximam o tronco e as bordas da calçada da grama.

## 2. Escala, câmera, pivôs e circulação

- **Marcus é a régua.** A pose lateral direita medida tem **16 × 48 px opacos** dentro de uma célula **64 × 64 px**, com pivô **(32, 8) px**, **32 pixels/unidade (PPU)** e altura visível **1,50 unidade**. O retângulo completo mede 2 × 2 unidades. Compare a silhueta útil em pelo menos uma pose frontal e uma lateral.
- **Câmera.** `EntradaDaCidade` configura Pixel Perfect em **512 × 288**, **32 PPU** e corte dos dois eixos. A zona pede tamanho ortográfico **5,5**. A amostra no Editor mediu **5,625** e aspecto **1,4222** após o ajuste da câmera. O tamanho ortográfico é metade da altura visível. Valide também no formato de tela alvo. O Canvas de interface usa referência **640 × 360**.
- **Exceção da Rua Norte.** Props e tiles novos usam **64 PPU**, com renderização Point. Em escala 1, **64 px = 1 unidade**. O importador genérico de arte pula a pasta `Art/RuaNorte` para respeitar os metadados locais. Use as dimensões em unidades para comparar um asset de 64 PPU com Marcus a 32 PPU.
- **Três medidas independentes.** O retângulo do sprite define a presença na tela. O colisor define a área que bloqueia a caminhada. O volume XYZ fornece a pegada e a altura para a sombra. A mesma cadeira tem altura desenhada de 0,81, colisor de 0,63 × 0,63 e oclusor de 0,45 × 0,38 × 0,78.
- **Pé e ordenação.** Use o contato com o chão como âncora de posição. O padrão das peças reordenadas pela Rua Norte é `1000 - round(y × 10)`. A cadeira direita é espelhada. Pivôs de orelhão e poste ficam sob a coluna real, deslocados do centro da imagem.

### Inventário dimensional do exterior

Medidas de sprite em Play Mode, arredondadas a duas casas. As coordenadas XY são âncoras no mundo. Os volumes são largura × profundidade × altura no shader. As medidas de arte incluem as bordas transparentes.

| Peça | Arte (px) | Sprite (un.) | Pivô (px) | Posição XY | Volume de sombra |
| --- | ---: | ---: | ---: | --- | ---: |
| Fachada | 526 × 314 | 8,22 × 4,91 | 263, 0 | 518; 14 | 7,20 × 3,10 × 3,80 |
| Picape | 276 × 132 | 4,31 × 2,06 | 138, 0 | 509,10; 10,95 | 3,90 × 1,35 × 1,65 |
| Mesa redonda | 73 × 62 | 1,14 × 0,97 | 36,5; 0 | 512,90; 13,70 | 0,86 × 0,66 × 0,76 |
| Duas cadeiras | 40 × 52 | 0,63 × 0,81 | 20, 0 | 512,26; 13,29 e 513,53; 13,40 | 0,45 × 0,38 × 0,78 |
| Engradado | 49 × 38 | 0,77 × 0,59 | 24,5; 0 | 521,55; 13,33 | 0,65 × 0,45 × 0,51 |
| Caixotes | 57 × 65 | 0,89 × 1,02 | 28,5; 0 | 522,80; 13,50 | 0,64 × 0,55 × 0,93 |
| Contentor | 91 × 66 | 1,42 × 1,03 | 45,5; 0 | 523,65; 14,07 | 1,24 × 0,70 × 0,91 |
| Orelhão | 63 × 124 | 0,98 × 1,94 | 18,27; 0 | 525,80; 13,20 | 0,25 × 0,25 × 1,40 |
| Dois postes | 72 × 216 | 1,13 × 3,38 | 12,24; 0 | 505,50; 13 e 527,50; 13 | 0,17 × 0,20 × 3,15 |
| Portão do Centro | 270 × 130 | 4,22 × 2,03 | 135, 0 | 532,50; 10 | 4,00 × 0,42 × 1,80 |
| Arbusto da rua | 100 × 59 | 1,56 × 0,92 | 50; 1,18 | 507; 13,60 | 0,95 × 0,60 × 0,65 |
| Moitas baixas | 100 × 59 | 0,77–1,14 × 0,45–0,67 | 50; 1,18 | Escalas 0,49 / 0,53 / 0,62 / 0,73 | Sem oclusor próprio |
| Árvore de copa larga | 238 × 250 | 3,02–3,79 × 3,18–3,98 | 116,62; 5,50 | Norte e sul, escalas 0,813–1,02 | 0,53 × 0,63 × 2,85, multiplicado pela escala |
| Araucária | 222 × 284 | 3,07–3,64 × 3,93–4,66 | 111; 6,25 | Norte e atrás do bar, escalas 0,885–1,05 | Mesmo perfil de árvore |
| Pitangueira | 176 × 179 | 2,15–2,51 × 2,18–2,55 | 82,72; 3,94 | Bosque e sul, escalas 0,78–0,912 | Mesmo perfil de árvore |
| Bracatinga | 243 × 252 | 3,42–3,65 × 3,54–3,78 | 111,78; 5,54 | Bosque e sul, escalas 0,90–0,96 | Mesmo perfil de árvore |

São **17 árvores** previstas, mais o arbusto e quatro posições de moitas. As âncoras e os perfis são definidos em [SantaRitaRuaNorte.cs](../Projeto%20Jogo%202D/Assets/Scripts/SantaRitaRuaNorte.cs) e [Capitulo1Director.cs](../Projeto%20Jogo%202D/Assets/Scripts/Capitulo1Director.cs). O ralo visível faz parte do piso. A grelha legada de sprite fica desativada nessa montagem.

## 3. Terreno e continuidade das texturas

A grade tem **38 × 22 células de 1 × 1 unidade**, origem **(500, 0)** e limite **(538, 22)**. O piso tem ordem **−20** e as peças visuais não geram colisor. Paredes de perímetro, volumes dos objetos e gatilhos controlam o fluxo.

- **Atlas.** São **256 × 832 px**, com **4 variantes de 64 × 64 px** em cada uma das **13 famílias**, no total de 52 peças. Na ordem de cima para baixo: grama, asfalto, passeio de pedra, terra, borda grama/passeio, meio-fio, terra/asfalto, trinca, poça, faixa, ralo, borda lateral grama/pedra e grama/terra.
- **Planta.** Pista nas linhas locais 8–11. O oeste começa em terra, vira transição na coluna 5 e segue em asfalto. As linhas 7 e 12 marcam o meio-fio e a transição oeste para terra. O passeio norte usa 13–15, com plataforma de pedra em frente ao bar e bordas laterais. A faixa vem na linha 10, em dois tiles de cada cinco. O ralo fica na coluna 29 da linha 13. A grade aplica variantes por hash determinístico e espelha as peças de borda quando necessário.
- **Continuidade entre células.** `_GroundCells` é uma textura linear RGBA32 de 38 × 22 em Point/Clamp. R e G registram coluna e linha do atlas a partir de baixo. B e A codificam espelho X/Y. O shader lê o vizinho mais próximo por eixo e inclui a diagonal nos cantos. Mistura cor, rugosidade, cavidade, altura e gradiente com faixa de **0,195 a 0,345 unidade** conforme a posição no mundo, atingindo cerca de meio a meio na borda. A orientação das normais acompanha o espelho. As amostras continuam discretas em pixels.
- **Detalhes locais.** Trinca e poça compartilham o perfil do asfalto daquela variante. A trinca usa a diferença de cor da arte para localizar o sulco. A poça usa a diferença de rugosidade para limitar a depressão à água. A faixa usa o substrato comum e recebe sua tinta no lugar original após a mistura, evitando a linha refletida na célula vizinha.
- **Critério de aprovação.** Atravesse várias células sob luz rasante e entre dois postes. Observe principalmente retângulos de brilho, mudanças de rugosidade, rejunte que termina na grade e poças com halo quadrado. Ajuste cor e mapas de dados juntos. Teste a junção dos três materiais em movimento e no tamanho normal.

## 4. Materiais, relevo e resposta à luz

A Rua Norte usa o passe `Universal2D` de [LuzVolumetrica.shader](../Projeto%20Jogo%202D/Assets/Resources/Art/RuaNorte/LuzVolumetrica.shader). O shader calcula iluminação em coordenadas de mundo com alturas e oclusores XYZ simplificados. É uma resposta a luz própria para sprites e Tilemap, com especular inspirado em PBR. O URP Lit padrão e a iluminação global dos outros mapas seguem seus próprios caminhos.

| Entrada | Importação | Informação por canal |
| --- | --- | --- |
| `Terreno.png` | Cor sRGB, Point | RGB de albedo e alpha |
| `Superficie.png` | Linear, Point, sem mipmaps | RG = normal XY, B = rugosidade, A = cavidades |
| `Relevo.png` | Linear, Point, sem mipmaps | R = altura, GB = gradientes XY assinados, A = oclusão local |
| `_GroundCells` | Linear, Point, Clamp, gerada ao montar a zona | RG = célula do atlas, BA = espelhos |

- **Amostragem.** Os três atlas devem ter dimensões e arranjo idênticos. O shader quantiza a posição de luz do chão em 64 amostras por unidade. Usa base tangente alinhada ao espelho e modula a iluminação com uma pequena variação de umidade em coordenadas de mundo.
- **Relevo.** O atlas adicional entra com intensidade **1** quando compatível. A altura de referência é **0,31**, com amplitude mundial **0,22 unidade**. O shader soma o gradiente à normal de base. Duas amostras de altura, a 2 e 5 px na direção da luz, reduzem a exposição local, com menor peso junto às emendas. O relevo altera luz e sombra. O Tilemap de circulação permanece plano.
- **Especular.** O shader usa GGX, Fresnel e visibilidade aproximada. Água achata a microtextura e comprime picos de brilho. O reflexo âmbar quebrado sob a porta acompanha a fonte e o piso. Pedra retém rejunte e desgaste. Grama permanece mais áspera e escura.
- **Sprites verticais.** `_Receiver` guarda a base Y, fator de altura e índice de oclusor via `MaterialPropertyBlock`. A normal passa de frontal inclinada para mais voltada ao céu no topo. O sprite ignora a própria caixa no teste de oclusão. Props, piso e Marcus compartilham a lógica visual. As `Light2D` locais legadas da Rua Norte são desativadas para esse caminho. Ao sair da zona, o material e as propriedades de Marcus são restaurados.
- **Autoria.** As fontes Aseprite e o gerador ficam em [Arte/Producao/RuaNorte](../Arte/Producao/RuaNorte). O [gerador de relevo](../Arte/Producao/RuaNorte/gerar-relevo-rua-norte.py) usa Python, NumPy e Pillow. Execute `python gerar-relevo-rua-norte.py <pasta-dos-atlas> <arquivo-de-saida>` e revise os mapas em conjunto antes de substituir o PNG. Preserve GUIDs, pivôs, PPU e importação Point. A procedência das fontes, inclusive os estudos ImageGen retrabalhados e exportados, está em [ASSET_SOURCES.csv](../ASSET_SOURCES.csv).

## 5. Luz, foco e sombra

| Fonte | Offset XY em relação ao pé do objeto | Altura | Alcance | RGB no shader |
| --- | --- | ---: | ---: | --- |
| Poste oeste | +0,66; −0,42 | 3,18 | 7,25 | 2,00 / 1,52 / 0,92 |
| Luminária da entrada | 0; −0,85 | 2,82 | 7,00 | 2,05 / 1,28 / 0,57 |
| Poste leste | +0,66; −0,42 | 3,18 | 6,90 | 1,82 / 1,44 / 0,87 |

- **Ambiente frio.** RGB de `(0.61, 0.70, 0.77)` perto do primeiro plano até `(0.26, 0.36, 0.43)` no bosque. A transição usa Y local de **13,7–20,8**. Os RGB das fontes são ganhos lineares e podem exceder 1.
- **Luz útil.** Cada fonte usa alcance e atenuação quadrática, direção da normal e teste de segmento contra caixas XYZ. O limite é de **32 caixas**, com uma vaga reservada para Marcus. Sua caixa acompanha os pés e tem **0,44 × 0,26 × 1,54 unidade**. No chão, a sombra de contato cai suavemente a partir da pegada e tem redução máxima **0,58**. Esse contato é distinto da sombra projetada.
- **Foco do jogador.** O centro de penumbra acompanha Marcus com deslocamento de **+0,70 Y**. A contribuição ambiente cai gradualmente entre **2,3 e 7 unidades** de distância elíptica, até **32%** no preset normal. A vinheta atua no próprio material, com escurecimento de até **48%** antes da proteção das fontes. Luz local forte reduz esse escurecimento. A interface continua legível.
- **Efeitos reduzidos.** A vinheta e o foco recebem força **0,35** em vez de 1. Vento e os dois véus locais são desligados. A lógica do efeito de chuva e a transição de porta pertencem aos sistemas próprios.

## 6. Atmosfera, passagem e integração

| Camada | Implementação | Valores de referência |
| --- | --- | --- |
| Piso e contato | Tilemap e material do chão | Ordem −20 |
| Respingos e poças animadas | `ChuvaChao` do sistema compartilhado `Chuva` | Ordem −15 |
| Cenário e Marcus | Sprites ordenados pela base e volumes de oclusão | Exemplo de ordem por pé `1000 - round(y × 10)` |
| Traços de chuva | `ChuvaAlta` do sistema compartilhado | Ordem 900. Confira com as ordens dos objetos ao adaptar |
| Véu sobre o bosque | `Nevoeiro.shader` em coordenadas de mundo | Ordem 2030, alpha nominal máximo 0,085, cor fria |
| Véu baixo junto à soleira | Segunda instância de `Nevoeiro.shader` | Ordem 2031, alpha nominal máximo 0,062, tom mais quente perto da porta |

O nevoeiro combina duas escalas de ruído, deslocamento pelo tempo e quantização de **32 px/unidade**. Seu limite se dissolve nas bordas da zona. O véu frontal é baixo no centro, sobe nos ombros da fachada e perde densidade sobre a soleira. A copa recebe deslocamento horizontal por rajada e fase individual, com amplitude base **0,092 × escala**. O tronco, o pivô e a colisão ficam no lugar. `SantaRitaAtmosphere` e `SantaRitaRimLight` suprimem os respectivos efeitos legados nesta zona para evitar dupla aplicação.

**Contrato da porta.** A fachada tem uma porta desenhada com pivô de cena em **(518, 14)** e o gatilho manual fica em **(518, 13,4)**, com **2 × 1,4 unidades**. O ponto de foco vem da maçaneta no sprite **(294, 249)** medido a partir do canto superior esquerdo. `SceneFader` mantém a rua visível até o quadro ficar coberto pela máscara escura com névoa. A troca de zona e câmera acontece sob a máscara. Com efeitos reduzidos, a transição usa fade simples. `BarDoZe3D` abre o salão em primeira pessoa, compartilhado com a sinuca via `SinucaRoom3D`. A exploração usa a mesma mão/maçaneta como continuidade visual, e a saída devolve Marcus à calçada perto de **(518, 12)**. O fluxo e as referências do salão 3D estão no [registro da entrada](ai-plans/2026-11-08_BBCONTTI_bar-ze-entrada-3d.md).

## 7. Como levar o padrão para outra cena

1. **Medir antes de desenhar.** Capture o local atual na câmera real. Marque a rota, as portas, a área de interação e as larguras de passagem. Coloque Marcus ao lado de uma porta, um móvel e uma árvore na mesma folha de escala.
2. **Compor por uso.** Defina primeiro o foco e os setores. Deixe as áreas de entrada, circulação, trabalho e serviço fazerem sentido mesmo com o personagem parado.
3. **Construir material por material.** Faça albedo, dados de superfície e relevo em conjunto. Trabalhe base, transições e detalhes isolados. Confira o encontro de quatro células, os espelhos e o contraste sob a luz mais forte da cena.
4. **Iluminar por hierarquia.** Comece com ambiente legível, acrescente fontes visíveis e confira sombra de contato e reflexo. Só depois aplique queda de luz, profundidade atmosférica e vinheta. Adapte intensidade, alcance e tonalidade às fontes do lugar.
5. **Dar movimento localizado.** Vento atua na copa, chuva segue espaço e superfícies, névoa varia em altura e densidade. Proteja acessos, pista visual e interface. Use uma condição clara para desligar ou reduzir efeitos em jogo.
6. **Validar e registrar.** Compare antes/depois na mesma escala, veja aproximação e afastamento em movimento e confirme acessos, colisão, ordenação e revisitas. Verifique pausa, efeitos reduzidos, resoluções e retorno de zona. Para entrega distribuível, repita no executável e meça custo com a câmera carregada. Anexe captura, valores finais e referências de autoria.

### Ficha de revisão para cada nova área

Preencha a ficha no plano da cena antes de produzir sprites. Compare a entrega com a imagem da Rodovia acima, no tamanho de jogo.

| Verificação | O que registrar |
| --- | --- |
| Propósito e chegada | O que Marcus faz aqui, qual marco ele vê primeiro, por onde entra e como encontra a saída. |
| Composição e escala | Enquadramento de chegada e de aproximação ao foco. Marque frente, área caminhável e fundo. Compare personagem, objetos e arquitetura na escala de Marcus. |
| Fontes e contraste | Indique cada fonte visível e o que ela ilumina. Compare Marcus e um objeto sob luz e em sombra. Em noite chuvosa, procure luz âmbar localizada, sombra fria e reflexo apenas onde há material úmido. |
| Materiais e profundidade | Compare piso, vegetação, fachada, objetos e personagem no mesmo quadro. Confira relevo na luz rasante, contato com o chão, pivô e ordenação em movimento. |
| Acessos e efeitos | Mostre porta, pista e caminho em jogo com chuva e efeitos reduzidos. Confira a volta à área anterior. |
| Evidência | Anexe a referência e capturas em câmera normal. Registre desvios de iluminação com a fonte que os justifica, por exemplo o fluorescente da conveniência. |

**Montagem padrão.** Os exteriores noturnos do Capítulo 1 usam o material criado por `SantaRitaRuaNorte.BuildGround`. O shader `LuzVolumetrica` carrega o contraste da Rodovia no próprio cálculo de ambiente, fachos e resposta ao piso. Vincule o terreno, os sprites e os personagens à mesma luz. Ajuste a posição e o alcance das fontes visíveis, a máscara de umidade e os volumes de sombra conforme a planta. Em interiores 3D, aplique a mesma hierarquia com as fontes locais de `SalaLit` e use a vista da porta para comparar a passagem entre rua e sala.

### Limites observados

- As passadas da Rua Norte foram verificadas no Unity em Play Mode, com os shaders usados em câmera de jogo e sem erros de shader. As capturas documentam o visual. A última passada de continuidade não incluiu teste de build distribuível.
- A auditoria desta referência encontrou duas instâncias de cada moita nova na hierarquia da sessão inspecionada. O código de montagem cria as moitas sem reaproveitar objetos de mesmo nome. Ao portar o padrão, torne a decoração repetível sem duplicação e confira revisitas. As medidas acima registram cada posição uma vez.
- As caixas de sombra são aproximações. Compare a silhueta projetada e a legibilidade, sobretudo junto aos pés, árvores e cantos da fachada. O shader da Rua Norte lista três fontes fixas. O comportamento da lanterna na chuva e em outras zonas vem dos sistemas existentes.

### Fontes e histórico

[Montagem do exterior](ai-plans/2026-11-08_BBCONTTI_rua-norte-bar-ze.md) · [Relevo adicional](ai-plans/2026-11-08_BBCONTTI_relevo-adicional-rua-norte.md) · [Continuidade do piso](ai-plans/2026-11-08_BBCONTTI_continuidade-piso-rua-norte.md) · [Shader de nevoeiro](../Projeto%20Jogo%202D/Assets/Resources/Art/RuaNorte/Nevoeiro.shader) · [Matriz técnica de arte](Matriz_Tecnicas_Arte.md).
