# Clima mais leve no orelhão

Autor do repositório @BBCONTTI. Execução em 09/11/2026 no projeto C:/Temp/SantaRita, Unity 6000.6.0f1.

O print do autor mostrou a névoa e os riscos de chuva competindo com a nova imagem. Memória, nota arquitetural e scripts de Canvas e orelhão consultados.

- [x] Atenuar apenas a camada animada do close-up. O CanvasGroup de ChuvaNoOrelhao usa alpha 0,22 e a animação mantém a variação interna e a opção de redução de efeitos.
- [x] Exibir a textura com o material padrão. Remover o shader OrelhaoGrade e o meta associados.
- [x] Aplicar os mesmos valores à instância já criada em Play, com o modal fechado e sem reiniciar a partida.
- [x] Compilar com as referências do Unity e conferir ausência de erro C#. A composição final com o modal aberto não foi capturada.

Alterações em Assets/Scripts/PayphoneJogoUI.cs e Assets/Resources/UI/OrelhaoJogo/OrelhaoGrade.shader e meta. Consulta e refresh Graphify indisponíveis por falta do CLI. Validate encontrou fontes ainda ausentes do índice.
