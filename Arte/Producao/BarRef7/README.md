> Histórico: esta direção foi substituída pelo pedido de manter a identidade 2D. Não aplicar estes Baked/ à exploração. A exportação inversa foi removida; consulte Arte/Producao/BarIdentidade2D e o plano de 31/10 de identidade 2D. As instruções antigas abaixo são preservadas como histórico, não comandos vigentes.

# Bar Ref7 — fontes e reprodução

39 fontes Aseprite preservam os atlas originais e as exportações finais desta revisão. As fontes PNG usadas no Player estão em Assets/Resources/Art/BarRef7; somente essa arte vai para o executável, não esta pasta de autoria.

- AmbienteAtlas/ItensAtlas: criações originais por ImageGen, com direção de pixel art fornecida pelo autor. Não são assets CC0.
- Inter1993: referência histórica oficial. A montagem não é mostrada como bandeira: o código extrai somente o distintivo e usa as quatro estrelas vigentes em 1994. Direitos do clube preservados.
- Demais fontes: projeções dos mesmos modelos da sala 3D, a 32 PPU, incluindo poses sentadas e bebendo. O corpo 3D mantém rosto e paleta dos personagens do projeto; não substitui a referência de Marcus.

## Reprodução
1. No Editor parado, executar `SinucaRef7Authoring.Prepare()`.
2. Em Play, abrir a prática dentro do Bar do Zé; exportar por `SinucaRef7Authoring.BakeWorldSprites()` ou executar a sessão isolada explícita `SinucaRef7BakeSession.Run()`.
3. Sair de Play e executar `SinucaRef7Authoring.ImportBakedSprites()`; preserva GUIDs e configura os pivôs em `Baked/pivots.json`.
4. Atualizar as fontes Aseprite com CLI local `aseprite --batch origem.png --save-as destino.aseprite`. As projeções guardam a iluminação da sala e usam material Sprite Unlit para evitar iluminação duplicada.

A autoria é explícita e não regenera cenas, não grava saves e não é executada automaticamente ao abrir o Unity. A avaliação artística pertence ao autor; as fontes e QA não constituem aprovação dele.
