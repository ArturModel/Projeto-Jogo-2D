# Fontes do prólogo jogável

Os cinco PNGs ativos originais gerados estão em `Projeto Jogo 2D/Assets/Resources/PrologoJogavel/`. São fontes raster com transparência preservada; não são arquivos Aseprite nem arte CC0 de terceiros. Procedência por arquivo em `ASSET_SOURCES.csv`.

Referência visual: `Assets/StreamingAssets/Prologo_teste.mp4`, vídeo já presente no projeto. Quadros inspecionados antes da autoria. O vídeo original foi preservado e deixou de conduzir a cena.

- Cabine: interior envelhecido de carro marrom, instrumentos âmbar, para-brisa transparente, volante e objetos removidos para camadas móveis.
- Volante: aro, mãos e mangas isolados; rotação limitada em torno do centro mecânico.
- Paisagem: campos, morros e cidade distante no mesmo clima noturno.
- Objetos: atlas com telefone à esquerda e documentos/foto familiar à direita.
- Asfalto: textura superior repetível, sem faixas pintadas embutidas. Shader aplica profundidade, textura ancorada ao percurso e cores de iluminação. A pintura/reflexos são camadas próprias.
- Chegada: conjunto isolado com placa SANTA RITA/BEM-VINDO, banco de madeira, orelhão verde e calçada. Referências conferidas em `Capitulo1Director.ConstruirZona1_Rodovia`.

O acabamento aprovado pelo usuário nesta conversa refere-se à primeira cabine/interação. A revisão posterior de estrada, chuva e chegada continua sujeita à avaliação dele. Os PNGs finais permitem novas derivações; prompts de direção estão resumidos acima.

Decisão posterior do autor (06/11): remover do prólogo a imagem de placa/banco/orelhão, que parecia uma plataforma na estrada. Somente a estrada e um recuo lateral discreto permanecem visíveis. O carro encosta, desacelera e para antes da transição; os marcos são vistos apenas na próxima cena. A imagem descartada foi arquivada fora de Assets, em Arte/Producao/PrologoJogavel/Descartados. Testes visuais devem priorizar Play, sem nova build por este ajuste.
