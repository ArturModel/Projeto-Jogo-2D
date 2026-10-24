# Saídas e escada da Sociedade — 23/10/2026

## Escopo e composição

Revisão localizada, conservando ligações, geografia, saves e a partida de Bocha. Quatro saídas C1 e oito interiores C2, mais o retorno do depósito à loja. Saídas continuam por E: soleira e capacho vistos de cima substituem a moldura frontal vazada. Aproximação recupera discretamente a cor, sem nome de destino, seta flutuante ou teleporte automático.

- Comércio/oficina: capacho de borracha canelada.
- Casa do avô/costureira: trama doméstica com franjas.
- Bar/Sociedade: fibra em tons de madeira.
- Prédios públicos/igreja/abrigo: trama verde discreta com borda.
- Depósito: superfície de serviço escura.
- Sociedade: copa e convivência no salão; três degraus rasos na abertura de 1,8 m conduzem à cabeceira, dentro da mesma cena. Laterais de contenção dão leitura do desnível; o movimento reduz a cadência no trecho sem interromper a animação ou mudar o ator de escala.
- Complemento solicitado durante a execução: projeção gradual de 15 pixels (46,9 cm / três espelhos), corpo, pés físicos, lanterna e sombra de contato coerentes; o retorno sobe pelos mesmos degraus. A projeção preserva as coordenadas persistidas, sem teleporte de cena nem simulação de gravidade 3D.
- A fachada interna e o forro recuam por transparência suave ao se aproximar da escada ou estar dentro da sala. Longe, voltam a fechar a visão; o restante do salão permanece renderizado. Colisões nunca são desativadas. Carga dentro da sala já começa com o recorte aberto.
- Luz da cancha mais neutra e contida, piso ligeiramente mais frio, sombra contínua junto à contenção e contato sob os pés. As luzes quentes do salão permanecem.
- Revisão posterior: Marcus reduz gradualmente até 90% da escala ao entrar no nível baixo e recupera a escala ao subir; a lanterna acompanha tamanho e altura. Bancos em quatro módulos por lateral, com encosto desenhado para a orientação sul. Parede recebe capeamento com face superior, montantes, reboco e lambril. Nair mantém a rotina na copa, fora da desembocadura da escada.
- Cancha explorável: saibro de 19 × 5,5 unidades, antes 22 × 6. Redução de 13,6% no comprimento e 8,3% na largura. Bancos, NPCs e densidade dos pixels preservados. Recompostos guias, cabeceiras, linha e referências de jogo. Geometria da partida independente preservada.

## Referências

[Cancha do Boqueirão, Passo Fundo](https://www.pmpf.rs.gov.br/esportes/2015/03/23/prefeitura-entrega-a-comunidade-cancha-de-bocha-do-boqueirao-9319/): areia, revestimento em tábuas e bancos de madeira. [Canchas de Rolante](https://commons.wikimedia.org/wiki/Category:Bocce_in_Rolante): referências de ambiente gaúcho. Consultadas como inspiração de uso/matéria; nenhuma foto incorporada ao jogo. O desnível é uma escolha de composição solicitada pelo autor, não uma reprodução documental dessas canchas.

## Plano e evidências

- [x] Preservar alterações anteriores e snapshot da partida em RAM fora do repo.
- [x] Produzir fontes Aseprite e exportações originais, sem reutilizar sprites de Pokémon.
- [x] Aplicar revisão dirigida às saídas e à cancha, sem regenerar o pacote C1.
- [x] Conferir circulação, E/retornos, descida/subida e saves no Editor.
- [x] Exercitar no Player QA e renderizar no Player normal.
- [x] Registrar limitações e conferência final dos saves pessoais.

Resultados: [167 verificações no executável e build normal com zero erros](Evidencias/SaidasEscada/Relatorio.md). Avaliação artística pelo autor permanece separada da conclusão técnica.

Dependência compartilhada indispensável: `PlayerController` consulta a área da escada para reduzir a velocidade a 78%; fora dela retorna a 100%. `Capitulo1Director` apenas troca os quatro visuais e remove três capachos retangulares antigos. Os gatilhos e destinos existentes permanecem.

`SantaRitaActorElevation` só é adicionado aos atores da Sociedade (Marcus e Tonão). Armazena poses projetadas por sprite/pixel e libera os sprites derivados ao destruir o ator. O rim light acompanha a pose projetada. Passos continuam no evento de apoio e usam cimento nos degraus. O teste de ida e volta inclui o desnível visual e a restituição dos pés ao subir.
