# Capítulo 2 — matriz de verificação

Esta matriz separa automação, inspeção de código/imagens e playtest humano. `Parcial` não equivale a teste integral aprovado. Resultados atuais ficam em `Evidencias/CidadeViva`; capturas e tentativas antigas foram arquivadas fora do projeto conforme `Evidencias/README.md`.

| Teste | Evidência / limite de cobertura |
|---|---|
| T01 | **Pendente:** novo jogo atravessando C1 e C2 sem QA/debug. Não foi realizado por pessoa nesta execução. |
| T02 | Save pré-Zé em armazenamento isolado mantém bloqueio do Centro; porta de serviço também não permite contorno. |
| T03 | **Parcial:** formato v2 e snapshot pós-Zé em fixture preservam ficha/pistas e atravessam a continuação; não havia save legítimo pós-Zé do autor disponível. |
| T04 | **Parcial:** resolver real da Sinuca e limiar aprovado foram executados; a campanha completa com vitória legítima na mesma sessão permanece ligada a T01. |
| T05 | Limiar real termina seu diálogo e abre Centro; a tela antiga de demonstração não é mais chamada. |
| T06 | Portas de ida/volta das nove cenas executadas; serviço retorna à posição da loja original. Spawns e acessos também conferidos no Editor. |
| T07 | **Parcial:** câmeras observadas em capturas de todas as cenas; navegação amostrada por colisão. Caminhar manualmente por todos os corredores/bordas ainda é playtest. |
| T08 | Depósito entra e retorna ao mesmo Mercadinho de C1; sem segunda frente ou segunda loja. |
| T09 | Planta comparada à referência: Cemitério norte, Sociedade sudoeste na margem urbana; tabela de coordenadas no plano. |
| T10 | Gate pré-Zé testado; destinos futuros sem portas utilizáveis; perímetro com cerca/vegetação, ponte em manutenção. |
| T11 | Tonão visitado cedo não abre duelo; Cemitério antes da Prefeitura tem leitura válida. |
| T12 | Comparação do memorial reconcilia fontes fora de ordem; objetivo crítico se completa antes das linhas opcionais. |
| T13 | Repetição pós-Bocha não duplica ficha nem diário. Flags e notas possuem identificadores estáveis. |
| T14 | **Parcial:** snapshot/recarga e posição foram exercitados, inclusive recarga real pós-vitória; não foi carregado cada estágio intermediário da investigação. |
| T15 | Requisitos de Tonão satisfeitos antes de rádio/costura; opcionais não condicionam o boss. |
| T16 | Revisão dos textos novos: fontes e limites identificados, datas alinhadas e nenhuma identidade atual/sobrevivência de Lucas revelada. |
| T17 | Oito interiores produzidos, percorridos por portas e capturados com suas funções; Escola excluída pela decisão expressa do autor. |
| T18 | Regras de estoque, repouso, turnos e lançamentos executadas; cancelamento por foco e pausa exercitado. Uso humano contínuo do mouse/teclado ainda ligado a T01. |
| T19 | Simulação executa aproximação e colisões/remoção, inclusive deslocamento e saída do balim; IA usa o mesmo kernel. |
| T20 | Medição em repouso, tolerância/empate, um e múltiplos pontos: verificações puras no Unity. |
| T21 | Estoque esgotado, lado sem válidas e ambos sem válidas: verificações puras no Unity. |
| T22 | Saída, balim fora, anulação, repetição e proteção contra dupla contagem: verificações de regras/física. |
| T23 | Vencedor somente após resolução; partidas completas reais de vitória e derrota executadas pela UI de QA. |
| T24 | Piso seco/úmido comparado e clone da IA preserva faixa; faixa fixa durante a mão e anunciada na UI. |
| T25 | **Parcial:** pausa de mira/carga e perda de foco testadas; não foi exercitada uma pausa humana em cada fase/efeito possível. |
| T26 | IA venceu estratégia fraca e perdeu para estratégia forte nas regras; vitória e derrota também exercitadas na UI. Sem terceira fase forçada. |
| T27 | Redimensionamento real pausa sem consumir estoque; callback de perda de foco cancela carga sem consumir estoque. Não substitui avaliação humana em monitores diferentes. |
| T28 | Carregar snapshot anterior descarta vitória sem orelhão; nenhum save automático de boss. |
| T29 | Orelhão real grava segunda ficha; recarga pelo fluxo público restaura consequência e posição. |
| T30 | Derrota real na UI oferece último orelhão; escopo isolado vazio oferece só menu, sem criar checkpoint. |
| T31 | Arquivo inválido, versão futura e cena desconhecida recusados; leitura preserva arquivo e outro slot válido. |
| T32 | Snapshot v2 conserva estado C1 e não cria vitória C2; incompatibilidade futura não migra silenciosamente. |
| T33 | Armazenamento e preferências próprios; namespace temporário, estado anterior em RAM restaurado. |
| T34 | **Parcial:** Sinuca física/resolver/invariantes, cenas antigas, revisitas e inventário foram conferidos. Controles completos e audição das músicas de C1 ainda pedem playtest humano. |
| T35 | Compilação e referência de cenas verificadas; falhas nativas/CPU de C1 encontradas, corrigidas e retestadas. Percurso final sem exceção gerenciada. |
| T36 | Uma chuva no retorno; ambiências com fade e impactos só por contato na implementação. **Mixagem e continuidade por audição humana pendentes.** |
| T37 | Capturas de texto/retratos, transcrições roláveis, mapa/legenda e resoluções diferentes inspecionadas. |
| T38 | Build Development e build de jogador geradas sem erros; percurso completo em Development passou. Startup do jogador registrado separadamente. |
| T39 | Comparação controlada em `Final/Desempenho`: C1 e C2 com média de 16,68 ms, 1280×720, vSync=0, limite 60 FPS, 90 frames/cena. Não é perfil completo nem garantia de FPS em outro PC. |
| T40 | Revisão visual identificou e corrigiu recortes de sprites, sobreposição de calçada/asfalto, leitura cortada, banners técnicos, textura periódica e espaços vazios. **Ritmo e diversão continuam pendentes de pessoa.** |

## Problemas encontrados e regressões permanentes

1. `level2 / Position out of bounds`: scripts secundários em um único arquivo eram serializados localmente na cena. Componentes separados em arquivos com nome correspondente; referências persistentes. Verificação impede novo MonoScript local antes do build.
2. `Texture ... is not readable`: C1 compõe pisos lendo pixels; quinze importadores dos terrenos habilitam CPU. A verificação deriva do conjunto usado pelo montador, sem alterar PNGs ou escala.
3. E repetido reiniciava fechamento do exame: estado de fechamento impede reentrada; textos longos agora têm rolagem.
4. Móveis/árvores carregavam primeiro fragmento do atlas: cópias integrais e importação própria de C2, comparadas por hash.
5. Estampas de passeio apagavam a rua: rasterização por célula, preservação do asfalto nos encontros com terra e seis amostras de regressão dos eixos.

6. A build de jogador removia recursos de pós-processamento: desativada a remoção no registro ativo de `URPShaderStrippingSetting`; o registro legado já estava desativado, mas não era a configuração efetiva. Reteste da abertura registrado à parte.

Não remover a distinção entre estes resultados e T01 ao apresentar a entrega.
