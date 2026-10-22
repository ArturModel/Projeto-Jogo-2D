# Ferramentas e skills de Santa Rita

Estado em 21/10/2026. Skills são procedimentos de produção lidos pelo Codex quando a tarefa corresponde ao seu escopo. Não são serviços autônomos nem garantem qualidade artística por estarem instaladas.

Revisão em 22/10/2026: a skill de pixel art passou a exigir composição por função e a matriz de técnicas implementadas. Centro, cemitério separado e interiores receberam nova revisão de mobiliário e circulação; rotinas locais e pistas foram revistas. Estado atual, capturas e testes em `Docs/Evidencias/CidadeViva/Relatorio.md`; técnicas, implementação e limites em `Docs/Matriz_Tecnicas_Arte.md`.

## Configuração e uso realizado

| Recurso | Onde ficou | O que foi aplicado |
|---|---|---|
| Unity MCP, CoplayDev 10.0.0 | Pacote do Unity; chave `unityMCP` em `.codex/config.toml` e configuração pessoal | Servidor HTTP local em `127.0.0.1:8080/mcp`. Instância `Projeto Jogo 2D@a7a0c66d831bf259` conferida antes de alterar cenas. Leitura do Editor, execução de verificações, aplicação visual e capturas. `CodexConfigurator.CheckStatus(false)` retornou `Configured`. |
| Python e uv | `%USERPROFILE%/.santarita` e PATH pessoal | Python 3.12.15 e uv 0.13.4 detectados. O aviso vermelho antigo era um resultado guardado na janela. O cliente correto é Codex. |
| Aseprite | `%USERPROFILE%/.santarita/aseprite-ninja/bin/aseprite.exe` | Compilado do fonte v1.3.16.1, sem compra ou distribuição de binário. Atalho no menu Iniciar, `ASEPRITE_PATH` e exportação batch verificada. O binário local informa `1.x-dev`. |
| `santa-rita-pixel-art` | `.agents/skills/santa-rita-pixel-art/SKILL.md` | Escala/contornos de Marcus, referência ambiental do autor, fontes editáveis, pivôs, filtros e comparação na câmera. NPCs de C1/C2, fachadas, flora, materiais, macros de terreno e caminhada preparados. |
| `santa-rita-narrativa` | `.agents/skills/santa-rita-narrativa/SKILL.md` | Limites do conhecimento dos moradores, apresentação, rotina, fontes das pistas, retorno e consequência. Removidos títulos automáticos/flutuantes e revisados encontros. Novos ramos opcionais implementados no JSON real, não apenas em documento. |
| `santa-rita-audio` | `.agents/skills/santa-rita-audio/SKILL.md` | 16 efeitos Kenney CC0, com licenças e origem. Passos, portas, páginas, coleta, diálogo e colisões de Sinuca/Bocha ligados a eventos. Músicas Suno fornecidas pelo autor preservadas. Passos agora dependem do contato dos pés da animação. |
| `santa-rita-qa` | `.agents/skills/santa-rita-qa/SKILL.md` | Verificação das cenas, contratos visuais, física/regras, saves isolados, caminhos opcionais e executável Development. Resumo e resultados finais em `Docs/Evidencias/CidadeViva`; capturas e progresso ficam locais, ignorados pelo Git. Aprovação artística e audição humana ficam explicitamente separadas dos testes. |
| ImageGen integrado | Ferramenta já disponível no Codex | Criação de atlas com referências comuns. Originais em `Arte/Producao`; acabamento/exportação pelo Aseprite. As imagens geradas não são rotuladas CC0. |
| Context7 | MCP já disponível e instrução em `AGENTS.md` | Consulta de documentação atual para alterações de pacotes/APIs. Não é um gerador de arte ou história. |
| Skill Creator e Verification Before Completion | Skills de apoio já disponíveis | Criação/validação das quatro skills do projeto e exigência de evidência antes de afirmar que algo passou. |

Não foi contratado um serviço pago de áudio, nem instalado um suposto plugin capaz de produzir sozinho todos os efeitos. A base atual usa bibliotecas licenciadas, eventos de jogo e ferramentas de autoria; efeitos específicos podem ser acrescentados com procedência registrada.

## Produção reproduzível

- Fontes: `Arte/Producao`, incluindo `.aseprite` de personagens, flora, fachadas, materiais e animação.
- Scripts: `Tools/Aseprite`. `Export-SantaRita.ps1 -Source <fonte.aseprite> -Output <pasta>` exporta imagem e JSON com tags.
- Caminhada: `marcus-walk.lua` preserva rosto/roupa e prepara oito poses de apoio/passagem/balanço por direção, além do repouso. `MarcusGait` dirige a pose pela distância realmente percorrida; `PeApoiou` liga imagem e som.
- Unity: menu `Santa Rita > Arte` aplica a direção visual, verifica produção/regressão e gera a build QA. A autoria recusa cenas não salvas e Play ativo.
- MCP: com o servidor local iniciado, `Santa Rita > Ferramentas > Conectar Unity MCP local` conecta o projeto. Na janela MCP, usar o cliente **Codex**. Não usar “configurar todos os clientes” para configurar programas sem necessidade.
- Ferramentas de compilação, binários do Aseprite e caches permanecem fora do player e do Git.

## Conteúdo e exploração

A inspiração em Ocarina of Time é estrutural: reconhecer lugares, observar objetos, conhecer moradores, relacionar descobertas e voltar com uma pergunta melhor. Não copiar personagens, diálogos ou a história de Zelda.

Ramos opcionais adicionados:

1. **Rodoviária:** conversar com passageira e motorista, ler o quadro, esclarecer a chamada com o motorista e voltar à passageira. Resolve uma preocupação cotidiana; Marcus não libera a estrada nem recebe uma pista artificial de recompensa.
2. **Costura:** conhecer Celina durante seu trabalho, consultar uma encomenda antiga e comparar homenagem/testemunho. Marcus conta explicitamente o que ouviu; ela anota a procedência na cópia de consulta, preservando o original. O retorno à Nair transmite a distinção entre registro contratado e testemunho.
3. **Oficina:** ler o desenho, ajustar o aparelho e voltar à Mila. Júlia aparece como alguém com humor, interesses e projetos, não apenas como vítima.
4. **Cemitério:** observar caderno/abrigo e perguntar a Alcina sobre o cuidado cotidiano. Ela não passa a ser testemunha de um fato que não viu.

Essas linhas não bloqueiam a investigação principal nem exigem repetir todas as conversas. Flags e notas persistem no snapshot de save existente, sem autosave ou alteração de slots pessoais. O mapa e os fatos da Bíblia continuam prevalecendo. Escola permanece no próximo capítulo.

## Limites de aprovação

As ferramentas estão preparadas para continuar a produção. A meta visual é a imagem de referência do autor; não declarar que todos os interiores atingiram essa qualidade só porque os assets foram trocados. Os relatórios de execução registram o que foi realmente testado e o que ainda depende de avaliação visual/auditiva do autor.
