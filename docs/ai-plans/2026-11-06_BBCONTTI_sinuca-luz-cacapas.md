# Sinuca — luz, caçapas e mãos

Autor: @BBCONTTI (autoria assistida por Codex). Data: 06/11/2026, America/Sao_Paulo. Ambiente: Unity 6000.6.0f1, Windows, branch melhoria-minigames. Status: implementado; apresentação aprovada pelo autor em conversa, antes da nova reformulação da sala.

Pedido: liberar caçapas de quina, concentrar iluminação realista na mesa, sala sombria sem exagero, otimização e mãos 2D recebendo luz.

Contexto: AGENTS, memória persistente, notas manuais, direção artística e skill santa-rita-qa consultados. Graphify query/refresh indisponíveis por CLI ausente; validate falha por hashes antigos. Código atual confirmado; cena MainMenu sem alterações/salvamento.

- [x] Recorte analítico comum a pano, fundo, borracha e madeira; passagem aberta entre boca física e rede.
- [x] Removidas tampas sólidas elevadas; garganta escura rebaixada.
- [x] Shader local SalaLit para cenário/objetos e luz no shader das mãos, com máscaras/arte preservadas.
- [x] Pendente focal, ambiente fraco e reflexos discretos; uma única luz com sombra, resolução baixa URP.
- [x] Cilindros compartilhados de 6/16 lados, redes e aros combinados; pano com 1790 vértices em vez de aproximadamente 26000.
- [x] Capturas Editor em três câmeras e fixture isolada sem acessar saves/GameState.

Arquivos: SinucaRoom3D.cs, .Table.cs, .Scenery.cs, .Actors.cs; SalaLit.shader e meta; MaoFlutuante.shader; UniversalRP.asset (sombras adicionais habilitadas). Evidências locais em Docs/Evidencias/SinucaAmbiente.

Validação: 46 checks físicos, 71 de caminhada; 624 amostras do espaço ocupado pela bola nas seis passagens, zero interseções com faces sólidas. Shaders sem erros; console Unity sem erros; nenhuma reconstrução geométrica por frame. Redes: 3744 triângulos cada. Sem build Player ou medição de FPS nesta etapa; aprovação artística relatada pelo usuário não equivale a campanha integral.

Próximo pedido: commit/push de checkpoint antes de reformular todo o salão e acrescentar luzes secundárias.
