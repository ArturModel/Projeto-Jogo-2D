# Trilhas de C2 e recuperação de binários LFS
Autor: Patrick Dotto (git config). Execução Codex. Data: 01/11/2026. Ambiente Unity 6000.6.0f1 / Windows. Status: concluído tecnicamente; validação artística/audição subjetiva reservadas ao autor.

Pedido: Centro em toda região/interiores; Cemitério e abrigo com faixa própria; Sociedade/Bocha com faixa própria e redução na partida. Corrigir arte ausente/quadrados brancos.

Contexto consultado: memória INDEX/CURRENT_STATE, arquitetura/cenas/catálogo Obsidian, Graphify AudioAmbiente/SantaRitaBarAudio/CriarFonteMusical; skills áudio e QA; código real Capitulo2Audio, Inventory, importer e metadados.

Diagnóstico: recuperação anterior por git show trouxe ponteiros Git LFS em lugar dos binários. 122 arquivos de Assets/Arte foram materializados pelo smudge LFS sem alterar o conteúdo original. Cuia e atlas eram texto, impedindo Resources.Load. O inventário exibia Image sem sprite quando possuía o item.

- [x] Materializar binários LFS, sem substituir trabalho autoral por arte nova.
- [x] Copiar três OGGs intactos, importar streaming/background e registrar origem.
- [x] Capitulo2Audio escolhe trilha por cena; Centro padrão C2, Cemitério/Abrigo e Sociedade exclusivos. Loop true; transições suaves; interior 65% e cutoff 1800 Hz; partida/dialogo 67% adicional. Sem sobrepor texturas musicais sintetizadas antigas.
- [x] Inventário só desenha ícone existente e tenta recarregar ícones anteriormente indisponíveis.
- [x] Compilação C# completa com rsp/referências reais do Unity: exit 0, avisos legados. OGGs 48 kHz com ~88,49/89,68/89,98 segundos; hashes idênticos às fontes.
- [x] Reconectar MCP à instância Projeto Jogo 2D@a7a0c66d831bf259, confirmar Application.dataPath e CuiaBar carregada. Guardar RAM/posição e hashes dos saves fora do repositório.
- [x] Build QA inicial sem erros; imagens reais e cinco ícones confirmados em execução/captura. Teste de áudio identificou fixture sem a flag de vitória do Zé; ajustar preparação isolada antes de repetir.
- [x] Player QA: 137 verificações + encerramento PASS. Onze visitas a cenas, faixas exclusivas, volume, filtro, mesma fonte entre interiores do Centro, passagem real pelo fim de cada OGG, preferência de volume e redução/restauração na Bocha.
- [x] Build normal e QA sem erros e mesmo hash c9cc4100d875e6a7352e91f101ec8d95e4f9fd4018c998e9c333ad35b20dc9a4. QA/autoria ausentes do assembly normal.
- [x] RAM restaurada idêntica ao JSON original; EntradaDaCidade (524.77,12.38), indicador de progresso não salvo false como original. Seis arquivos pessoais de save com hashes inalterados. runInBackground restituído a false no projeto e build final.

Arquivos: Capitulo2Audio.cs, SantaRitaInventory.cs, Resources/Audio/*_loop.ogg e metas, fontes de assets e binários LFS recuperados. Nenhuma cena regenerada ou save escrito. Não presumir audição/aprovação artística.

Evidências: Docs/Evidencias/AudioC2/verificacao.txt. Índices estruturais atualizados separadamente; não equivalem a teste runtime.

Índices: refresh e validate PASS (112 C# incluindo QA de áudio). Não atribuir a esta etapa a homologação runtime de tarefas anteriores. Capturas/logs intermediários permanecem locais.

Limites: Editor confirmou projeto/compilação/CuiaBar carregada, mas sua execução automática não avançou; o ensaio completo foi feito no executável Development. Mixagem verificada por parâmetros/reprodução/loop; não representa audição subjetiva ou aprovação artística do autor. Capturas reais do bar/inventário, Igreja, Cemitério, Sociedade e interface da Bocha inspecionadas localmente.

Encerramento: refresh/validate PASS (112 fontes C#); diff --check exit 0. Trabalho preexistente preservado, sem commit/push. Evidências finais em Docs/Evidencias/AudioC2/PlayerQA/runtime-qa.txt, PlayerNormal/build-windows.txt e saves-preservados.txt. A falha inicial de fixture e a limitação do ensaio Editor estão registradas acima.
