# Sinuca: pegada fechada vista pelo jogador

Autor @BBCONTTI (git config user.name), execução Codex. 2026-11-06 16:00 UTC-03, America/Sao_Paulo.
Ambiente Windows / C:/Temp/SantaRita / Unity 6000.6.0f1. Status: implementado; aprovação visual in-game pendente.

## Pedido / correção

Autor rejeitou o sprite frontal e depois a tentativa com dedos/palma expostos por baixo. Forneceu foto de empunhadura/alinhamento/tensão e pediu pegada fechada, dorso visível em vista de cima, ângulo para tacada e mão compacta flutuante com pulso curto; não braço completo. Pedido de reduzir aparência de imagem recortada e acertar passagem do taco permanece.

## Contexto

AGENTS, memória e notas de arquitetura já consultadas nesta sessão; atualizações de CURRENT_STATE/arquitetura e query Graphify verificadas novamente. Skills pixel-art/ImageGen/QA já em uso. MarcusWalk.png e as três referências fornecidas pelo autor inspecionados. O último print da empunhadura prevalece sobre a inferência de pegada aberta anterior. Foto usada apenas para anatomia, sem redistribuí-la no Player ou reproduzir logos/textos.

## Executado

- [x] Nova arte original ImageGen: pegada fechada, dorso e nós dos dedos em perspectiva, polegar lateral e pulso curto. Tentativa aberta intermediária rejeitada e não publicada.
- [x] Gerada com eixo vertical de taco como referência e editada com ImageGen para remover somente o taco, mantendo contorno/pose. PNG selecionado: exec-357776e1-e459-4886-88a2-3b5bea51d77b.png, origem no diretório generated_images desta sessão. Arquivo ativo MaoTaco.png substituído, meta/GUID preservado. Arte antiga arquivada em Arte/Producao/Sinuca/Versoes/MaoTaco_dorso_frontal_rejeitada.png.
- [x] Pivô (.47,.45) no eixo de pegada, largura .30 m, contato local (0,0,-.72) do taco. Orientação frontal com roll calculado pela projeção do eixo 3D, em vez de ângulo visual arbitrário.
- [x] Dorso .022 m à frente do eixo na direção da câmera: taco fica atrás da mão fechada, não desenhado atravessando o dorso. Ponto projetado permanece alinhado e posição acompanha recuo do taco.
- [x] Material exclusivo de mão, shader MaoFlutuante: terminação do pulso por dissolução em pixels estável nas UV, sombreado discreto no final. Não há ruído temporal nem edição de pixels em Python. Mão de apoio mantém PNG/pose; só terminação de pulso tratada.
- [x] Mesma camada URP de primeiro plano, mãos ocultas ao caminhar, Zé parado/sem taco e restante das mecânicas preservados.
- [x] Proveniência, memória, arquitetura, QA e evidências atualizados. Sem cenas/saves/commits.

## Arquivos

Assets/Resources/Art/Sinuca/MaoTaco.png (meta preservado), MaoFlutuante.shader + meta; Assets/Scripts/SinucaRoom3D.cs; Tools/QA/verify-sinuca-rig.py; ASSET_SOURCES.csv; arquivo de versão artística; notas/plano/relatório.

## Validação

Assembly-CSharp: 0 erros / 17 avisos CS0649. 22 verificações de assets/projeção PASS. Amostra opaca da nova mão fica 100% enquadrada na projeção numérica de repouso/meia força/força total. Arquivos: ../Evidencias/SinucaMecanicas/compilacao-pegada-fechada.txt e rig-pegada-fechada.txt. git diff --check específico passou.

PNG isolado inspecionado. Isso não aprova a arte ou contato em jogo. Shader não foi validado por render/Player nesta etapa; compilação C# não compila HLSL. Log recente consultado sem erro de shader registrado no trecho, o que não prova compilação de todas as variantes. Unity MCP continuava sem sessão na revisão anterior; CUA anteriormente negado, sem bypass/repetição. Necessária revisão in-game de pegada/dissolução/ponto de contato.

Graphify query/refresh indisponíveis; validate falha por hashes antigos. Índices não falsificados. Fontes PNG são achatadas; não anunciado Aseprite com camadas/quadros inexistentes. Sem aprovação artística presumida.
