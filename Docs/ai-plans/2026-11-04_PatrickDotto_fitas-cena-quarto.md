# Fitas — cena de quarto e reprovação de elenco

Autor: @PatrickDotto / Patrick Dotto (identificador conforme histórico; git config confirmado no plano anterior, gh indisponível). Data: 04/11/2026, BRT. Ambiente: Windows, Unity não usado nesta revisão.

Status: ambiência/foley preparados e renderer validado; cena completa e elenco NÃO concluídos, geração externa bloqueou Lucas/Marcus. Não há aprovação artística/sonora.

## Pedido

Autor rejeitou ambas as audições como artificiais e incompatíveis com idade; faltaram ambiente de quarto e voz de Marcus ao fundo. Corrigir direção e tratar as fitas como cenas gravadas, não locução isolada.

## Diagnóstico e fontes

Lidos INDEX/CURRENT_STATE, notas manuais Obsidian, skills áudio/narrativa/Audio Lens e direção/produção. Graphify consultado para SantaRitaTapeDeck; catálogo JSON e contrato do tocador conferidos no código. Bíblia: Marcus 16 em 1990, não voz de 30 do presente. Documentação Qwen/Google Gen AI via Context7 e fontes originais Freesound para licenças. A prova anterior era apenas timbre seco: não atendia à encenação solicitada. Rejeição do autor prevalece sobre métricas técnicas/recomendação por funcionalidades.

## Executado

- [x] Marcar audições anteriores como rejeitadas; manter fora de Resources/Player.
- [x] Direção por personagem/posição, conversa cotidiana e volume/distância, sem inventar idade exata de Júlia.
- [x] Obter prévias públicas HQ de ambiente real, cadeira e botão de cassete CC0; registrar fontes e modificações.
- [x] Renderizar base privada de quarto de 14 s, sem atores, identificada como tal.
- [x] Criar renderer offline que exige tomadas reais de todos os papéis para exportar cena completa.
- [x] Nova tentativa vocal limitada: Júlia retornou WAV; Lucas falhou na tentativa e uma recuperação, Marcus falhou uma vez. SSE error/null sem causa informada; sem contorno por réplicas ou insistência.
- [x] Conferir alternativa na UI do AI Studio em aba nova: Voice Design exige chave paga; nenhuma chave criada/vinculada ou contratação. Não houve áudio Google.
- [x] Testar ramo completo com sinais explicitamente sintéticos em fixture temporária e bloqueio de exportação real com elenco faltante. Remover fixture criada após conferir caminho dentro de Temp.
- [ ] Produzir elenco convincente, montar cena completa, ouvir/avaliar e sincronizar a fita definitiva. Permanece pendente, sem substituir com vozes já rejeitadas.

## Arquivos

Criados `Tools/Audio/quarto_scene.json`, `Tools/Audio/tape_scene.py`, este plano e `Docs/Evidencias/MilaInventario/Revisao_Cena_Quarto.md`. Atualizados README de áudio, pesquisa vocal, skill santa-rita-audio, histórico/estado de memória do repositório e derivados Graphify/Obsidian. Áudios/prévias e metadados de geração em diretório privado fora do Git. Nenhum Asset, cena, save ou código de runtime alterado.

## Verificação

Python compila; skill validada. Base PCM16/24 kHz estéreo, 14 s; Audio Lens sem clipping, pico -22,69 dBFS, RMS do downmix -39,02 dBFS. Valores baixos são de stem ambiente para acompanhar diálogo, não master final. Teste do renderer: três cues, limite de pico e manifesto não aprovado; ausência real de Lucas/Marcus bloqueia cena completa. Sem alegar audição subjetiva, timbre adequado ou campanha testada. Não houve build Unity nova porque esta revisão é de autoria externa.

Refresh e validate Graphify executados: PASS, 123 C# cobertos com hashes atuais, caminhos portáveis, canvas/HTML e quatro consultas reais. git diff --check PASS após concluir a exportação dos derivados; zero arquivos staged. Manifesto de cena e licenças validados. Sem commit/push ou compras. [Relatório e fontes](../Evidencias/MilaInventario/Revisao_Cena_Quarto.md).

## Próximo passo indispensável

Obter tomadas juvenis convincentes e consistentes dos três personagens por um serviço efetivamente acessível ou elenco autorizado, antes de tratar o áudio como fita pronta. Ambiência e efeitos não corrigem uma voz adulta/artificial. Não abrir novas contas, criar acessos pagos nem migrar o projeto com base nesta reprovação.
