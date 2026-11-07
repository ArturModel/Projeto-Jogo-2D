using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Perception is transient. No extra physical ball, scoring rule or save flag.
public partial class SinucaMinigameUI
{
    private struct FalaHistoria { public string texto; public bool marcus; }
    private readonly Queue<FalaHistoria> falasHistoria = new Queue<FalaHistoria>();
    private bool locutorPendenteMarcus;
    private bool batidasOuvidas, inspecaoConcluida;
    private readonly SinucaPerceptionSchedule agendaPercepcoes = new SinucaPerceptionSchedule(Guid.NewGuid().GetHashCode());
    private AudioClip ecoFesta;
    private bool PercepcaoSobMesaAtiva => agendaPercepcoes.Active == SinucaPerceptionSchedule.Event.UnderTable;
    private bool FalaPercepcaoOcupada => falasHistoria.Count > 0 ||
        !string.IsNullOrEmpty(falaZePendente) || dialogoZe?.Ocupada == true;
    private bool bolaFalsaAtiva, bolaFalsaExaminada;
    private int numeroFalso = -1, turnoBolaFalsa = -1;
    private Vector2 posicaoFalsa;
    private float proximaBatida, tempoSobMesa, exameBola;
    private int batidasRestantes, faseSobMesa;
    private AudioSource audioSobMesa;
    private AudioClip batidaSobMesa, aguaSobMesa;
    private bool audioPercepcaoSuspenso;
    private bool OlhandoDebaixo => tableView3D!=null && tableView3D.Room!=null && tableView3D.Room.InspectingUnderTable;

    private void FalaHistoriaEnfileirar(string texto,bool marcus=true)
    { falasHistoria.Enqueue(new FalaHistoria{texto=texto,marcus=marcus}); }
    private void ProximaFalaHistoria()
    {
        if(falasHistoria.Count==0 || dialogoZe == null || dialogoZe.Ocupada || !string.IsNullOrEmpty(falaZePendente))return;
        var fala=falasHistoria.Dequeue();falaZePendente=fala.texto;falaZePendenteUrgente=true;
        locutorPendenteMarcus=fala.marcus;intervaloFalaZe=0;
    }
    private void ReiniciarPercepcoesSinuca()
    {
        EncerrarPercepcaoDaMao();falasHistoria.Clear();
        batidasOuvidas=inspecaoConcluida=false;
        exameBola=0;agendaPercepcoes.Reset();locutorPendenteMarcus=false;
    }
    private void EncerrarPercepcaoDaMao(bool preservarFalas=false)
    {
        agendaPercepcoes.Finish();
        batidasOuvidas=false;inspecaoConcluida=true;
        if(tableView3D!=null && tableView3D.Room!=null){tableView3D.Room.EndUnderTable();tableView3D.Room.SetFalseBall(false,1,Vector2.zero);}
        if(audioSobMesa!=null)audioSobMesa.Stop();
        bolaFalsaAtiva=false;batidasRestantes=0;tempoSobMesa=0;faseSobMesa=0;
        numeroFalso=-1;turnoBolaFalsa=-1;if(!preservarFalas)falasHistoria.Clear();
        if(txtPercepcaoSubtitulo!=null)txtPercepcaoSubtitulo.text="";
    }
    private bool PrepararBolaFalsa()
    {
        bolaFalsaAtiva=false;
        if(modoPratica || agendaPercepcoes.Active != SinucaPerceptionSchedule.Event.FalseBall || OlhandoDebaixo || batidasRestantes>0)return false;
        numeroFalso=-1;
        for(int n=1;n<16;n++)if(n!=8 && bolas[n].ativa && (regrasBrasileiras==null || regrasBrasileiras.CanNominate(n))){numeroFalso=n;break;}
        if(numeroFalso<0)return false;
        // A tempting free position, without covering an actual ball or pocket.
        posicaoFalsa=EncontrarPosicaoLivreDeterministica(new Vector2(-15f,48f),-1);
        if(!PosicaoValidaSemColisao(posicaoFalsa,-1))return false;
        bolaFalsaAtiva=true;bolaFalsaExaminada=false;
        turnoBolaFalsa=contadorTurnosMao;exameBola=0;return true;
    }
    private void AtualizarBolaFalsaVisual()
    {
        if(tableView3D==null || tableView3D.Room==null)return;
        if(modoPratica || contadorTurnosMao!=turnoBolaFalsa || estado==EstadoPartida.FimDeMao || estado==EstadoPartida.Inativo)bolaFalsaAtiva=false;
        // The cue can pass through the perception. Nothing is added to the solver.
        if(bolaFalsaAtiva && estado==EstadoPartida.BolasEmMovimento && Vector2.Distance(bolas[0].pos,posicaoFalsa)<BallRadius*2.4f)
        {
            bolaFalsaAtiva=false;
            FalaHistoriaEnfileirar("Eu vi a bola ali. A branca atravessou...");
        }
        tableView3D.Room.SetFalseBall(bolaFalsaAtiva,numeroFalso,posicaoFalsa);
    }
    private void AplicarGuiaBolaFalsa(ref SinucaAimGuideData guia)
    {
        if(!guia.ativa || !bolaFalsaAtiva || bolaFalsaExaminada)return;
        Vector2 dir=new Vector2(Mathf.Cos(anguloMiraRad),Mathf.Sin(anguloMiraRad));
        var delta=posicaoFalsa-guia.origem;float proj=Vector2.Dot(delta,dir),r=BallRadius*2;
        float perp=delta.sqrMagnitude-proj*proj;
        if(proj<=0 || perp>=r*r)return;
        float hit=proj-Mathf.Sqrt(r*r-perp);
        if(hit<0 || hit>Vector2.Distance(guia.origem,guia.pontoContato))return;
        guia.pontoContato=guia.origem+dir*hit;guia.atingeBola=true;guia.numeroBolaAlvo=16;
        guia.posBolaAlvo=posicaoFalsa;guia.direcaoBolaAlvo=(posicaoFalsa-guia.pontoContato).normalized;guia.alvoEhMarcada=false;
    }
    private bool AtualizarPercepcoesSinuca()
    {
        if(modoPratica || tableView3D==null || tableView3D.Room==null)return false;
        var sala=tableView3D.Room;var kb=Keyboard.current;
        float dt=Time.deltaTime;
        if(interfaceLivre)return false;
        AtualizarAgendaPercepcoes(dt);
        ProximaFalaHistoria();
        if(OlhandoDebaixo)
        {
            if(kb!=null && (kb.eKey.wasPressedThisFrame || kb.qKey.wasPressedThisFrame)){SairDeBaixoDaMesa();return true;}
            var mouse=Mouse.current;
            if(mouse!=null && Cursor.lockState==CursorLockMode.Locked)sala.LookUnderTable(mouse.delta.ReadValue());
            tempoSobMesa+=dt;
            sala.SetUnderTableMemory(tempoSobMesa>=1.5f && tempoSobMesa<8.5f,tempoSobMesa,MenuPausa.ReduzirEfeitosIntensos);
            txtControlesRodape.text="Mouse: olhar debaixo da mesa · E / Q / Esc: levantar";
            if(faseSobMesa==0 && tempoSobMesa>=1.5f)
            {
                faseSobMesa=1;TocarSobMesa(true);
                txtPercepcaoSubtitulo.text="[Gotinhas caem da madeira. Três batidas no tampo, bem acima da sua cabeça.]";
                FalaHistoriaEnfileirar("Está pingando... de dentro da madeira.");
            }
            if(faseSobMesa==1 && tempoSobMesa>=4.8f)
            {
                faseSobMesa=2;batidasRestantes=3;proximaBatida=0;
                FalaHistoriaEnfileirar("Tem alguém aí embaixo?");
            }
            if(faseSobMesa==2 && tempoSobMesa>=8.5f)
            {
                faseSobMesa=3;
                txtPercepcaoSubtitulo.text="[As marcas desaparecem. Só madeira seca. O bar continua.]";
                FalaHistoriaEnfileirar("Essas batidas... eu já ouvi isso antes.");
            }
            AtualizarBatidas(dt);AtualizarVisualTacoEMira();return true;
        }
        bool seguro=estado==EstadoPartida.MirandoJogador && turnoDeMarcus;
        AtualizarBatidas(dt);
        bool perto=sala.CanInspectUnderTable;
        if(PercepcaoSobMesaAtiva && batidasOuvidas && !inspecaoConcluida && seguro)
        {
            txtPercepcaoSubtitulo.text="[Três batidas abafadas vêm debaixo do tampo.]";
            txtControlesRodape.text=EmPe?(perto?"E: abaixar e olhar debaixo da mesa · Mouse: olhar":"Aproxime-se da lateral da mesa para investigar o som."):"Q: levantar para investigar as batidas sob a mesa.";
        }
        if(bolaFalsaAtiva && EmPe && seguro && sala.LookingAtFalseBall)
        {
            txtControlesRodape.text="Segure E para observar esta bola.";
            if(kb!=null && kb.eKey.isPressed && !interfaceLivre)
            {
                exameBola+=dt;
                if(exameBola>=1.15f)
                {
                    bolaFalsaExaminada=true;bolaFalsaAtiva=false;
                    sala.SetFalseBall(false,numeroFalso,posicaoFalsa);
                    FalaHistoriaEnfileirar("Duas com o mesmo número... e essa não fazia sombra.");
                }
                return true;
            }
        }
        else exameBola=0;
        if(PercepcaoSobMesaAtiva && seguro && EmPe && perto && batidasOuvidas && !inspecaoConcluida && !interfaceLivre && kb!=null && kb.eKey.wasPressedThisFrame)
        {
            CancelarCargaAoSuspender();levantarAposTacada=-1;tempoSobMesa=0;faseSobMesa=0;
            sala.BeginUnderTable();AtualizarVisualTacoEMira();return true;
        }
        AtualizarBolaFalsaVisual();return false;
    }
    private void AtualizarAgendaPercepcoes(float dt)
    {
        bool seguro=estado==EstadoPartida.MirandoJogador && turnoDeMarcus && EmPe && !interfaceLivre;
        var inicio=agendaPercepcoes.Tick(dt,seguro,FalaPercepcaoOcupada);
        switch(inicio)
        {
            case SinucaPerceptionSchedule.Event.UnderTable:
                batidasOuvidas=true;inspecaoConcluida=false;batidasRestantes=3;proximaBatida=0;
                FalaHistoriaEnfileirar("Espera. Tem alguma coisa batendo debaixo da mesa.");
                break;
            case SinucaPerceptionSchedule.Event.FalseBall:
                if(!PrepararBolaFalsa())agendaPercepcoes.Finish();
                break;
            case SinucaPerceptionSchedule.Event.WaterInPipes:
                TocarSobMesa(true);
                txtPercepcaoSubtitulo.text="[Água corre dentro de um cano. O chão junto à mesa continua seco.]";
                FalaHistoriaEnfileirar("Esse cano passa por aqui?");
                break;
            case SinucaPerceptionSchedule.Event.PartyEcho:
                // Reuse the project's original abstract crowd texture; no new voices or music.
                TocarSobMesa(false);audioSobMesa.Stop();
                if(ecoFesta==null)ecoFesta=SantaRitaBarAudio.CriarTextura(0);
                audioSobMesa.pitch=.92f;audioSobMesa.PlayOneShot(ecoFesta,.34f);
                txtPercepcaoSubtitulo.text="[Um salão cheio, distante. As conversas do bar continuam perto de você.]";
                FalaHistoriaEnfileirar("Parecia que tinha muito mais gente conversando.");
                break;
        }
        var ativo=agendaPercepcoes.Active;
        if(ativo==SinucaPerceptionSchedule.Event.None)return;
        float limite=ativo==SinucaPerceptionSchedule.Event.UnderTable?24:
            ativo==SinucaPerceptionSchedule.Event.FalseBall?16:7;
        if(OlhandoDebaixo)
        {
            if(tempoSobMesa>=12)SairDeBaixoDaMesa();
            return;
        }
        if((ativo==SinucaPerceptionSchedule.Event.FalseBall && !bolaFalsaAtiva) || agendaPercepcoes.Age>=limite)
        {
            EncerrarPercepcaoDaMao(true);
        }
        AtualizarVolumeSobMesa();
    }
    private void SairDeBaixoDaMesa()
    {
        if(tableView3D==null || tableView3D.Room==null)return;
        bool viu=tempoSobMesa>=1.5f;
        tableView3D.Room.EndUnderTable();batidasRestantes=0;
        if(audioSobMesa!=null)audioSobMesa.Stop();
        inspecaoConcluida=true;
        agendaPercepcoes.Finish();batidasOuvidas=false;
        if(viu)
        {
            FalaHistoriaEnfileirar("Eu ouvi daqui. Bem debaixo da mesa.");
            FalaHistoriaEnfileirar("É a chuva, Marcus. Joga.",false);
            FalaHistoriaEnfileirar("Não parecia chuva.");
        }
        txtPercepcaoSubtitulo.text=viu?"[O chão está seco. Ninguém no bar parece ter parado para ouvir.]":"[As batidas cessam quando você se levanta.]";
        frameCaptura=Time.frameCount;tableView3D.ReiniciarReferenciaPonteiro();AtualizarVisualTacoEMira();
    }
    private void AtualizarBatidas(float dt)
    {
        if(batidasRestantes<=0)return;
        proximaBatida-=dt;
        if(proximaBatida>0)return;
        TocarSobMesa(false);batidasRestantes--;proximaBatida=batidasRestantes==1?.72f:.34f;
    }
    private void SuspenderAudioPercepcao(bool suspenso)
    {
        audioPercepcaoSuspenso=suspenso;
        if(audioSobMesa==null)return;
        if(suspenso)audioSobMesa.Pause();
        else {audioSobMesa.UnPause();AtualizarVolumeSobMesa();}
    }
    private void AtualizarVolumeSobMesa()
    {
        if(audioSobMesa==null || tableView3D==null || tableView3D.Room==null)return;
        var cam=tableView3D.Room.camera.transform;var delta=new Vector3(0,.46f,0)-cam.localPosition;
        audioSobMesa.panStereo=Mathf.Clamp(Vector3.Dot(cam.right,delta.normalized)*.65f,-.65f,.65f);
        audioSobMesa.volume=.52f*AudioSettingsController.VolumeImpactos/(1+delta.magnitude*.12f);
    }
    private void TocarSobMesa(bool agua)
    {
        if(audioPercepcaoSuspenso)return;
        if(audioSobMesa==null){audioSobMesa=gameObject.AddComponent<AudioSource>();audioSobMesa.playOnAwake=false;audioSobMesa.spatialBlend=0;}
        if(batidaSobMesa==null)batidaSobMesa=Resources.Load<AudioClip>("Audio/Foley/door");
        if(aguaSobMesa==null)aguaSobMesa=CriarAudioSobMesa(true);
        AtualizarVolumeSobMesa();audioSobMesa.pitch=agua?1f:.97f;
        var clip=agua?aguaSobMesa:batidaSobMesa;
        if(clip!=null)audioSobMesa.PlayOneShot(clip,agua?.58f:1f);
    }
    // Original procedural sound design, not a recording of a child or spoken dialogue.
    private static AudioClip CriarAudioSobMesa(bool agua)
    {
        const int hz=22050;int count=(int)(hz*(agua?2.6f:.26f));var data=new float[count];uint noise=7319;
        float low=0;
        for(int i=0;i<count;i++)
        {
            float t=i/(float)hz;noise=1664525*noise+1013904223;
            float n=((noise>>8)/8388607.5f-1);low=Mathf.Lerp(low,n,.08f);
            float env=agua?Mathf.Sin(Mathf.PI*i/(count-1)):Mathf.Min(1,t/.004f)*Mathf.Exp(-t*21f);
            float wave=agua?low*.38f+Mathf.Sin(t*225f+Mathf.Sin(t*31f)*3)*.07f:
                Mathf.Sin(2*Mathf.PI*143*t)*.42f+Mathf.Sin(2*Mathf.PI*287*t)*.16f+low*.17f;
            data[i]=wave*env*.8f;
        }
        var clip=AudioClip.Create(agua?"SR_Memoria_Agua_Original":"SR_SobMesa_Batida_Original",count,1,hz,false);clip.SetData(data,0);return clip;
    }
    private void DestruirAudioPercepcao()
    { if(aguaSobMesa!=null)Destroy(aguaSobMesa);if(ecoFesta!=null)Destroy(ecoFesta); }
}
