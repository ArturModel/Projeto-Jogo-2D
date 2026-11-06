using UnityEngine;

public class MainMenuActions : MonoBehaviour
{
#pragma warning disable CS0414
    [SerializeField] private string gameplaySceneName = "GameplayScene";
#pragma warning restore CS0414

    // Partida nova começa na estrada; "Continuar" volta direto para o
    // orelhão salvo, sem repetir a abertura.
    [SerializeField] private string prologoSceneName = "Prologo";

    private MenuAmbiente ambiente;

    private void Awake()
    {
        if (gameObject.scene.name == "MainMenu")
            ambiente = gameObject.GetComponent<MenuAmbiente>() ?? gameObject.AddComponent<MenuAmbiente>();
    }

    public void NovoJogo()
    {
        if (ambiente != null && ambiente.PrepararSaida(IniciarViagem, true)) return;
        IniciarViagem();
    }

    private void IniciarViagem()
    {
        // Começo limpo em RAM e spawn pendente. Clear preserva os slots manuais
        // existentes; eles continuam disponíveis na seleção explícita de carga.
        CheckpointSystem.Clear();
        GameState.Limpar();
        SceneFader.Instance.LoadScene(prologoSceneName);
    }

    public void Continuar()
    {
        if (!CheckpointSystem.HasCheckpoint)
        {
            Debug.Log("Nenhum orelhão registrado ainda.");
            return;
        }

        // Abre a seleção dos 5 Slots de Orelhão para o jogador escolher qual save carregar
        PayphoneCheckpoint.AbrirModalCarregarDoMenu();
    }

    public void Opcoes()
    {
        Debug.Log("Opções clicado!");
    }

    public void Sair()
    {
        if (ambiente != null && ambiente.PrepararSaida(Encerrar, false)) return;
        Encerrar();
    }

    private void Encerrar()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
