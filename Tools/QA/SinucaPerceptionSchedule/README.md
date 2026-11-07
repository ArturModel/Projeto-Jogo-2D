# Agendador de percepções da sinuca

Requer .NET SDK 10. Executar da raiz: `dotnet run --project Tools/QA/SinucaPerceptionSchedule/SinucaPerceptionSchedule.csproj`. O projeto compila diretamente o arquivo real de Assets/Scripts por link; não usa cópia ou reimplementação do agendador.

1.000 sementes, 24 eventos por série, exclusão mútua, intervalos, turnos, gates, bolsa sem repetição, reset e distribuição. Não acessa Unity, slots pessoais nem rede de jogo. Não substitui input/render/audição ou build Player.
