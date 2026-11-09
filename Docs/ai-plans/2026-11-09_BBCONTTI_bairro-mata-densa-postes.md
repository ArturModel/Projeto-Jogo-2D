# Mata densa e postes do Bairro

Autor @BBCONTTI. Data 09/11/2026. Unity 6000.6.0f1, URP 2D. Implementado e inspecionado no Editor.

O autor pediu mata contínua nas áreas de grama, copas mais escuras e mais iluminação nos trechos residenciais. A composição anterior tinha clareiras entre os estratos, em especial a oeste e entre a orla e os fundos. Foram consultadas as notas de mapa do C1, a memória e as orientações de pixel art e QA.

- Adicionadas 81 árvores em grupos desencontrados e submata, usando os sprites e perfis existentes. A ambientação das folhas e da casca é mais escura antes da soma da luz direta. A passagem conserva seus corredores.
- Adicionados quatro postes altos nas ruas das casas, dois no lado oeste e dois perto dos nº 22 e 40. Cada poste tem iluminação no chão, sombra e cone âmbar.
- Código em SantaRitaRuaNorte.Bairro.cs e SantaRitaRuaNorte.cs. LuzVolumetrica.shader e Nevoeiro.shader recebem as novas fontes.

Play Mode com transição de teste pelo diretor. Foram confirmados 208 sprites de árvore com perfil de vegetação, quatro fontes extras, nove halos ativos na composição e zero colisores adicionados pelos novos elementos. Shaders sem erros no Editor. Capturas de cinco trechos em Docs/Evidencias/BairroEstradaMata/mata-densa-*.png. As imagens depois-1, depois-2, depois-3 e depois-4 são a referência anterior equivalente. Executável standalone pendente.

Graphify query e refresh indisponíveis por falta da CLI. O validador apontou fontes ausentes no grafo existente.
