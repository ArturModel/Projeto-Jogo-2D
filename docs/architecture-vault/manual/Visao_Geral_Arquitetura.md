# Arquitetura atual — C1/C2

Atualizado em 26/10/2026 a partir dos fontes atuais. [Inventário completo](Catalogo_Completo_Scripts.md).

| Sistema | Responsabilidades e fontes |
|---|---|
| Sessão/persistência | `GameState`, `CheckpointSystem`, `PayphoneCheckpoint`: RAM, snapshots, slots explícitos, save manual; isolamento de QA. |
| Mundo C1 | `Capitulo1Director.cs`, `InteracaoCapitulo1.cs`, `ChegadaEmSantaRita.cs`, `TransicaoDeZonaTrigger`: zonas e interiores, progressão inicial e retornos do Centro. As classes foram separadas; não procurar tudo no arquivo legado Chegada. |
| Mundo C2 | `Capitulo2Director`, `Capitulo2Interactable`, `Capitulo2Portal`, `Capitulo2Gate`, `SantaRitaGeography`; cenas independentes do Centro/interiores/cemitério e dados em `Resources/Cap2/Investigation.json`. |
| Investigação/UI | `GameMessageUI`, `MapaInterativoUI`, `Interactable`, `InteractionPrompt`, `UIFactory`: diálogo/exame próximo, mapa/zoom/legenda, diário e ações sem spoilers. |
| Movimento/luz | `PlayerController`, `SantaRitaFootsteps`, `SantaRitaAtmosphere`, `SantaRitaActorElevation`: poses, apoio/passo, lanterna F, luz, projeção localizada de desnível. |
| Ambientes | `SantaRitaRoomLayout`, `SantaRitaNpcWork`, `SantaRitaExitThreshold`, `SantaRitaShallowStairs`, `SantaRitaRoomCutaway`: trabalho, circulação, saídas, escada e paredes da Sociedade. |
| Sinuca | `SinucaMinigameUI`: jogo do Bar do Zé e retorno ao seletor explícito de saves após derrota. |
| Bocha | `BochaMinigameUI`, `BochaCourtView`, `BochaShot`, `BochaSimulation`, `BochaRules`, `BochaAI`: apresentação separada da simulação, força por clique/soltura, Ponto/Rafa/Volo, vistas/minimapa, IA, pontuação, colisões/rebotes e consequência. |
| Áudio | `AudioSettingsController`, `AudioAmbiente`, `SantaRitaFootsteps`, `SantaRitaBarAudio`, `Capitulo2Audio` e sistemas legados: música fornecida pelo autor, contexto/atenuação e efeitos por evento. Confirmar responsáveis reais antes de adicionar nova reprodução. |
| Autoria/QA | `Assets/Editor/` e verificadores `#if UNITY_EDITOR || DEVELOPMENT_BUILD`; fontes Aseprite em Arte/ e Tools/. Código de autoria e fixtures não entram no Player comum. |

## Contratos

- Nenhum autosave ao coletar pista, trocar de cena ou vencer minigame. Carga exige escolha de slot; não recarregar o último automaticamente.
- Portal manual aceita E sem janela global de espera; fade em andamento não pode ser sobrescrito. Passagens automáticas mantêm antirretorno e reset de relógio/sessão.
- C1/C2 compartilham estado e geografia; o cemitério é cena própria. Preservar compatibilidade de saves antigos e os dois caminhos de retorno.
- A cancha pertence à mesma cena do salão; o minigame usa apresentação própria coerente com a sala. Dimensão do desenho em mundo não precisa ser dimensão física da simulação.
- Arquitetura, luz e mobiliário orientam exploração; evitar títulos automáticos e a cidade organizada para receber Marcus.

Grafo/cofre são derivados. Extração AST de C# não verifica narrativa, cenas YAML, importação de sprites, luz renderizada ou qualidade artística; consultar fontes e relatórios próprios.
