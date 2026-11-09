# Saída animada do Bar do Zé

09/11/2026 · Codex · Projeto local de @BBCONTTI · Unity 6000.6.0f1

O autor pediu a volta da animação da saída. O fluxo local girava a porta sem animar a mão. A cena separada BarDoZe carregava EntradaDaCidade antes da cinemática.

- [x] Reaproveitar o sprite de mão e seguir a maçaneta interna até soltar a porta.
- [x] Concluir a abertura antes do início da névoa. A câmera avança pela soleira enquanto o abafamento da chuva diminui.
- [x] Usar a mesma máscara na rota local e na rota da cena separada, com a posição de saída junto à porta.
- [x] Bloquear o controle 2D durante a exploração 3D do bar e durante a revelação da Rua Norte.

Arquivos: SinucaRoom3D.Bar, Capitulo1Director, SceneFader, SceneTransition PlayerController e InteractionPrompt.

[Sequência capturada em Play Mode](../Evidencias/Interiores3D20261109/saida-bar-do-ze.gif). A fixture posicionou a câmera e chamou o fluxo real da porta do diretor. Nenhum slot pessoal foi usado. Executável pendente.

```text
PASS EntradaDaCidade: mão, contato, abertura, soleira, névoa, rua e spawn.
PASS BarDoZe → EntradaDaCidade: mão, contato, abertura, soleira, névoa, rua e spawn.
```

Graphify não está instalado. O refresh falhou por ausência da CLI e o validate apontou fontes ausentes do índice anterior.
