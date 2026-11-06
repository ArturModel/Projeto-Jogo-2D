# Sala da sinuca — reformulação 3D

Autor: @BBCONTTI (autoria assistida por Codex). Data: 06/11/2026, America/Sao_Paulo. Ambiente: Windows, Unity 6000.6.0f1, branch melhoria-minigames. Status: implementado e verificado no Editor; avaliação artística do autor desta revisão pendente.

Pedido: adicionar luzes fracas no restante do salão, melhorar objetos/sprites/encaixes e reformular a sala. Antes de qualquer mudança, commit e push da versão aprovada.

## Checkpoint anterior à reforma

Commit `7cb8779e` registra o estado anterior inteiro (74 arquivos: sinuca, áudio, menu, bocha e documentação). Push para origin/ArturModel/Projeto-Jogo-2D, branch melhoria-minigames, concluído após autorização explícita do usuário. Seis objetos LFS enviados. A reforma começou somente depois do sucesso do push. Esta nova revisão permanece local para avaliação.

## Contexto e diagnóstico

AGENTS.md, memória persistente, cofre manual, direção artística, Matriz_Tecnicas_Arte, procedência e skills santa-rita-pixel-art/santa-rita-qa consultados. Graphify query indisponível por CLI ausente; código atual prevalece. Móveis pintados com perspectiva em planos verticais pareciam chapados ao contornar a mesa. Revestimento de madeira cobria paredes inteiras; faltavam forro, acabamentos e fontes visíveis de luz secundária.

## Executado

- [x] Mantido o plano compartilhado e os pontos de circulação/NPCs, sinuca, saída e geografia da exploração.
- [x] Mesas de café, cadeiras livres, banquetas, balcão, geladeira, engradados, taqueira e TV com volumes 3D leves.
- [x] Recorte superior do sprite original do balcão reutilizado para garrafas/prateleira; personagens e cartazes nativos preservados.
- [x] Copos, cuia e térmica alinhados ao tampo físico, mantendo visibilidade pelas rotinas existentes.
- [x] Reboco acima do lambril, rodapés, guarnições, forro e vigas; acabamentos da porta e maçaneta.
- [x] Três arandelas visíveis, luminária de serviço sobre balcão e reflexo discreto da TV. Mesa continua foco principal.
- [x] Móveis combinados por material, cilindros compartilhados e forro/vigas com referências em cache, ocultados nas vistas acima da sala.

## Arquivos

Novo: `SinucaRoom3D.Furnishings.cs` e meta. Alterados: `SinucaRoom3D.Scenery.cs`, `.Actors.cs`, `.cs`. Sem novos assets externos ou alteração do renderer de exploração. Fixture fora de Assets em Tools/QA; capturas e relatório local em Docs/Evidencias/SinucaSalaR2.

## Validação

Unity conectado à instância correta. Console final sem erros. Shaders SalaLit/MaoFlutuante sem erros. Fixture isolada restaura ativação das salas existentes, sem tocar GameState, slots pessoais ou salvar cenas. Capturas em Mesa3D, LinhaTaco, VistaSuperior e três posições do salão. 624 amostras da projeção da bola nas passagens: zero interseções com faces sólidas. 75 checks numéricos de caminhada e 57 de rig/assets PASS.

Cena de fixture: nove luzes, uma com sombra; 164 mesh renderers habilitados e 61952 triângulos (incluindo bolas/redes/mãos), zero reconstruções geométricas por frame. Isso é inventário de geometria, não medição de FPS ou garantia em hardware mínimo. Combinação por material limita renderers adicionais do mobiliário. Sem Player build nesta revisão; render e layout verificados no Editor. Áudio/regras não alterados.

Graphify refresh/validate tentados: CLI indisponível; índices e hashes antigos não foram apresentados como atuais. Próximos passos: avaliação do autor e validação em Player/perfil de desempenho.
