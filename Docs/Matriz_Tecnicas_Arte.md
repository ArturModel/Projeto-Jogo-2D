# Técnicas de arte — implementação verificável

A skill descreve o procedimento. Os arquivos e cenas abaixo são a implementação. A adoção de uma técnica deve melhorar legibilidade, composição ou atmosfera na câmera real; nomes de pacotes não substituem esse resultado.

| Termo citado pelo autor | Aplicação real | Limite / verificação |
|---|---|---|
| Tileset e tilemapping | C2 usa Tilemap, materiais 32 px e macros 128 px em células contínuas; C1 compõe seu chão em células de 32 px. Asfalto, terra e saibro receberam acabamento nativo. | Não afirmar que C1 inteiro foi convertido a Tilemap. Inspecionar repetição, cruzamentos e bordas. |
| Decals e layering | Camada `DetalhesTerreno` para transições e desgaste, marcas locais e chuva em camada própria. Caminhos rurais diagonais recebem contorno em pixels e terra contínua, preservando os eixos e acessos. | Não são decals 3D do URP. Separar detalhe de chão da colisão. |
| Iluminação ambiente e dinâmica / URP | URP 2D, materiais Sprite-Lit, Light2D global, pontos quentes, luzes de trabalho e lanterna. | Luzes e materiais exercitados na câmera; pacote sozinho não comprova iluminação. |
| Shadow Casters | Footprints ShadowCaster2D nos quatro edifícios públicos principais, sombras projetadas por Light2D. | Nem todo móvel possui sombra projetada individual. Sombras pequenas usam contato. |
| Ambient Occlusion | Sombras de contato sob móveis/objetos sólidos, em transparência discreta. | É oclusão de contato para sprites 2D, não SSAO 3D nem solução completa por geometria. |
| Rim lighting | Shader `SantaRita/PixelRim`, um pixel de recorte no lado de Marcus voltado para uma Light2D próxima. Direção considera espelhamento; intensidade limitada. | Sem halo permanente e sem normal map inventado. Desativado ao reduzir efeitos. Conferir shader no player. |
| Foreground Obscurance | Profundidade por Y; copas na frente do personagem, com transparência gradual quando escondem Marcus. | Não apaga colisões nem revela pistas fora de alcance. |
| Fog & particles | Névoa baixa limitada a 18 partículas, transparência baixa, desativada em interiores e pela preferência de reduzir efeitos. | Evitar custo e ocultação excessivos. |
| Chuva interativa | Respingos de passo ligados ao apoio; pontos de respingo evitam sólidos e respeitam poças. Chuva responde à iluminação e é abafada dentro. | A checagem é de volumes 2D no chão; não simula cada telha/calha fisicamente. |
| Pós-processamento e filtros | Vignette e ajuste moderado de cor/contraste no URP; pixel art com Point, sem mipmaps/compressão destrutiva. | Sem desfoque de profundidade ou filtros que destruam leitura de pixels. |
| Coerência visual | Referência de Marcus e da imagem ambiental compartilhada; 24 novos móveis cívicos/comerciais e de apoio, fontes Aseprite e pivôs normalizados. Mesas, cadeiras, janelas e armários secundários também entram na revisão. | Conferir escala e função no cenário, não só a beleza do atlas. |

## Composição obrigatória

Cada sala tem função, circulação, lugar de trabalho/espera, estoque ou uso doméstico e sinais comuns da época. Um quadro de avisos público não é uma mesa de exposição; uma cancha precisa de guias/cabeceiras, piso contínuo, bancos, luz e suportes. Pistas antigas ficam em arquivos e objetos pertinentes, sem converter todos os ambientes em homenagens a Júlia.

As escolhas usam a infraestrutura URP 2D já existente. Documentação atual consultada por Context7: Unity Graphics, materiais padrão do Renderer2D e comportamento da iluminação de sprites. Não foi adicionado um pacote 3D de SSAO/desfoque só para marcar uma lista.
