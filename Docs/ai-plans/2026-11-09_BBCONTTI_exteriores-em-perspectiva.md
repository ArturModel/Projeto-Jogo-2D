# Exteriores em perspectiva

09/11/2026 às 13:37 · Codex · Projeto local de @BBCONTTI · Unity 6000.6.0f1

Status: implementado e renderizado em Play Mode.

O autor pediu profundidade real pela janela e pela porta dos interiores. Asfalto, calçadas, luz e névoa usam 3D. Árvores e postes mantêm a arte em pixels.

Consultei o código dos cinco ambientes, a direção de arte, a matriz de técnicas, o índice de memória, o mapa de fluxo do Obsidian e as skills de pixel art e QA. O Bar compartilha o cenário com a sinuca. A porta da cancha leva ao salão da Sociedade.

- [x] Trocar as capturas por segmentos de rua ou jardim modelados nas coordenadas locais de cada ambiente.
- [x] Fazer vãos físicos e preservar espessura, molduras, folhas, maçanetas e colisor de interação.
- [x] Reutilizar texturas de piso e sprites de vegetação e poste do cenário 2D. Os sprites giram no eixo vertical para acompanhar a câmera.
- [x] Aplicar luz âmbar, chuva no espaço e névoa com espalhamento nos postes. O vidro desenha reflexos e gotas sobre a vista em perspectiva.
- [x] Conferir Posto, Mercadinho, Casa do Avô, Bar do Zé e Bocha com dez capturas da câmera 3D em Play Mode.

Arquivos de implementação: ExteriorInterior3D, os construtores Posto3D, SinucaRoom3D e BochaRoom3D, shaders JanelaMolhada, NevoaExterior3D e ChuvaEntradaBar. Removida a rotina intermediária VistaExterior2D. As alterações anteriores de iluminação e entrada continuam em SalaLit, LuzInstavel e SceneFader.

[Dez vistas verificadas](../Evidencias/Interiores3D20261109/exteriores-3d.jpg). [Comparativo da luz interior](../Evidencias/Interiores3D20261109/interiores-antes-e-depois.jpg).

As capturas usaram instâncias temporárias e posicionamento de câmera por fixture. Nenhum slot pessoal foi usado. A composição da rua é uma reconstrução local para perspectiva. O executável e a campanha completa ficam pendentes.

Ensaio de porta no Editor:

```text
PASS entrada do Posto pelo Capitulo1Director
PASS saída do Posto e câmera 2D restaurada
```

Graphify não está instalado neste ambiente. O refresh falhou por CLI ausente. O validate sinalizou fontes ausentes do índice anterior. A nota manual do Obsidian foi atualizada.
