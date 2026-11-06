# Menu integrado — evidências

06/11/2026. Autor: Ismael Weinert (@ismaelweinertb).

A captura Antes.png mostra o vídeo original. Principal.png e Continuar.png registram a composição em Play com as novas imagens e orelhão independente. São capturas de execução, não mockups. Fontes e referência do vídeo em Arte/Producao/MenuAmbiente.

QA inicial: eventos de mouse e teclado no InputSystem, com CheckpointSystem.BeginIsolatedTests e restauração de preferências. Resultado real: 21 PASS / 6 FAIL de 27. Ver resultado-inicial.txt. A suíte continha espera insuficiente para o fade e expectativa incorreta de apagar slots em novo jogo. O roteiro foi ajustado, mas não repetido integralmente antes do autor validar o funcionamento e solicitar encerramento. Player final não testado nesta execução. Nenhuma aprovação automatizada integral é presumida.

Console inspecionado ao encerrar: sem erros ou avisos. Autor declarou ter visto tudo funcional e autorizou a conclusão. Plano: ../../ai-plans/2026-11-06_ismaelweinertb_menu-integrado.md.
