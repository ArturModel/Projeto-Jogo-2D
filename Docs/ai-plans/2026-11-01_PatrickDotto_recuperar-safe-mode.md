# Recuperação do Safe Mode
Autor: Patrick Dotto (git config), execução Codex. Data: 01/11/2026. Ambiente: Unity 6000.6.0f1, Windows.

Solicitação: corrigir os erros CS0246 apresentados ao abrir o projeto.
Diagnóstico: scripts auxiliares da sinuca e outros arquivos novos estavam ausentes da árvore de trabalho, preservados no terceiro pai do stash@{0}. Conferidos documentos atuais, notas manuais e definições reais recuperadas. Não era necessário substituir SinucaMinigameUI por uma versão antiga.

- [x] Recuperados 640 arquivos ausentes do stash, com bytes originais, incluindo fontes/metas/assets/documentação.
- [x] Nenhum arquivo existente sobrescrito; stash mantido e saves não alterados.
- [x] Refresh solicitado no Unity; nova compilação avançou para reload de assemblies, com warnings de APIs obsoletas.
- [x] Refresh Graphify executado: topologia sem mudanças; exportação realizada, validação inicial falhou ao acessar scan-state.json. Validador repetido separadamente.

Confirmado: Editor saiu do Safe Mode e abriu MainMenu. Não se trata de nova homologação da campanha.

Refresh repetido: PASS, 111 fontes C#; diff --check PASS. Editor confirmado em MainMenu fora do Safe Mode.
