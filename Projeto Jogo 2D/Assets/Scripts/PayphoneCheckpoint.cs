using UnityEngine;
using UnityEngine.SceneManagement;

// O orelhão: único ponto de salvamento manual do jogo (Regra R1).
// Grava cena, posição, ID do orelhão e todo o estado da sessão (flags, pistas, diário, ficha).
// Contextualiza também por que Marcus não consegue pedir socorro externo imediato pela linha sob tempestade.
public class PayphoneCheckpoint : Interactable
{
    [Header("Identificador Estável do Orelhão")]
    [SerializeField] private string payphoneId = "orelhao_encruzilhada";

    [Header("Orelhão")]
    [TextArea]
    [SerializeField]
    private string[] falas =
    {
        "\"Alô? ... Só chiado de chuva na linha.\"",
        "\"Tento discar para a casa de Adilio de novo. Chama até cair.\"",
        "\"A linha interurbana está muda por causa do temporal na serra.\"",
        "\"Se eu ligar para o plantão agora dizendo só que ele não atendeu hoje, vão mandar esperar até amanhã.\"",
        "\"Preciso descobrir por conta própria onde Adilio esteve hoje.\""
    };

    [SerializeField] private string textoDeSave = "Progresso salvo manualmente no orelhão.";

    public string PayphoneId => payphoneId;

    public void ConfigurarId(string novoId)
    {
        if (!string.IsNullOrEmpty(novoId))
        {
            payphoneId = novoId;
        }
    }

    public override string Prompt => "Atender / Salvar";

    public override void Interact()
    {
        var player = FindAnyObjectByType<PlayerController>();
        Vector2 savePos = player != null ? (Vector2)player.transform.position : (Vector2)transform.position;

        GameState.DefinirNumero(
            Flags.OrelhaoUsos,
            GameState.Numero(Flags.OrelhaoUsos) + 1
        );

        string falaEscolhida = ProximaFalaContextual();

        // Salva manualmente DEPOIS de atualizar os contadores para que o save fique sincronizado
        CheckpointSystem.Save(
            SceneManager.GetActiveScene().name,
            savePos,
            payphoneId
        );

        GameMessageUI.Instance.Show(
            $"{falaEscolhida}\n<size=75%><color=#88E098>[ {textoDeSave} ]</color></size>",
            "Marcus",
            "marcus"
        );
    }

    private string ProximaFalaContextual()
    {
        if (GameState.Tem(Flags.VenceuZeSinuca))
        {
            return "\"A linha continua chiando. Agora tenho a ficha do Clube e o que o Zé deixou escapar: Adilio não saiu de casa por vontade própria.\"";
        }

        if (GameState.Tem(Flags.FalouAtendenteMercadinho))
        {
            return "\"Só estática na linha. Se um desconhecido perguntou por Adilio no mercadinho e esteve com o Zé antes, a resposta está no bar da rua norte.\"";
        }

        if (GameState.Tem(Flags.AchouReciboMercadinho) || GameState.Tem(Flags.ExaminouDesordemCasa))
        {
            return "\"Sem linha limpa para fora da cidade. Com o café passado hoje e o tapete revirado na sala, não posso dar meia-volta.\"";
        }

        if (falas == null || falas.Length == 0)
        {
            return "\"Chiado de chuva na linha.\"";
        }

        int index = Mathf.Clamp(
            GameState.Numero(Flags.OrelhaoVozIndice),
            0,
            falas.Length - 1
        );

        string fala = falas[index];
        // Filtro de segurança contra qualquer string antiga serializada na cena que viole a Regra R1
        if (fala != null && (fala.Contains("clube vivo") || fala.Contains("Clube vivo")))
        {
            fala = "\"A linha interurbana está muda por causa do temporal na serra. Preciso verificar a cidade pessoalmente.\"";
        }

        if (index < falas.Length - 1)
        {
            GameState.DefinirNumero(Flags.OrelhaoVozIndice, index + 1);
        }

        return fala;
    }
}
