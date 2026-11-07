# Zé: direções de animação e falas

Autor: BBCONTTI (git local), execução Codex. Data: 06/11/2026, America/Sao_Paulo.
Ambiente: Windows; Unity 6000.6.0f1 em Play, instância C:/Temp/SantaRita/Projeto Jogo 2D confirmada.
Status: implementado e validado tecnicamente; avaliação artística/audição pelo autor e Player final pendentes.

Solicitação: caminhada/tacada de frente, perfil e costas, trocadas automaticamente pela posição da câmera; restaurar comentários ausentes do Zé.

Diagnóstico: a caminhada escolhia direção apenas ao deslocar, usando eixo da câmera em vez da posição relativa ao ator; a tacada forçava perfil. A guarda de EnfileirarFalaZe bloqueava toda fala desde batidasOuvidas até inspecaoConcluida. Na sessão real, batidasOuvidas=True e inspecaoConcluida=False, sem fala/legenda. Memória INDEX/CURRENT_STATE, Obsidian manual, direção/produção, matriz de arte e skills pixel-art/QA/ImageGen consultados. Graphify query indisponível; código atual prevaleceu.

- [x] Preservados os dezesseis desenhos anteriores pixel a pixel; adicionados oito desenhos ImageGen de tacada frontal/traseira, com Zé/Marcus como referências.
- [x] Atlas 512x768, 24 células 128x128, pés y=113, PPU 64; GUID/meta preservados, Point sem mipmaps. Fontes raster, tags, prompt e exportação reproduzível guardados em Arte/Producao/ZeSinuca e Tools/Aseprite/export-ze-directional.py. Sem fonte Aseprite nova.
- [x] Heading de mundo independente do billboard; direção atualizada por posição relativa da câmera a cada UpdateCamera, sem reiniciar fase de pernas/tacada; histerese evita oscilação na fronteira dos setores. Perfil espelhado à esquerda.
- [x] Falas comuns bloqueadas durante batidas reais/inspeção ativa/fila narrativa, liberadas enquanto a inspeção apenas está pendente. Fala narrativa pendente de Marcus mantém prioridade e locutor.
- [x] Regras, trajetória, contato em .82, saves e cenas preservados.

Arquivos: Assets/Scripts/SinucaRoom3D.Opponent.cs, SinucaRoom3D.cs, SinucaMinigameUI.cs; Npc_Ze_Sinuca.png; exportadores; verify-ze-sprites.py e verify-sinuca-dialogue.py; novos fontes/tags/prompt em Arte/Producao/ZeSinuca; ASSET_SOURCES.csv; documentação e evidência.

Validação: compilação geral .NET 0 erros, 2 avisos de referências auxiliares MSB3277. Console Unity sem erros. PNG: 24 desenhos distintos, pés alinhados e sem recortes. Harness C# de falas: 32 PASS (inclui regressão de inspeção pendente); planner 1580 PASS/300 aproximações; coroutine 10 PASS. Fixture Unity nativa criou sala isolada temporária, reposicionou o ator explicitamente e conferiu 33 trocas de malha (todas as poses, quatro lados e câmera em idle), depois Dispose. Doze capturas reais das três vistas; GIF em Docs/Evidencias/ZeDirecional/comparacao.gif. Fixture não executou partida integral nem comprova contato mão/taco em todas as distâncias. Aprovação artística não inferida.

Graphify refresh executado e falhou por CLI ausente; validate executado separadamente e falhou por quatro parciais atuais não indexados. Índices não foram falsificados. Sem build Player nesta tarefa. Sem commit/push.

Procedência: ferramenta integrada ImageGen, imagens geradas para o projeto, não CC0. Prompt em Arte/Producao/ZeSinuca/Prompt_FrenteCostas.txt; fontes e atlas no mesmo diretório.

Conferência adicional na UI real em Play: legenda 'Zé: Primeiro a partida, Marcus. Depois a gente conversa.' ativa, alpha=1, escrita progressiva com 46 caracteres visíveis e AudioSource de voz presente. Partida em execução do usuário, sem alterar regras/saves. Isso confirma exibição técnica, não audição subjetiva.
