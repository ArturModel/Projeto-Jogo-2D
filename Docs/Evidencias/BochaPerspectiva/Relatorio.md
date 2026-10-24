# Bocha — QA e revisão de produto

23/10/2026. Implementação executada na branch `capitulo_2`, preservando as alterações anteriores. Não há commit/push nesta entrega. [Plano e decisões](../../Bocha_Perspectiva_Execucao.md).

## Entrega

- Clique segurado carrega de 4 a 100% em 1,5 s e retorna em 1,5 s, repetindo. Soltar lança uma vez, mesmo sem arrastar. Espaço oferece a mesma alternativa. Não existe botão LANÇAR nem slider concorrente.
- Cursor define direção e extensão da linha de mira. A força define o alcance; a linha não promete atravessar obstáculos. Ponto, Rafa e Volo usam o mesmo kernel da IA, com rotação, altura, atrito, contatos contínuos e quique.
- Cabeceira e minimapa têm a mesma lateral. A/D corrigidos pela base da câmera. Vista geral agora lança da esquerda para a direita; a consulta não consome bocha. Minimap permanece orientado pela cabeceira, com base embaixo.
- Cancha integrada como sala fechada da Sociedade: divisória, passagem física para salão/copa, corredores, bancos internos, suporte/bancada de Tonão, lambril e arandelas. Fonte da geografia e limites preservados. Não há nova região ou capítulo.
- Atlas Aseprite original, materiais compartilhados com o ambiente explorável, forro na perspectiva, luz quente por vértice e contato junto às guias. A projeção UI não substitui o renderer URP 2D da exploração.
- Mecânica de água removida. A história da inundação permanece. Quatro bochas por lado e partida até seis, recompensa única, prática sem progresso e saves manuais mantidos.

## QA executado

| Frente | Resultado e prova |
|---|---|
| Física/carga | [28 verificações](fisica.txt): raspada que atravessava esfera na versão anterior, transferência de impulso, dissipação, paredes/balim, pilha no canto, rolagem, três modos, voo acima de obstáculo, queda, repouso, clone e passo fixo. |
| Regressão geral | [117 verificações](regras-save.txt), incluindo duas partidas completas pelo kernel: IA vence uma estratégia fraca e perde para busca tática. Também regras, caminhada, narrativa e saves isolados existentes. |
| Editor | Rodada completa de integração passou antes dos últimos ajustes de legibilidade. Confirmou travessia por movimento real, clique, vistas, três modos, sessão inteira e saves. Os ajustes finais foram novamente exercitados no Player QA. Capturas locais em `Editor/`. |
| Player QA atual | [70 verificações](PlayerQA/runtime-qa.txt), mais o status final PASS. Mouse virtual enviado pelo Input System atravessa o EventSystem real; teclado A/D e movimento atravessam o loop real. Não são ações de hardware humano. |
| Carga prolongada | Mouse parado: mínimo 4,47%, máximo 99,57%, quatro inversões; nenhum estoque consumido enquanto segurado; exatamente uma bocha ao soltar. Cancelamento por foco/vista/tipo/release fora, pausa e consulta também passaram. |
| Resoluções | 1024×768, 1920×1080 e 1280×720: carga cancelada e pausa ao redimensionar; interface inteira dentro da tela. |
| Sessão/recompensa | Entrada por interação/diálogo reais, duelo até seis com IA pelo Update e oito lançamentos de Marcus. Tempo acelerado e busca de mira declarados; nenhum placar/bola movido para forçar vitória. Consequência, ficha, revisita e ausência de autosave passaram. |
| Derrota/carregamento | Rotina de derrota exercitada com fixture: cancelar seleção retorna à derrota; slot 1 escolhido mesmo com slot 5 gravado por último; Sinuca compartilha o fluxo; sem save não inventa checkpoint. Não representa duas derrotas humanas completas. |
| Sala e acessos | [Auditoria de cenas](cenas-acessos.txt) sem falhas; sete interações e 851 células acessíveis na Sociedade. Marcus atravessou a porta por movimento real; save antigo foi adaptado para posição conectada à saída. |
| IA | 14,03 / 58,26 / 100,16 ms para 0/4/7 bochas durante build concorrente, nesta máquina. Medição síncrona por decisão, não média de FPS nem promessa de desempenho em outro PC. |

Fixtures iniciam a Sociedade/progressão em RAM e alguns enquadramentos usam reposicionamento explícito. Não comprovam campanha integral desde o Novo Jogo. `BochaPerspectiveVerification.Run()` executa regressões do kernel; `-srBochaQA` executa a suíte no Development. Autoria e harness foram [verificados ausentes](PlayerNormal/assembly.txt) no assembly comum.

## Revisão de produto (PO)

Revisão feita pelo agente sobre critérios e cenários de uso; não equivale à aprovação do autor.

| Critério | Inspeção / correção |
|---|---|
| Entender o gesto | Tutorial, regras, barra e instruções agora descrevem segurar/soltar. Comprimento da mira e força têm papéis explícitos. |
| Evitar ações acidentais | Removidos botão/slider de lançamento. Consultar ou cancelar não consome estoque; release tardio após cancelamento não dispara. |
| Saber quando pode agir | Tipos desabilitados fora da vez; mensagem contextual troca entre carregar, esperar repouso, vez de Tonão e voltar da consulta. |
| Examinar o resultado | Minimap permanente, vista do alvo, vista geral e zoom limitado. Mesmas posições físicas; medida entre superfícies após repouso. |
| Continuidade do lugar | Sala fechada substitui a antiga aparência de espaço aberto. Bancos saíram do salão para a lateral interna, mantendo circulação. Forro/lambril/saibro reforçam o mesmo uso. |
| Composição na câmera | Primeira auditoria detectou bancada estreitando a cabeceira: reduzida/recuada. Captura revelou parede cobrindo a cabeça: ordenação corrigida e verificada no Player. |
| Recuperação | Prática tem saída; duelo oferece consequência ao vencer e seleção explícita ao perder. Saves pessoais não entram na suíte. |

As recomendações imediatas da revisão foram aplicadas: cancelar com segurança, mostrar espera/vez do rival, manter consulta disponível, dar passagem real à sala e corrigir oclusão. A curva de 1,5 s e a dificuldade de Tonão ainda precisam de playtest humano para avaliar preferência e diversão. Não foi declarada “perfeição” ou aprovação artística por contagem de testes.

## Builds e proteção

- [Development Windows x64](PlayerQA/build-qa.txt): Succeeded, zero erros, 295.644.825 bytes.
- [Player comum Windows x64](PlayerNormal/build-windows.txt): Succeeded, zero erros, 224.168.798 bytes. Executável em `Build/Capitulo2/SantaRita.exe`.
- SHA256 agregado de fontes/cenas/dados/texturas/materiais/metas igual nas duas builds e novamente conferido no Editor: `a12dd36e7e72e0420075f9a62dbc192768657c852c739616353efde08aa32310`.
- [Smoke comum](PlayerNormal/smoke.txt): menu/seleção/fixture e sala carregados, composição observada. A injeção externa de E não foi reconhecida pelo Player, inclusive após refocar; por isso não se afirma partida manual completa na build comum. A sessão/inputs foram validados na build Development do mesmo conteúdo.
- [Cinco saves e preferências restaurados](PlayerNormal/saves-preservados.txt), hashes idênticos, perfil real sem junction e zero valores `SRQA_` residuais. Teste comum usou diretório temporário com uma fixture criada do zero.
- Editor devolvido à MainMenu, fora de Play e sem cena suja. Materiais e ferramentas de autoria não foram substituídos por assets copiados do [jogo de referência](https://texasboccecourts.com/pages/play-bocce-ball-online).

## Capturas locais (ignoradas no Git)

- `PlayerNormal/sala-final.jpg`: sala explorável no executável comum, fixture em posição declarada.
- `PlayerQA/sala-entrada.png`, `sala-cabeceira.png`, `sala-bancos.png`, `sala-fundo.png`: divisão salão/sala, circulação e parede atrás de Marcus.
- `PlayerQA/lancamento-Ponto.png`, `lancamento-Rafa.png`, `lancamento-Volo.png`: composição, rolagem/voo e instruções contextuais.
- `PlayerQA/vista-superior.png`, `vista-alvo.png`: orientação e consulta.
- `PlayerQA/resolucao-*.png`, `vitoria-consequencia.png`, `derrota-*-slots.png`: interface e recuperação.

Somente relatórios finais, fontes/arte editáveis e código útil ficam em Changes. Rodadas intermediárias, imagens e logs pesados são locais. Audição humana, acabamento final e equilíbrio/diversão continuam dependentes de avaliação humana.
