# Atlas — evidências e limites

## Implementação

Levantamento de 19 regiões, camadas derivadas dos sprites/Tilemaps reais, 38 acessos resolvidos por portas/spawns. Nenhuma cena reconstruída ou salva. A arte histórica não é carregada na experiência normal. O atlas conserva orientações e escalas locais; zonas de cenas diferentes possuem transformações explícitas, sem presumir coordenadas contínuas.

Zoom 1x/1,8x/3,5x revela progressivamente construções, acessos/mobiliário e detalhes/anotações. Máximo 12x. Rótulos são TMP separados das imagens e passam por controle de sobreposição. Sem rota GPS/linhas falsas. Plantas são cenas efetivamente construídas, disponíveis após a visita; marcador é projetado pela mesma transformação. Frente/serviço do mercado mantêm identidade única e origem exterior de entrada.

Mapa ocupa a área principal, painel contextual abre por seleção/abas. M/Esc, scroll, +/-/mouse/setas/WASD, C, Z e botões preservados. Diário/fichas acessíveis antes do mapa; desenho cartográfico fica oculto até aquisição. Nome configurável em Resources/Atlas/Survey.json.

## Evidências

- `levantamento.csv`: limites reais e transformação de todas as cartas.
- `bake.txt`: geração explícita concluída.
- `EditorQA/`: fixture e capturas no Editor.
- `PlayerQA/`: o mesmo contrato no executável Windows, com capturas das cenas e atlas.
- `BuildQA/` e `PlayerNormal/`: relatórios de build e fingerprint dos assets.

Os cenários nas capturas foram acessados por fixture (carregamento/reposicionamento), não por campanha legítima integral.

## Ocorrências corrigidas

1. Campos privados de transição C1 não são propriedades serializadas: leitura por reflexão dos campos reais.
2. Limites atuais do Centro são -20,-8 até 96,85. Cartas C1 deslocadas para não serem encobertas por esse retângulo; nenhuma posição da cena foi alterada.
3. Windows impediu sobrescrita de PNG carregado. Autoria agora gera em Library e publica fora do Play por substituição atômica, preservando metas.
4. Editor estava pausado por erro anterior; captura refeita com frames ativos para incluir Start/dressing dos interiores.
5. Marcador deve ser último irmão tanto ao abrir quanto ao alternar plantas. Corrigido e adicionado teste.
6. Portão da Rua Norte usa InteracaoCapitulo1, não um portal C2: incorporado ao grafo real de acessos.
7. Anotações consultam a identidade do estabelecimento também nos acessos com alias; incluída regressão e captura da Prefeitura.
8. Cliques na barra de controles não iniciam arraste do mapa.

## Limites

Aprovação artística pertence ao autor. O levantamento reproduz a composição existente; não redesenha cidades, mobiliário ou iluminação. Não introduz terrenos/interiores inventados nos intervalos entre cartas ou em regiões futuras. C1 e C2 são dados em sistemas locais distintos; o atlas não afirma que as bordas de cartas são uma simulação contínua. O mercado conserva frente e serviço nas cartas respectivas, identificados como o mesmo comércio.

As capturas e testes técnicos não equivalem à campanha inteira jogada por uma pessoa, nem à revisão subjetiva de todas as escalas/resoluções. Cenas legadas CasaDoAvo/BarDoZe mantêm a associação do marcador ao edifício; as plantas levantadas são as zonas correntes de EntradaDaCidade.

## Resultado final

Editor e Player Windows: 237 verificações + encerramento PASS em cada rodada final. Incluem 19 transformações/57 camadas, 38 acessos, aquisição, scroll/Esc reais, zoom/abas, posição externa/interna, plantas sobrepostas corretamente, anotações por alias e save manual isolado. Cinco snapshots pessoais C1/C2 foram desserializados/importados em isolamento; seis arquivos pessoais mantiveram SHA256 idêntico ao início.

Build QA e normal: Succeeded, zero erros, mesmo hash de fontes/assets `007b692beed5588d8c51e0e417da6a43d3835a9b98934daf9dfefd1fafd9ea70`. Atlas incluído; runner QA e assembly Editor ausentes no Player normal. Build normal em `Build/Capitulo2/SantaRita.exe`. Console final: nenhum erro de código; a consulta de avisos/erros retornou somente a mensagem do próprio Unity `1 URP assets included in build`.

Refresh e validate Graphify: PASS, 116 C#. `git diff --check`: PASS. Nenhuma cena, configuração do projeto ou fonte de arte anterior modificada. Alterações permanecem não staged em `codex/atlas-cartografico`, sem commit, push, merge ou publicação.

O Windows registrou fallback de ExclusiveFullscreen para FullscreenWindow durante a fixture de resolução; as capturas finais do Player têm 1280×720. A validação no Editor utilizou também seu enquadramento de Game View; isso não substitui ensaio subjetivo em todas as resoluções.


## Smoke da interface no Player normal

Executável sem flags de QA aberto via computer-use: menu → seleção explícita do slot 1 pessoal C2 → mundo → item mapa → atlas → Focar Você → arraste → Fichas. Capturas nativas 1536×864 em PlayerNormal e roteiro em smoke-interface.txt. Dados narrativos/ficha adquirida mantidos. Processo de teste encerrado; seis arquivos de save novamente conferidos, sem mudança.

As teclas rápidas injetadas pelo helper não geraram ação observável, e houve troca de foco detectada; não se presume validação de teclado físico no normal a partir desse smoke. M/Esc e navegação foram exercitados pelo Input System no QA. A skill usada foi .codex/plugins/cache/openai-bundled/computer-use/26.1027.30824/skills/computer-use/SKILL.md, além das skills de produção do projeto.
