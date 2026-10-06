using UnityEngine;

public class MainMenuActions : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName = "GameplayScene";

    // Partida nova começa na estrada; "Continuar" volta direto para o
    // orelhão salvo, sem repetir a abertura.
    [SerializeField] private string prologoSceneName = "Prologo";

    public void NovoJogo()
    {
        // Começo limpo: descarta o orelhão salvo e tudo que a partida anterior
        // marcou (flags do prólogo, falas do orelhão já ouvidas, progresso).
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

        CheckpointSystem.RequestSpawnAtCheckpoint();

        string cena = CheckpointSystem.CheckpointScene;
        SceneFader.Instance.LoadScene(
            string.IsNullOrEmpty(cena) ? gameplaySceneName : cena
        );
    }

    public void Opcoes()
    {
        Debug.Log("Opções clicado!");
    }

    public void Sair()
    {
        Debug.Log("Sair clicado!");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
