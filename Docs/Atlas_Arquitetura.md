# Arquitetura do atlas do jogador

O atlas é um levantamento em cartas georreferenciadas, não a ilustração histórica nem um sistema de GPS. O desenho é derivado dos sprites e Tilemaps dos cenários existentes, em 32 pixels por unidade, sem reconstruir cenas.

- `SantaRitaAtlas`: lê `Resources/Atlas/Survey.json`. Cada região declara cena/zona, limites locais e retângulo no atlas. `ToAtlas`/`ToLocal` são transformações afins inversas. Os eixos mantêm norte para cima e escala uniforme.
- `SantaRitaAtlasAuthoring`: usa cena carregada em uma fixture Play, extrai limites reais do diretor C2 ou da tabela C1, sprites estáticos e células reais dos Tilemaps. Produz em Library/SantaRitaAtlasStage três camadas por região, PNG Point/RGBA sem mipmaps ou compressão destrutiva. Captura portas, condições e pontos de chegada, inclusive os privados de C1. Não chama builders nem SaveScene.
- `SantaRitaAtlasView`: apresenta as camadas em RawImages, revela mobiliário em 1,8x e miudezas/notas em 3,5x. TMP separado da arte, decluttering de nomes. Sem RenderTexture/câmera extra no Player. Máximo 12x.
- `MapaInterativoUI`: integração M/Esc, scroll, +/-/arraste/setas/WASD, C centralizar, Z cidade. Diário/fichas/legenda e aquisição existente preservados. Painel contextual ocupa espaço somente quando consultado.
- Visitas e origem de entrada ficam em `GameState`: `atlas.visit.<região>` e `atlas.entry.<região>`. São dados RAM normais do snapshot v2; não há autosave ou novo formato obrigatório. Saves antigos usam defaults. Anotações derivam exclusivamente de flags existentes.

## Geografia e costuras

C1 usa coordenadas afastadas em EntradaDaCidade. Cada zona externa recebe uma translação declarada, sem deformar a sua geometria. Centro/Cemitério conservam as coordenadas próprias de produção; interiores têm cartas separadas. Os vínculos unem portas aos spawns reais de outra região nos dados; não há linhas artificiais entre regiões ou até o objetivo. Não presumir que os intervalos entre cenas são terrenos construídos. Sem detalhes inventados para Escola/Clube/Truco/Capela.

O Mercadinho é uma única identidade de estabelecimento com frente e serviço. As duas fachadas reais ficam nas suas cartas de origem, vinculadas pelo interior/depósito. Não remover a fachada de uma cena para forçar coincidência artificial dos sistemas de coordenadas. O marcador no interior conserva a origem exterior de entrada em RAM/snapshot, evitando deslocar quem entrou pelo Centro para a Encruzilhada.

## Reprodução segura do levantamento

1. Preservar trabalho não salvo e slots. Usar MainMenu salva, sem partida do autor em execução.
2. Entrar em Play numa fixture; chamar `SantaRitaAtlasAuthoring.BeginSurvey()` pelo Editor. Isso altera somente a RAM da fixture; não carrega/grava saves pessoais. Aguarda cenas/zonas em execução e captura 19 regiões.
3. Aguardar `Docs/Evidencias/Atlas/bake.txt` terminar com PASS. Sair de Play.
4. Chamar `SantaRitaAtlasAuthoring.Import()` no Editor. A publicação substitui os arquivos por troca atômica, preserva metas e mantém o anterior em Library, evitando arquivos mapeados pelo Windows. Importadores são aplicados somente aos PNGs gerados em Resources/Atlas.
5. Revisar `Survey.json`, `levantamento.csv`, acessos e capturas. Build não depende de caminhos absolutos ou da ferramenta de autoria.

Nome da cidade: campo `city` de `Survey.json`; não renomeia classes, cenas ou pastas do projeto.


## Complemento — 03/11/2026

O título da interface é Mapa da Cidade. O entorno agora recebe paisagem decorativa de campos, morros baixos, árvores e arroio, derivada da arte atual. Não representa novas áreas jogáveis; continua proibido inferir ruas/acessos nas costuras. O levantamento das 19 regiões/38 acessos não foi alterado.

CADERNO abre livro independente, sem carta ao fundo; J é atalho direto, setas/roda e botões viram folhas, Última anotação leva ao fim. Fichas e Legenda interceptam roda sem zoom da carta. A projeção narrativa e avisos estão em MarcusNotebookContent/MarcusNotebookUI; dados originais do diário e formato de save preservados. Fichas são filtradas por posse (1/2/3), sem revelar pendentes ou misturar cupom.
