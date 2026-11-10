# Centro — arrimos retos em Play

- [Antes — referência enviada pelo autor](../CentroRuaVertical/Antes-referencia.png)
- [Depois — rua oeste](Rua-Oeste.png)
- [Depois — rua leste](Rua-Leste.png)
- [Depois — rua leste ao norte](Rua-Leste-Norte.png)

O arrimo mantém as bordas paralelas à via. A inclinação da câmera mostra a tampa e a face curta na ponta sul. As copas mais altas cobrem partes do arrimo, e as grelhas e tampas aparecem no passeio. Os recortes foram conferidos a 32 PPU, com bordas verticais verificadas no alfa dos três sprites.

O Unity 6000.6.0f1 importou sete arrimos e 21 detalhes com `_SurfaceMap` e `WorldPBR`. No Play, a instância manteve os sete arrimos, 21 detalhes no material PBR e 1.241 árvores. O console atual registrou zero erros na verificação final. Um teste inicial consultou o nome do prefab em vez do nome da instância na cena. A contagem foi repetida com o nome da instância e passou.

As capturas de 1024 × 576 foram feitas pela câmera de jogo em Play, com reposicionamento temporário de Marcus em (31,93; 70,97), (60,50; 54,70) e (60,50; 68,00). A posição e o alvo da câmera foram restaurados antes de sair do Play. A cena serializada manteve o SHA-256 original. Nenhum slot pessoal foi usado.

A primeira tentativa de Play encontrou erros de compilação de uma alteração paralela na praça. Após a correção concorrente da chamada da lanterna, a recompilação passou com zero erros C#. Esta revisão alterou apenas a arte, o prefab e a documentação.

Avaliação artística do autor e rodada no Player de distribuição pendentes.

