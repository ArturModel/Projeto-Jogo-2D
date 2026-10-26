# Cenas, zonas e conexões — C1/C2

Atualizado em 26/10/2026. Nome do arquivo mantido para preservar links; o escopo agora inclui C2. [Geografia](../../../Projeto%20Jogo%202D/Assets/Scripts/SantaRitaGeography.cs).

C1: MainMenu → Prologo → EntradaDaCidade, com zonas Rodovia/Posto/Encruzilhada/Bairro/Rua Norte e interiores de conveniência, mercadinho, casa do avô e Bar do Zé gerenciados por Capitulo1Director. CasaDoAvo e BarDoZe também existem como cenas legadas; conferir uso real antes de editar ou excluir.

Progressão: breve abertura → carro próprio marrom (mapa/lanterna) → casa de Adílio/pistas → mercadinho e Bar do Zé → sinuca/ficha → Centro. Relações e revelações seguem Bíblia e direção atual; este índice não substitui condições no código.

C2: CentroSantaRita liga IgrejaMatriz, PrefeituraSantaRita, RodoviariaSantaRita, MercadinhoDeposito, OficinaRadio, CasaCostureira, SociedadeBeiraRio e CemiterioSantaRita; AbrigoCemiterio pertence ao circuito do cemitério. Cemitério não é somente um trecho do Centro.

Retornos C2 → C1 pelo mercadinho (depósito/serviço) e caminho norte do Bar do Zé. Preservar pontos de entrada/saída, flags e compatibilidade de saves anteriores. Escola continua futura.

Sociedade: salão e cancha na mesma cena, acesso por escada rasa sem porta/novo carregamento. O minigame Bocha tem câmera/apresentação própria, minimapa/vista geral e consequência imediata; não avança automaticamente para C3.

Interação E próxima de portas/objetos; transições bloqueadas durante fade. Saídas usam soleira/capacho. Carregar mostra slots; não deslocar para último save automaticamente ao abrir minigame.

## Cenas habilitadas em EditorBuildSettings
- `Assets/Scenes/MainMenu.unity`
- `Assets/Scenes/Prologo.unity`
- `Assets/Scenes/Cidade/EntradaDaCidade.unity`
- `Assets/Scenes/Cidade/CasaDoAvo.unity`
- `Assets/Scenes/Cidade/BarDoZe.unity`
- `Assets/Scenes/GameplayScene.unity`
- `Assets/Scenes/Cidade/CentroSantaRita.unity`
- `Assets/Scenes/Cidade/IgrejaMatriz.unity`
- `Assets/Scenes/Cidade/PrefeituraSantaRita.unity`
- `Assets/Scenes/Cidade/RodoviariaSantaRita.unity`
- `Assets/Scenes/Cidade/MercadinhoDeposito.unity`
- `Assets/Scenes/Cidade/OficinaRadio.unity`
- `Assets/Scenes/Cidade/CasaCostureira.unity`
- `Assets/Scenes/Cidade/SociedadeBeiraRio.unity`
- `Assets/Scenes/Cidade/AbrigoCemiterio.unity`
- `Assets/Scenes/Cidade/CemiterioSantaRita.unity`
