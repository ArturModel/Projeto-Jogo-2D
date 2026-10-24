using System;
using UnityEngine;

// Encenação local: âncoras de superfície explícitas, rotas curtas e poses de trabalho.
// Não calcula horários nem reage à chegada do jogador.
[DefaultExecutionOrder(125)]
public sealed class SantaRitaNpcWork : MonoBehaviour
{
    [Serializable] public struct Station
    {
        public Transform anchor;
        public float duration;
        public bool working;
        public bool faceLeft;
    }
    public Station[] stations;
    public Sprite[] poses;
    public float speed=.55f;
    private SpriteRenderer body;
    private Sprite idle;
    private int index;
    private float elapsed, distance;
    private bool initialized;
    public bool Working { get; private set; }
    public int StationIndex => index;
    private void Start()
    {
        body=GetComponent<SpriteRenderer>();idle=body.sprite;initialized=true;
        var legacy=GetComponent<SantaRitaNpcRoutine>();if(legacy!=null)legacy.enabled=false;
    }
    private void Update()
    {
        if(!initialized||stations==null||stations.Length==0||Time.timeScale==0||MenuPausa.Aberto||MapaInterativoUI.IsShowing||PayphoneCheckpoint.IsSaveUIOpen)return;
        var player=FindAnyObjectByType<PlayerController>();
        bool talking=GameMessageUI.IsShowing&&player!=null&&Vector2.Distance(player.transform.position,transform.position)<2.6f;
        if(talking||BochaMinigameUI.IsShowing)
        {Working=false;body.sprite=idle;if(talking)body.flipX=player.transform.position.x<transform.position.x;return;}
        var station=stations[index];if(station.anchor==null)return;
        Vector3 target=station.anchor.position;
        float delta=Vector2.Distance(transform.position,target);
        Working=delta<.04f&&station.working;
        if(delta>=.04f)
        {
            var next=Vector3.MoveTowards(transform.position,target,speed*Time.deltaTime);
            // Não atravessa Marcus; retoma quando a passagem desocupa.
            if(player==null||Vector2.Distance(next,player.transform.position)>.65f)
            {distance+=Vector3.Distance(next,transform.position);transform.position=next;}
            body.flipX=target.x<transform.position.x;
            body.sprite=poses!=null&&poses.Length>=8?poses[4+(int)(distance*6)%4]:idle;
        }
        else
        {
            elapsed+=Time.deltaTime;body.flipX=station.faceLeft;
            body.sprite=Working&&poses!=null&&poses.Length>=4?poses[(int)(elapsed*3)%4]:idle;
            float duration=station.duration*(GameState.Tem(Capitulo2Director.Vitoria)?1.35f:1);
            if(elapsed>=duration){elapsed=0;index=(index+1)%stations.Length;}
        }
        body.sortingOrder=1000-Mathf.RoundToInt(transform.position.y*10);
    }
    private void OnDisable(){if(body!=null&&idle!=null)body.sprite=idle;}
}
