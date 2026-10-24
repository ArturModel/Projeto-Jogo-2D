using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Regressões do kernel real. Autoria/teste fora do Player normal.
public static class BochaPerspectiveVerification
{
    public static string Run()
    {
        var report=new List<string>();
        void Check(bool pass,string name){report.Add((pass?"PASS ":"FAIL ")+name);if(!pass)throw new Exception(name);}
        BochaBall Ball(float x,float z,float vx=0,float vz=0)=>new BochaBall{pos=new Vector2(x,z),vel=new Vector2(vx,vz),raio=.18f,massa=1,giro=new Vector3(vz,0,-vx)/.18f};
        var s=new BochaSimulation();var a=Ball(-.05f,10,14);var b=Ball(0,10.3595f);s.bolas.Add(a);s.bolas.Add(b);int hits=0;s.Contato=_=>hits++;s.Step();
        Check(hits>0&&b.vel.magnitude>.1f,"Raspada varrida: caso antigo de tunneling transfere impulso.");
        s=new BochaSimulation();a=Ball(0,10,0,6);a.giro=Vector3.zero;b=Ball(0,10.36f);s.bolas.Add(a);s.bolas.Add(b);s.Step(.001f);
        Check(b.vel.y>4.9f&&a.vel.y>0&&a.vel.y<1.1f,"Batida frontal igual massa transfere velocidade com restituição.");
        Check(a.vel.y+b.vel.y<=6.001f&&a.vel.y+b.vel.y>5.98f,"Impulso conserva momento horizontal, descontada resistência do piso.");
        float energy=a.vel.sqrMagnitude+b.vel.sqrMagnitude;
        Check(energy<36,"Colisão dissipa energia translacional.");
        foreach(bool small in new[]{false,true})foreach(int axis in new[]{0,1})
        {
            s=new BochaSimulation();a=small?BochaBall.Balim(new Vector2(0,19.8f)):Ball(0,19.8f);
            a.pos=axis==0?new Vector2(2.88f,10):a.pos;a.vel=axis==0?new Vector2(12,0):new Vector2(0,12);s.bolas.Add(a);s.Step(.1f);
            Check(a.valida&&(axis==0?a.vel.x<0:a.vel.y<0),"Guia/cabeceira devolve "+(small?"balim":"bocha")+" eixo "+axis);
            s.RepousarParaPrevisao();Check(!s.IsMoving&&Mathf.Abs(a.pos.x)<=3-a.raio+.0001f&&a.pos.y>=a.raio&&a.pos.y<=20-a.raio+.0001f,"Repouso contido, sem desaparecer.");
        }
        float previous=0;
        foreach(float force in new[]{.15f,.35f,.55f,.75f})
        {
            var match=new BochaMatch();s=match.CriarMao();s.bolas[0].pos=new Vector2(2.5f,19);
            match.Lancar(s,BochaSide.Marcus,BochaLaunch.Preparar(BochaThrowType.Ponto,0,Vector2.up,force));s.RepousarParaPrevisao();
            Check(!s.IsMoving&&s.bolas[1].pos.y>previous,"Ponto: mais força produz maior alcance antes da cabeceira "+force);previous=s.bolas[1].pos.y;
        }
        foreach(var mode in new[]{BochaThrowType.Ponto,BochaThrowType.Rafa,BochaThrowType.Volo})
        {
            var match=new BochaMatch();s=match.CriarMao();match.Lancar(s,BochaSide.Marcus,BochaLaunch.Preparar(mode,0,Vector2.up,.7f));
            float peak=0;int drops=0;s.Contato=c=>{if(c.tipo==BochaContactType.Saibro)drops++;};
            for(int i=0;i<4200&&s.IsMoving;i++){s.Step();peak=Mathf.Max(peak,s.bolas[1].altura);}
            Check(!s.IsMoving&&s.bolas.TrueForAll(q=>q.valida&&q.altura==0),"Todos repousam no saibro: "+mode);
            Check(mode==BochaThrowType.Volo?peak>2&&drops>0:peak<.01f,"Altura e queda coerentes com modo "+mode);
        }
        // Em pleno voo a esfera passa acima de uma bocha sem colisão fantasma no plano.
        s=new BochaSimulation();a=Ball(0,9.7f,0,10);a.altura=1;a.vertical=1;b=Ball(0,10);s.bolas.Add(a);s.bolas.Add(b);s.Step(.08f);
        Check(b.vel==Vector2.zero&&a.pos.y>10,"Altura participa do contato: voo passa acima de obstáculo.");
        s=new BochaSimulation();s.bolas.Add(Ball(0,3,1,7));var copy=s.Clone();for(int i=0;i<120;i++)s.Step();for(int i=0;i<60;i++)copy.Step(1f/60);
        Check(Vector2.Distance(s.bolas[0].pos,copy.bolas[0].pos)<.0001f,"Mesmo segundo físico em 60 ou 120 chamadas.");
        s.Impacto=_=>{};s.Contato=_=>{};copy=s.Clone();copy.bolas[0].pos=Vector2.zero;
        Check(copy.Impacto==null&&copy.Contato==null&&s.bolas[0].pos!=Vector2.zero,"Previsão copia estado sem áudio nem mutação da partida.");
        s=new BochaSimulation();for(int i=0;i<9;i++)s.bolas.Add(Ball(2.5f+(i%3)*.1f,19.2f+(i/3)*.1f,5,8));s.RepousarParaPrevisao();
        Check(!s.IsMoving&&s.bolas.TrueForAll(q=>q.valida&&!float.IsNaN(q.pos.x)&&Mathf.Abs(q.pos.x)<=2.821f&&q.pos.y<=19.821f),"Pilha no canto converge sem explosão numérica.");
        bool rejected=false;try{s.Step(float.NaN);}catch(ArgumentOutOfRangeException){rejected=true;}Check(rejected,"Passo NaN é recusado.");
        Check(BochaMinigameUI.ForcaNoTempo(.2f)<BochaMinigameUI.ForcaNoTempo(1.3f)&&BochaMinigameUI.ForcaNoTempo(2.7f)<BochaMinigameUI.ForcaNoTempo(1.7f)&&Mathf.Abs(BochaMinigameUI.ForcaNoTempo(3.2f)-BochaMinigameUI.ForcaNoTempo(.2f))<.001f,"Carga sobe, desce e repete sem saturar no máximo.");
        string output=Path.GetFullPath(Path.Combine(Application.dataPath,"../../Docs/Evidencias/BochaPerspectiva"));Directory.CreateDirectory(output);File.WriteAllLines(Path.Combine(output,"fisica.txt"),report);
        return "PASS "+report.Count+" regressões físicas/força.";
    }
}
