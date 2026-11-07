# Zé: sprites novos e circulação na sinuca

Autor: BBCONTTI (git local); execução por Codex. Data: 06/11/2026, America/Sao_Paulo.
Status: implementação e verificações locais concluídas; conferência visual Unity/Player pendente.
Ambiente: Windows, C:/Temp/SantaRita, Unity 6000.6.0f1 instalado, nenhuma instância MCP disponível.

## Solicitação e direção
Zé deve contornar a mesa até a posição da branca e fazer a tacada com pernas, inclinação e taco sincronizados. Correção expressa do autor: redesenhar sprites mais detalhados para Zé, mantendo os outros NPCs como estão. Não reaproveitar/deformar os sprites antigos como animação.

## Contexto e diagnóstico
AGENTS, memória INDEX/CURRENT_STATE, notas manuais de arquitetura/catálogo/cenas, direção de arte e critérios de produção consultados. Skills santa-rita-pixel-art, santa-rita-qa e imagegen aplicadas. Marcus e Zé originais inspecionados como referências de estilo/identidade. Código confirmou Zé estático em SinucaRoom3D e tacada instantânea na coroutine de IA. Graphify query indisponível; índices históricos não foram tratados como fonte atual. Alterações de áudio simultâneas no arquivo de UI, saves e cenas preservados.

## Executado
- [x] Folha INTEIRAMENTE NOVA via ferramenta ImageGen integrada: 16 desenhos, frente/costas/perfil e quatro poses de preparação/inclinação/tacada. Perfil espelhado para esquerda.
- [x] Fonte raster original preservada em Arte/Producao/ZeSinuca/ZeSinuca_Gerado.png; prompt integral em Prompt_ImageGen.txt. PNG editável; não foi produzida fonte Aseprite nesta máquina, onde o executável não foi encontrado.
- [x] Exportador alinha células 128x128, transparência e pés; atlas 512x512, Point, sem mipmap, PPU 64 definido ao criar cada Sprite. Proporção física equivalente à arte anterior.
- [x] Removida a tentativa inicial de deformar os pixels da arte antiga. Não existe OpponentMesh/GetPixel no controlador final.
- [x] Busca de caminho com circulação real, mesa/mobiliário, suavização de segmentos e alinhamento atrás da direção de tacada da IA.
- [x] Três desenhos de pernas por direção, seleção de frente/costas/perfil conforme câmera, fase por distância real, taco próprio no layer da sala.
- [x] Preparação de 1,65 s, contato a 82% com impulso físico único e recuperação; suspensão em pausa, foco e painéis. Fechamento/nova série/prática cancelam apresentação. Ausência de recurso não trava turno.

## Arquivos desta tarefa
SinucaRoom3D.Opponent.cs e meta; integração em SinucaRoom3D.cs e SinucaMinigameUI.cs; Npc_Ze_Sinuca.png/meta; Arte/Producao/ZeSinuca (fonte, atlas, prompt); Tools/Aseprite/export-ze-sinuca.py; Tools/QA/verify-sinuca-opponent.py, verify-sinuca-opponent-timing.py e verify-ze-sprites.py; fixture verify-sinuca-walking.py adaptada ao ator móvel. ASSET_SOURCES.csv e memória/notas atualizados por acréscimo.

## Verificações executadas
- Compilação de todos os fontes Assets/Scripts em projeto temporário .NET com referências/defines do projeto Unity: 0 erros. Avisos de campos serializados/não usados e referências auxiliares do ambiente; não equivale a build Unity/Player.
- Planner real extraído em harness C#: 1580 PASS, 300 aproximações consecutivas, segmentos livres e alinhamento geométrico.
- Coroutine real extraída com relógio controlado: 10 PASS, pausa/foco/modais, preparação sem impulso, um impulso no contato, cancelamento e fallback.
- Circulação/alcance do Marcus: 75 PASS.
- Mecânicas existentes: 45 PASS.
- PNG: 16 células distintas, RGBA, pés alinhados, sem recortes; inspeção visual do atlas exportado executada.
- Scripts PowerShell refresh/validate tentados; refresh falhou porque Graphify CLI não está instalado. Validação Python também falhou: índice anterior não inclui parciais atuais, incluindo Opponent. Não foi fabricado um refresh nem sobrescrito o grafo.

## Limites e próximos passos
Sem acesso MCP ao Editor: não foram capturados vídeo em jogo, alinhamento de mãos/taco na câmera real ou build Player. Conferir animações/escala/contato no Unity antes de declarar acabamento visual aprovado. Fonte ImageGen não é CC0; referências são do projeto. Aprovação artística permanece do autor. Graphify precisa ser instalado/restaurado para atualizar índice. Não foram alterados saves, regras ou cenas.
