# Menu ambiente — Santa Rita

Direção: Ismael Weinert (@ismaelweinertb), 06/11/2026.

Referência de composição: vídeo existente `Assets/Videos/telaPrincipalV2.mp4`, 1280×720, 10,01 s. Os quadros em `Referencias/` foram extraídos desse vídeo. Preservados rua molhada, clube Júlia à direita, igreja distante, bandeirinhas, poste e orelhão em primeiro plano.

`Assets/Resources/MenuAmbiente/Rua.png` (1672×941) e `Orelhao.png` (1024×1536, RGBA) são imagens geradas com OpenAI ImageGen para este projeto, a partir do quadro original e da paisagem do prólogo como referência de acabamento. Não são assets CC0. O fundo foi reconstruído sem chuva e sem orelhão embutidos; o telefone foi produzido como camada independente. Os PNGs são as fontes raster finais. Importação Point, sem mipmaps, NPOT preservado e sem compressão destrutiva.

O menu compõe chuva, ondas e reflexos em geometria UI, iluminação do clube e parallax do orelhão. A seleção por mouse/teclado dirige o enquadramento; o orelhão funciona como atalho para a seleção de registros. Os controladores existentes de configurações e saves são reaproveitados. Novo jogo preserva os slots manuais, conforme o comportamento atual de CheckpointSystem.Clear.

O autor declarou a versão funcional e autorizou a conclusão em 06/11/2026. A última suíte automatizada integral não foi repetida após correções nas suas expectativas de save e tempo de fade; não confundir validação do autor com resultado automatizado integral. Detalhes no plano e no relatório de evidências.

## Vista frontal dos registros — revisão posterior

`Assets/Resources/MenuAmbiente/OrelhaoFrente.png` é uma nova imagem gerada com OpenAI ImageGen a partir do orelhão e da rua existentes. A imagem final incorpora o fone e o cabo, por correção expressa do autor. O teclado e o visor recebem texto e áreas interativas no Unity. O monofone separado foi rejeitado como apresentação final; o estudo permanece apenas localmente fora de Assets.

As teclas 1–5 representam os cinco registros; o visor mostra os dados do slot. A aproximação dura 0,22 s. Ao carregar, o aparelho permanece na tela enquanto SceneFader escurece diretamente até a cena salva, sem recuo intermediário. O autor confirmou o funcionamento e pediu commit/push. Plano: Docs/ai-plans/2026-11-06_ismaelweinertb_orelhao-registros.md.
