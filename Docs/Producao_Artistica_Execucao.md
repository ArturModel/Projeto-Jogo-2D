# Santa Rita — evolução da produção

Revisão posterior de cidade/interiores, concluída tecnicamente em 22/10/2026: critérios em `Revisao_Cidade_Viva.md`, técnicas e limites em `Matriz_Tecnicas_Arte.md`, capturas/testes/build em `Evidencias/CidadeViva/Relatorio.md`. A revisão substitui a composição anterior da praça, separa o cemitério e mantém os dois retornos a C1. Aprovação artística pelo autor permanece separada.

Pedido autorizado em 21/10/2026. Preservar mapa vigente, saves e progressão de C1/C2; Escola permanece no próximo capítulo.

## Critérios de entrega

- [x] Unity MCP instalado em versão fixa, configurado localmente no Codex e exercitado contra o Editor.
- [x] Aseprite compilado localmente, exportação automatizada verificada e fontes editáveis preservadas.
- [x] Skills de sprites/arte, narrativa/terror/exploração, áudio e QA disponíveis para uso.
- [x] Marcus como referência de escala, perspectiva e acabamento; NPCs e objetos harmonizados por comparação visual.
- [x] Centro com materiais legíveis, transições de terreno, passeios, sinalização e iluminação coerentes; manter geometria do mapa.
- [x] Remover identificação automática de locais e letreiros flutuantes; orientação por arquitetura, placas físicas e descobertas.
- [x] Revisar encontros para rotina própria, apresentação e conhecimento limitado dos moradores, sem alterar revelações canônicas.
- [x] Efeitos de caminhada, interação, exame, portas e minijogos por eventos reais, com volumes e pausas respeitados. Preservar músicas Suno fornecidas.
- [x] Compilação, regressão, capturas e limitações registradas antes de afirmar conclusão.

## Registro

- Inspeção: projeto já usa URP 17.6, Tilemap, importador Aseprite 6.0 e animação 2D. Instalar novamente esses pacotes não resolve a direção de arte.
- Unity MCP escolhido: CoplayDev v10.0.0 (MIT), compatível declarado com Unity 2021.3–6.x; validação local concluída conforme evidências abaixo.
- Aseprite não encontrado nos locais usuais. Autor autorizou avaliar compilação local, sem compra. Visual Studio Community disponível.
- Alterações anteriores de C2 preservadas; esta etapa será registrada aqui com evidências próprias.
- MCP v10.0.0: servidor HTTP em 127.0.0.1:8080; configuração do Codex no projeto e usuário. Chamada nativa do conector leu a cena e executou detecção: Python 3.12.15 e UV 0.13.4 disponíveis. A janela inicial guardava um resultado anterior; feita reavaliação. A seleção de Antigravity não corresponde ao Codex.
- Aseprite compilado do código v1.3.16.1, commit d6bb24d90e962f3c31b6f33018a679de879009fa. Binário local se identifica como `1.x-dev`; não é instalador comercial redistribuído. MSVC/Skia e Windows SDK via pacotes oficiais Microsoft usados apenas em ferramentas locais fora do repo. Compilação terminou com exit 0.
- Exportação batch Lua executada com exit 0: oito NPCs 64×64/48 úteis e treze terrenos 32×32, com arquivos .aseprite. Quatro skills passaram no validador do skill-creator.
- Nova referência ambiental fornecida pelo autor: `Docs/Referencias/Direcao_Visual_Aprovada.jpg`. Uso como direção, sem presumir licença de redistribuição da imagem em builds.
- Foley/interface Kenney CC0: 16 arquivos selecionados, licenças incluídas. Passos por distância real, eventos de porta/exame/página/item e colisões de Sinuca/Bocha integrados. Regressão executada; aprovação de naturalidade/mixagem depende de audição humana.

## Fechamento técnico desta etapa

- Configuração atual: Codex = Configured; Python e uv disponíveis; instância do projeto confirmada. Editor devolvido à MainMenu, sem Play e sem cena suja.
- Novos fontes: oito NPCs C1, oito C2, oito fachadas, oito elementos de flora, materiais/macros e três animações direcionais com oito poses. Referência ambiental e Marcus mantidos como direção; isto não é aprovação artística pelo autor.
- Passos sincronizados pelo evento de apoio, inclusive após frame demorado. Bocha com cabeceiras fechadas/rebote para todas as bolas. Seis ramos opcionais de conversa e consequência implementados.
- 84 contratos de produção, 114 checks de regras/física/caminhada/narrativa/saves; 109 checks no executável C2 e 17 no smoke C1. Development e Player final compilados com zero erros.
- Capturas, GIF e tentativas históricas foram arquivados fora do projeto na limpeza solicitada pelo autor. Resumo e resultados atuais em `Docs/Evidencias/CidadeViva/Relatorio.md`; organização em `Docs/Evidencias/README.md`.
- Avaliação artística final, audição e campanha legítima sem fixture permanecem distintas dos testes automatizados; nenhuma aprovação do autor foi presumida.
