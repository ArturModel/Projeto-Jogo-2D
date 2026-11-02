# Marcus: ciclo limpo e transparência da lanterna
Autor: @PatrickDotto / Patrick Dotto, execução Codex. Data 02/11/2026, Unity 6000.6.0f1 Windows. Status: concluído tecnicamente; avaliação artística pelo autor.

Pedido: remover aparência de quatro pernas; caminhar de forma simples/natural; eliminar retângulo ao equipar lanterna e conferir demais poses.

Consultados INDEX/CURRENT_STATE, Obsidian visão/catálogo/cenas, query Graphify MarcusGait/PlayerController, skills pixel-art/QA e direção de produção, folhas e exportadores reais. As transformações cumulativas sobre pernas anteriores deixavam silhuetas extras. A folha equipada tinha somente nove células com corpo e o restante opaco/vazio; origem compatível com geração usando folha anterior. A exportação passa a aguardar/registrar exit code e validar dimensões, conteúdo e transparência, sem presumir sucesso só pela existência do arquivo.

- [x] Reconstruir apenas duas pernas articuladas por quadro, sem reaproveitar a silhueta inferior antiga; preservar rosto/roupa/escala, 16 quadros e apoio por distância.
- [x] Regenerar lanterna a partir da folha completa e sincronizar deslocamento do braço/feixe com o corpo.
- [x] Validar margens transparentes e quantidade plausível de pixels em todas as 51 células de cada folha no loader; folha inválida não vira retângulo no personagem.
- [x] Player QA: 33 verificações + encerramento PASS; todas as 102 células validadas, quatro direções e repouso com/sem lanterna capturados; folha equipada correta e contatos/repouso verificados.
- [x] Builds QA/normal sem erros e mesmo hash 604e1451b69d0018adabf1b60ad349b6a5d2b296951fc36611f93c96d20f1d27; QA/autoria excluídos do normal. Refresh/validate PASS 112 fontes. Seis saves intactos; restauração da RAM/posição no encerramento.

Arquivos: scripts Aseprite marcus-walk/flashlight, fontes Arte/Producao/Marcus, duas folhas StreamingAssets, PlayerController e parciais QA existentes (112 fontes mantidas). Origem: Marcus autoral já existente, acabamento local Aseprite. Sem cenas regeneradas ou alterações de progressão. Qualidade artística depende do autor, não do número de asserts.

Export-Marcus.ps1 executado em sandbox fora do repositório: ambas as folhas reproduzidas com SHA256 idêntico às do jogo, sem tocar os assets durante a build. Capturas reais do Player inspecionadas (laterais, caminhada e repouso equipado), sem retângulo ou pernas extras. Rodada de compilação inicial corrigiu imports faltantes no QA; não foi declarada como teste aprovado.

Relatórios finais: Docs/Evidencias/MarcusLimpo/PlayerQA/runtime-qa.txt e PlayerNormal/build-windows.txt. Ensaio Windowed com fixtures, sem comprovar campanha integral ou aprovação artística pelo autor. Fontes/GIF e 32 capturas ficam locais. Nenhuma cena reautora, commit ou push.

Encerramento: RAM restaurada idêntica ao snapshot, Rua Norte (520.3448,13.4552), indicador de progresso não salvo true preservado. Diff check exit 0.
