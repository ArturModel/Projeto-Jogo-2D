#if UNITY_EDITOR || DEBUG
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public sealed partial class Chapter2RuntimeVerification
{
    private bool portalForce;
#if UNITY_EDITOR
    public static void StartPortalInEditor(string output)
    {
        Environment.SetEnvironmentVariable("SR_EVIDENCE_DIR",output);
        var host=new GameObject("QA_Portas_E_Explicito");DontDestroyOnLoad(host);host.AddComponent<Chapter2RuntimeVerification>().portalForce=true;
    }
#endif
    private static readonly FieldInfo portalClock=typeof(Capitulo2Portal).GetField("ultimoCorte",BindingFlags.NonPublic|BindingFlags.Static);
    private IEnumerator RunPortalRegression()
    {
        report.Add("Fixture em RAM: progressão C1 liberada e reposicionamentos nas portas. E pelo Input System/PlayerController real; relógio da última transição semeado para reproduzir a tentativa descartada. Saves pessoais isolados.");
        Application.targetFrameRate=60;QualitySettings.vSyncCount=0;
        foreach(string flag in new[]{Flags.ZeDerrotadoNaSinuca,Flags.RecebeuFichaClubeZe,Flags.MapaDoCarroObtido,Flags.LanternaDoCarroObtida})GameState.Marcar(flag);
        SceneTransition.Ir("CentroSantaRita","c1_portao_norte");yield return AwaitScene("CentroSantaRita");yield return CloseText();
        foreach(string room in new[]{"SociedadeBeiraRio","IgrejaMatriz","PrefeituraSantaRita","RodoviariaSantaRita","MercadinhoDeposito","OficinaRadio","CasaCostureira"})
        {
            var door=FindObjectsByType<Capitulo2Portal>().Single(d=>d.cenaDestino==room);
            yield return Place(door.transform.position+Vector3.down*.6f);
            Check(Interactable.FindNearest(FindAnyObjectByType<PlayerController>().transform.position)==door,"Prompt e E selecionam entrada de "+room);
            portalClock.SetValue(null,Time.unscaledTime-.1f);
            yield return TapR2(Key.E);
            Check(SceneFader.IsTransitioning||SceneManager.GetActiveScene().name==room,"E abre "+room+" mesmo logo após outra transição.");
            yield return AwaitScene(room);yield return CloseText();yield return Capture("entrada-"+room);
            var exit=FindObjectsByType<Capitulo2Portal>().First(d=>d.cenaDestino=="CentroSantaRita");
            yield return Place(exit.transform.position+Vector3.up*.45f);
            portalClock.SetValue(null,Time.unscaledTime+600);
            yield return TapR2(Key.E);
            Check(SceneFader.IsTransitioning||SceneManager.GetActiveScene().name=="CentroSantaRita","E sai de "+room+" sem bloqueio por relógio reiniciado.");
            yield return AwaitScene("CentroSantaRita");yield return CloseText();
        }
        // Callback de nova sessão também deve funcionar com Domain Reload desativado.
        var reset=typeof(Capitulo2Portal).GetMethod("ResetSession",BindingFlags.NonPublic|BindingFlags.Static);
        Check(reset!=null,"Portais possuem reset explícito ao iniciar Play.");
        reset.Invoke(null,null);Check(float.IsNegativeInfinity((float)portalClock.GetValue(null)),"Nova sessão libera primeira passagem automática.");
        var hostAuto=new GameObject("QA_Portal_Automatico");hostAuto.transform.position=new Vector2(-1000,-1000);
        var auto=hostAuto.AddComponent<Capitulo2Portal>();auto.automatico=true;auto.cenaDestino="SociedadeBeiraRio";auto.entradaDestino="centro";
        portalClock.SetValue(null,Time.unscaledTime-.1f);auto.Interact();Check(!SceneFader.IsTransitioning,"Passagem automática conserva proteção contra retorno imediato.");
        portalClock.SetValue(null,Time.unscaledTime+60);auto.Interact();Check(SceneFader.IsTransitioning,"Relógio recuado não prende passagem automática indefinidamente.");
        var second=FindObjectsByType<Capitulo2Portal>().First(d=>d.cenaDestino=="PrefeituraSantaRita");string original=second.entradaDestino;second.entradaDestino="qa_nao_deve_substituir";second.Interact();second.entradaDestino=original;
        var entry=typeof(SceneTransition).GetField("entradaPendente",BindingFlags.NonPublic|BindingFlags.Static).GetValue(null) as string;
        Check(entry=="centro","Segundo portal durante fade não sobrescreve a entrada pendente.");
        yield return AwaitScene("SociedadeBeiraRio");yield return CloseText();
        var actualExit=FindObjectsByType<Capitulo2Portal>().First(d=>d.cenaDestino=="CentroSantaRita");
        GameMessageUI.Instance.Show("QA de bloqueio durante leitura.","MARCUS","marcus");actualExit.Interact();Check(!SceneFader.IsTransitioning,"Leitura continua bloqueando transição sob a interface.");yield return CloseText();
        Check(!CheckpointSystem.HasCheckpoint,"Entradas, saídas e reinício do relógio não gravam checkpoint.");Finish();
    }
}
#endif
