# Saídas, escada e profundidade da cancha

Complemento posterior desta revisão: [correção da tecla E nas entradas](../PortasE/Relatorio.md), com reprodução antes da correção e regressão de ida/volta. Os resultados abaixo registram a entrega visual anterior a esse ajuste.

23/10/2026. [Plano e escolhas de composição](../../Saidas_Escada_Execucao.md). Implementação localizada na branch `capitulo_2`, conservando alterações anteriores e saves pessoais. Sem commit/push.

## Resultado implementado

- Quatro interiores C1 e oito C2 com soleira/capacho de chão; depósito também tem retorno de serviço. Cinco materiais contextualizados, sem a moldura roxa vazada. E mostra **Sair** e mantém os destinos. Aproximação não carrega outra cena.
- Sociedade conectada à cancha por três degraus rasos na mesma cena. Corpo, pés físicos, sombra de contato e lanterna seguem uma projeção de 15 pixels de desnível. Marcus reduz até 90% do tamanho e recupera a escala ao subir. O som continua no quadro de apoio, com superfície de cimento nos degraus.
- Parede com reboco, lambril, montantes e capeamento com face superior; fachada/forro somem suavemente perto da escada ou dentro da sala e reaparecem à distância. O recorte não desliga colisões nem esconde o salão.
- Luz da cancha mais neutra/contida e sombra junto à contenção. Saibro contínuo de 19 × 5,5 (antes 22 × 6); cabeceiras/guias/linhas recompostas. Quatro bancos em cada lateral; orientação do encosto sul desenhada separadamente, sem virar pés de sprite de cabeça para baixo. Bancos e atores não sofrem redução global da cena.
- Fontes Aseprite originais e exportação reproduzível em `Tools/Aseprite/interior-thresholds.lua`. Origem por asset no CSV. Sem fotos externas ou fontes de autoria dentro do Player.

## Verificação

| Frente | Evidência e alcance |
|---|---|
| Editor | Travessia filmada nos dois sentidos; redução de escala, reposição dos pés, recorte e bancos dos dois lados passaram. Capturas locais em `EscadaFinal/`. |
| Player QA | [167 verificações passaram](PlayerQA/runtime-qa.txt): E nas saídas C1/C2, serviço → loja, ausência de transição involuntária, descida/subida, recorte com paredes físicas, tamanho da cancha, bancos, carga/vistas/lançamentos da Bocha, sessão inteira com IA e consequência, revisita, saves/slots antigos e ausência de autosave. |
| Circulação | [Auditoria das cenas e acessos](PlayerQA/cenas-acessos.txt): interações e ligações alcançáveis; mapa, limites e cenas preservados. |
| Build QA | [Windows x64/D3D11, zero erros](PlayerQA/build-qa.txt). |
| Player normal | [Windows x64/D3D11, zero erros](PlayerNormal/build-windows.txt), mesmo hash da build QA: `dc0faef251b8127a3db57e1d13f6c9b0aba4b80296b52d2c088d9bcd1280bbf1`. [Assembly](PlayerNormal/assembly.txt) contém os quatro componentes de produção e zero classes de QA/autoria. [Smoke visual](PlayerNormal/smoke.txt): menu/slot/escada/recorte e bancos sul renderizados; captura local `PlayerNormal/escada-final.jpg`. |
| Preservação | [Cinco saves e preferências originais restaurados](PlayerNormal/saves-preservados.txt), hashes idênticos aos anteriores, zero namespace SRQA residual. Partida em RAM devolvida ao mesmo ponto da Sociedade no Editor, snapshot idêntico e indicador de progresso não salvo preservado. Fixtures foram arquivadas fora do perfil pessoal e do repo. |

Fixture: progressão em RAM e reposicionamentos de aproximação declarados; E, movimento e mouse exercitam os handlers reais do Input System. Isto não equivale a uma campanha legítima desde o início. Uma rodada intermediária do Editor falhou sem foco; a filmagem também revelou que duração fixa não garantia chegar ao patamar. A travessia passou a encerrar pela posição alcançada, com timeout. Não foram reduzidos os critérios da partida: o Player passou na oscilação contínua e na sessão completa.

Os efeitos de profundidade são projeção 2D com poses derivadas, escala e iluminação, sem gravidade/arquitetura 3D. A avaliação estética pelo autor continua necessária; nenhum teste técnico foi tratado como aprovação artística.

Limite da inspeção normal: não foi contabilizada a segunda tentativa de renderizar uma fixture dentro da cancha, pois a janela foi minimizada após intervenção no computador. Movimento, foco, partida e consequência foram exercitados no executável QA com as mesmas fontes, cenas e materiais; o smoke normal é prova de compilação/renderização, sem campanha manual completa.

## Capturas locais

- `PlayerQA/saida-*.png`: saídas e porta de serviço.
- `PlayerQA/parede-distante.png`: sala fechada vista do salão.
- `PlayerQA/escada-antes.png`, `escada-cabeceira.png`, `escada-retorno.png`: patamares.
- `PlayerQA/cancha-compacta.png`: corredor e bancos.
- `descida-player.gif`: câmera real do Player, montagem em Aseprite, sem faixa de áudio.

Imagens, GIF, logs e rodadas intermediárias ficam ignorados pelo Git. Somente fontes de produção e resultados finais selecionados entram em Changes.
