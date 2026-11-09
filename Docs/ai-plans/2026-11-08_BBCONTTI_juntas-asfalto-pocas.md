# Juntas do asfalto e poças da Entrada

Registro de @BBCONTTI em 08/11/2026, Unity 6000.6.0f1 e URP 2D.

O autor pediu a retirada da faixa azul, continuidade do relevo entre tiles, pista mais escura e poças mais visíveis. A captura apontava duas causas. O meio-fio tem asfalto em metade do próprio tile, e o sombreamento de altura perdia força perto das juntas.

Em `LuzVolumetrica.shader`, o escurecimento e a máscara da água passaram a acompanhar as amostras que o shader mistura. Na Rodovia, a amostra de altura segue para a célula vizinha, as misturas ocupam uma faixa mais estreita e a microtextura usa coordenadas do mundo. As poças combinam a máscara de rugosidade com brilho ambiente, luz local e reflexo do quadro de Marcus.

A câmera normal em Play mostrou o meio-fio ligado à pista, com relevo visível e poças destacadas. O Unity confirmou o shader sem erros e o Git passou em `diff --check`. A evidência está em `Projeto Jogo 2D/Docs/Evidencias/EntradaDaCidade/entrada-asfalto-e-pocas.png`. A validação do executável segue pendente. A atualização do Graphify continua indisponível e seu validador apontou três fontes ausentes no índice existente.
