# Capítulo 2 — implementação e verificação

Estado de execução em 21/10/2026. Baseline aprovado: `aebf3dc`, branch `capitulo_2`. O histórico de trabalho está no [plano](Capitulo_02_Execucao.md); os resultados atuais estão no [resumo de evidências](Evidencias/CidadeViva/Relatorio.md).

## 1. Percurso disponível

Após vencer Zé, a passagem da Rua Norte leva ao Centro. Marcus pode investigar a consulta anterior de Adílio na Prefeitura, conversar com Bento na Igreja, comparar a homenagem com os registros de manutenção e com a memória do Cemitério e procurar Tonão na Sociedade Beira-Rio. Visitas antecipadas têm respostas próprias; rádio e costura são linhas opcionais com conclusão.

O confronto de Bocha leva à segunda ficha, ao relato de Tonão e a uma nova pergunta sobre a seleção das palavras do registro. O recado de Adílio expressa uma intenção anterior; não comprova seu paradeiro. O capítulo termina aí, com controle devolvido, revisitas e orelhão manual disponíveis.

## 2. Como iniciar e controles

Executável local: `Build/Capitulo2/SantaRita.exe` (manter a pasta `SantaRita_Data` ao lado). No Unity, parar o Play, abrir `Assets/Scenes/MainMenu.unity` e usar o fluxo normal. Um save v2 pós-Zé conserva seus itens e permite seguir pela Rua Norte. Saves anteriores continuam exigindo a conclusão de C1. Não é necessário regenerar as cenas pelo menu de autoria: as cenas prontas integram a entrega.

- Exploração: controles aprovados de C1; E interage de perto; M abre mapa/diário; Esc abre pausa.
- Exame: imagem ampliada e transcrição; roda do mouse, setas e Page Up/Down percorrem textos longos.
- Bocha: A/D escolhem posição, mouse define direção, segurar/soltar clique ou Espaço controla força. C/botão direito cancelam; Esc pausa. Perda de foco e mudança da resolução pausam e cancelam uma carga sem gastar bola.
- Salvar: somente nos orelhões, escolhendo um dos cinco slots.

## 3. Áreas e mapa

| Cena | Função e relação com o mapa |
|---|---|
| CentroSantaRita | Exterior integrado, praça, vias, jardins, portões e retorno ao Bar |
| IgrejaMatriz | Nave, altar, velas, mural e testemunho parcial de Bento; norte da praça |
| PrefeituraSantaRita | Atendimento e consulta pública separada; a leste do núcleo |
| RodoviariaSantaRita | Espera, guichê, horários e telefone funcional; sudeste do Centro |
| MercadinhoDeposito | Fundos e estoque do mesmo comércio de C1; porta de serviço para a loja original |
| OficinaRadio | Bancada, desenho de Júlia e teste da faixa LOCAL; oeste/sudoeste da praça |
| CasaCostureira | Máquina, tecidos, encomenda e revisão da homenagem; vizinha à oficina |
| AbrigoCemiterio | Velas, caderno e apoio do Cemitério ao norte |
| SociedadeBeiraRio | Salão comunitário, manutenção, registro de visitas e cancha; sudoeste, margem urbana |

Escola fica para o próximo capítulo, conforme decisão expressa do autor. Clube, Bairro Operário, Truco, Capela e outra margem não foram produzidos. Os acessos futuros têm obstáculos visíveis. Coordenadas e conexões estão no plano e em `portas.csv`.

## 4. Narrativa e decisões locais

Júlia permanece consciente durante tentativas repetidas; a demora de socorro é sustentada por fontes diferentes. Padre, manutenção, homenagem e Tonão têm conhecimentos e limites próprios. Não há revelação de Lucas vivo, novo responsável pelo desaparecimento ou explicação total do sobrenatural.

São decisões locais de encenação: plantão de consulta pública, nomes dos moradores de apoio, registro anterior da visita de Adílio, oficina de Mila e encomenda de Celina. Não fixam profissão de Helena nem localização atual de Adílio. Diário identifica fontes e distingue testemunho, documento, interpretação e percepção.

## 5. Bocha e terror

Variante autoral: quatro bochas por lado, balim móvel, seis pontos para vencer. Joga quem está mais distante enquanto tiver estoque. A medida ocorre após os lançamentos e o repouso, com tolerância explícita de 0,01 unidade entre superfícies. Empate vale zero; sem adversárias válidas contam as próprias válidas; sem válidas dos dois lados, zero. Balim fora anula a mão e repete sua disposição e iniciador.

Laterais amortecem; cabeceiras permitem saída. Aproximação, remoção e deslocamento do balim compartilham a mesma simulação usada pela IA. A IA decide antes do lançamento e inclui erro de execução; não corrige a bola depois. A faixa úmida é anunciada entre mãos, permanece fixa durante a mão e desacelera todas as bolas igualmente.

Prática opcional não concede progresso. Vitória e derrota usam o resultado final. Derrota oferece o último orelhão ou menu, sem criar checkpoint. Sino da Igreja, percepção no memorial e resposta abafada da cancha são ocorrências contextualizadas; legendas e diário preservam a informação com efeitos reduzidos.

## 6. Saves e regressão

Mantido o formato v2 de cinco slots. Progresso novo é derivado de flags, pistas e diário do snapshot existente. Salvar continua uma ação manual. Versões futuras, arquivos malformados, posições inválidas e cenas desconhecidas recebem tratamento explícito.

Os testes de save criam armazenamento e preferências isolados e restauram o estado em RAM anterior. Nenhum slot pessoal do autor é usado. A regressão da Sinuca executa invariantes preexistentes, uma tacada física e o resolver real em casos 2–0, 2–1 e marcada + legal simultâneas; esses casos controlados não equivalem a jogar toda a campanha de C1.

## 7. Testes e ambiente

Unity 6000.6.0f1, Windows x64, URP 17.6.0, DX11, AMD Radeon integrada. Autoria, compilação e QA usam uma cópia isolada do projeto, mantendo o Editor do autor aberto.

**Resultados finais:** 75 verificações de regras/física/saves passaram; nove cenas verificadas; carga/serviço/revisitas de C1 e percurso de C2 com vitória, derrota, recompensa, save/recarga e retorno passaram. Build de jogador Windows x64: zero erros, 207.676.240 bytes, gerada em 21/10/2026 às 11:41 UTC.

Os resultados atuais de regras, cenas/acessos, runtime C1/C2, build e inicialização ficam em `Evidencias/CidadeViva`, conforme seu relatório. O histórico desta etapa, incluindo capturas e tentativas anteriores, foi arquivado fora do projeto conforme `Evidencias/README.md`. O percurso automatizado reposiciona o personagem entre pontos e acelera a Bocha de forma explícita: não certifica T01.

Comparação controlada, 1280×720, vSync desativado, limite 60 FPS, 90 frames por região:

| Região | Média | Maior frame observado |
|---|---:|---:|
| C1, Rua Norte | 16,68 ms | 16,98 ms |
| C2, Praça | 16,68 ms | 17,02 ms |

São amostras curtas com limitador e carga do host; não medem todo o capítulo nem estabelecem desempenho em outro PC.

## 8. Arte, áudio e origem

Seis fachadas, dezesseis móveis, oito moradores, oito retratos e seis ilustrações de exame foram gerados para o projeto com ImageGen. Terrenos e sons de C2 são procedurais originais. Vinte e sete PNGs aprovados foram copiados integralmente para importação independente, mantendo atribuições de C1. Inventário em `ASSET_SOURCES.csv`; detalhes em [arte e áudio](Capitulo_02_Arte_Audio.md).

Sem novos packs de terceiros. Imagens geradas não são declaradas CC0. As faixas WAV fornecidas para Bairro/Casa do Avô e Bar do Zé permanecem no fluxo aprovado de C1. Mixagem por audição humana não foi certificada pelos testes automatizados.

## 9. Sistemas alterados e preservação de C1

- Novas cenas estáticas, conteúdo em `Resources/Cap2`, direção do capítulo e implementação separada de Bocha.
- C1: substituída a tela de demonstração por continuidade pós-Zé, acrescentadas revisitas e a ligação de serviço do mesmo mercado.
- Sistemas compartilhados: saves isolados/validação, leitura ampliada rolável, diário/regiões, bloqueio de input durante Bocha, sorting e lanterna para interiores novos, registro de cenas.
- Separados três componentes de C1 em arquivos com nomes correspondentes e substituídas referências locais de script nas três cenas antigas por GUIDs persistentes. Comparação normalizada confirmou preservação dos demais dados de cena. Quinze importadores de terreno habilitam leitura CPU exigida pelo montador existente.
- Corrigido um campo do registro ativo de stripping URP para conservar recursos de pós-processamento na build de jogador. Referência: [documentação oficial de stripping URP](https://docs.unity3d.com/Manual/urp/shader-stripping-features.html) e implementação instalada de `URPShaderStrippingSetting`.
- O gerador legado de C1 deixou de executar automaticamente ao abrir o projeto, evitando reautoria incidental do capítulo aprovado. Continua disponível por comando explícito.

## 10. Limites de prova e pendências

A falha nativa de C1 foi corrigida pelas referências persistentes de scripts; limpar cache sozinho não a resolveu. A leitura de terrenos no player também foi corrigida e retestada. A [matriz T01–T40](Capitulo_02_Matriz_Testes.md) identifica os limites de cada verificação.

Pendências humanas: T01 desde Novo jogo sem QA; campanha iniciada de um save legítimo pós-Zé obtido em jogo; circulação manual por todas as bordas; ritmo, diversão e audição da mixagem. A automação usa fixtures explícitas, reposicionamento e aceleração de QA e não substitui essas avaliações. Escola permanece adiada por decisão do autor. O vídeo legado de menu ainda emite avisos de timestamp/primárias no Windows Media Foundation, e o host usa fallback de tela cheia; a imagem do menu foi observada normalmente.

Permanecem separados: automação passou; captura visual foi inspecionada; percurso legítimo sem QA foi jogado; diversão, ritmo e mixagem foram avaliados por pessoa. Os dois últimos não devem ser inferidos dos testes. Nenhum commit, push ou publicação foi feito.
