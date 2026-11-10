# Centro de Santa Rita

## Antes do detalhamento

[Captura de Play Mode da base reconstruída](centro-play.png). A câmera estava junto ao canteiro leste da Matriz, com Marcus por volta de (50,3, 59,1). A captura mostra a situação antes de substituir o adro e o pedregulho por materiais do mesmo atlas.

No Play direto, o Unity confirmou a malha da Rua Norte, o relevo e as poças do Centro. O terreno anterior ficou invisível com seu colisor ativo. As 51 células de (29, 37) até (79, 37) eram pista. Os 14 postes ativos foram verificados quanto à ocupação da pista ou de terra. Os nove destinos de portal continuaram registrados. O campo de campanha `c2_iniciado` permaneceu falso.

## Detalhamento

Três atlas gerados com as linhas-base da Rua Norte intactas e dois materiais novos de Matriz. A validação estática comparou todos os pixels da região reaproveitada. O Unity compilou uma cópia temporária do shader sem mensagens.

No Play novo, o mapa exibiu 197 células de pedregulho, 113 de lajes do adro e 499 de terra. A passagem horizontal continuou asfaltada. Foram contados 14 postes fora da pista, 139 agrupamentos de submata, dez trechos de mureta e oito peças novas de mobiliário nos acessos. O console tinha zero erros e a fachada manteve sua textura independente de Marcus durante a amostragem. `c2_iniciado` permaneceu falso.

- [Matriz detalhada](matriz-detalhada.png), com a junção de lajes, pedregulho e grama.
- [Caminho de terra](caminho-terra.png), com veios de umidade, poças e margem plantada.
- [Passeio e vegetação em Play](detalhamento-play.png), antes do ajuste final de variação dos tufos junto à calçada.

As capturas de setor usaram câmera temporária em RenderTexture. A câmera e Marcus da sessão permaneceram onde estavam e a câmera temporária foi removida. Nenhum slot foi usado. Referência do Pipeline consultada em `connectivity.md`, `commands/scripts.md`, `commands/build-and-compilation.md`, `commands/editor-lifecycle-and-observability.md`, `commands/multiplayer-play-mode.md` e `commands/capture.md`.
