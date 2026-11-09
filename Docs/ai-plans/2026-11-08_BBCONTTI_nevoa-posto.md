# Névoa na porta do posto

Autor @BBCONTTI (Git local), com Codex. 08/11/2026. Unity 6000.6.0f1, EntradaDaCidade.

O autor escolheu a transição por textura de névoa. A saída deve mostrar a abertura da porta antes da cobertura. O mapa de zonas do Obsidian e o código indicam que a rua e a conveniência convivem na mesma cena. A consulta ao Graphify ficou indisponível.

- SceneFader e Capitulo1Director fecham a névoa em torno da maçaneta e abrem a cobertura na maçaneta correspondente após a troca.
- Posto3D ajusta o enquadramento inicial pela posição da maçaneta 2D e solta esse ajuste após a névoa abrir. Na saída, a mão alcança a maçaneta e a folha começa a abrir antes do fechamento da névoa.
- EntradaSinuca.shader dá ao posto uma névoa escura com correntes em três escalas. O modo de efeitos reduzidos usa fade simples.
- Os recursos temporários da passagem espacial foram retirados.

Verificado em Play Mode com posicionamento de Marcus por fixture. A entrada anterior ao ajuste de cor teve coincidência de foco medida entre 2D e 3D e abertura de 100 graus. A captura da versão final mostra mão e folha em movimento antes da névoa na saída, troca coberta e retorno ao controle na calçada. Vídeo sem áudio, montado de quadros do Game View. Validação em build standalone pendente. [Captura em movimento](../Evidencias/PostoReforma/nevoa-posto.mp4).

O refresh do Graphify falhou porque o executável está indisponível. A validação do Graphify encontrou fontes ausentes no índice.