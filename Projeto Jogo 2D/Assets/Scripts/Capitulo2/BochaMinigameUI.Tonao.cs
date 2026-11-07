public sealed partial class BochaMinigameUI
{
    // A base da legenda encosta na faixa de comandos do rodapé.
    private const float AlturaLegendaBocha = 94f;
    private MinigameDialogue dialogoTonao;
    private int falasTonao;
    private float intervaloTonao;

    private void ConstruirFalaTonao() =>
        dialogoTonao = new MinigameDialogue(raiz.transform, "LegendaTonao", AlturaLegendaBocha);

    private void ComentarJogadaTonao(BochaShot shot)
    {
        if (dialogoMarcus.Ocupada || intervaloTonao > 0 || dialogoTonao.Ocupada) return;
        if (falasTonao++ == 0)
            FalarTonao("Olha onde ela para. Aí a gente mede.");
        else if (shot.tipo == BochaThrowType.Ponto)
            FalarTonao("Devagar. Ainda dá pra chegar.");
        else if (shot.tipo == BochaThrowType.Rafa)
            FalarTonao("Essa eu tiro do caminho.");
        else
            FalarTonao("Por cima ainda tem espaço.");
    }

    private void ReagirAoRepouso()
    {
        var bola = sim.bolas[sim.bolas.Count - 1];
        if (!bola.valida || !sim.bolas[0].valida) return;
        float distancia = BochaMatch.Distancia(bola, sim.bolas[0]);
        if (bola.lado == BochaSide.Marcus && distancia < .35f)
            FalarTonao("Deixa aí. Eu vi onde parou.");
        else if (bola.lado == BochaSide.Tonao && distancia > 1.35f)
            FalarTonao("Sobrou força. Na próxima eu acerto.");
    }

    private void FalarTonao(string fala)
    {
        if (dialogoMarcus.Ocupada || intervaloTonao > 0 || dialogoTonao.Ocupada) return;
        dialogoTonao.Dizer("Tonão", fala, .89f);
        intervaloTonao = 8f;
    }

    private void AtualizarFalaTonao(float dt)
    {
        intervaloTonao -= dt;
        dialogoTonao.Atualizar(dt);
    }

    private void SuspenderFalaTonao() => dialogoTonao?.Suspender();
    private void LimparFalaTonao() => dialogoTonao?.Limpar();
}
