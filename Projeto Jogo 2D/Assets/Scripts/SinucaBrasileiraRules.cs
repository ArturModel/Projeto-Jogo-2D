using System;
using System.Collections.Generic;

// Pure rules kernel. Point values/ordered on-ball follow CBBS; the marked-ball ban is the explicit story variant.
public sealed class SinucaBrasileiraRules
{
    public enum Sequence { Start, MustOn, FreeColor, OptionalColor }
    public readonly bool[] Present = { false,true,true,true,true,true,true,true };
    public int MarcusPoints { get; private set; }
    public int ZePoints { get; private set; }
    public bool MarcusTurn { get; private set; }
    public bool Opening { get; private set; } = true;
    public Sequence Phase { get; private set; }
    public bool Finished { get; private set; }
    public int Winner { get; private set; } = -1;
    public bool MayRefuse { get; private set; }
    public int OnBall { get { for(int i=1;i<=7;i++)if(Present[i])return i; return 0; } }
    public sealed class Result
    {
        public bool foul, replayOpening, cueInHand;
        public string reason;
        public readonly List<int> respot=new List<int>();
        public int credited;
    }
    public SinucaBrasileiraRules(bool marcusStarts){MarcusTurn=marcusStarts;}
    public bool CanNominate(int n){return n>=1&&n<=7&&Present[n]&&(!Opening||n==OnBall)&&(Phase!=Sequence.MustOn||n==OnBall);}
    public void Refuse(){if(!MayRefuse||Finished)return;MarcusTurn=!MarcusTurn;MayRefuse=false;Phase=Sequence.Start;}
    public Result Resolve(int nominated,int first,IList<int> potted,bool scratch,int marked,bool defense=false)
    {
        if(Finished)throw new InvalidOperationException("Frame already finished");
        int on=OnBall;
        var r=new Result { cueInHand=scratch };
        bool potTarget=potted.Contains(nominated);
        bool color=nominated!=on || (potTarget && (Phase==Sequence.FreeColor || Phase==Sequence.OptionalColor));
        bool castigo=color&&Phase!=Sequence.FreeColor;
        bool wrong=!CanNominate(nominated)||first!=nominated;
        bool markedPot=marked>0&&potted.Contains(marked);
        r.foul=scratch||wrong||potted.Count>1||(potted.Count==1&&!potTarget)||markedPot||
            (color&&defense)||(castigo&&!potTarget);
        r.reason=markedPot?"Bola marcada convertida":scratch?"Branca encaçapada":wrong?"Primeiro contato diferente da bola cantada":
            potted.Count>1?"Duas bolas na mesma tacada":color&&defense?"Colorida jogada em defesa":castigo&&!potTarget?"Colorida com castigo não convertida":"Bola não visada convertida";
        MayRefuse=false;
        if(Opening)
        {
            foreach(int n in potted)if(n>0&&n<=7)r.respot.Add(n);
            if(r.foul||potTarget){r.respot.Clear();for(int i=1;i<=7;i++)if(Present[i])r.respot.Add(i);r.replayOpening=true;r.cueInHand=true;return r;}
            Opening=false;MarcusTurn=!MarcusTurn;Phase=Sequence.Start;return r;
        }
        if(r.foul)
        {
            foreach(int n in potted)if(n>0&&n<=7)r.respot.Add(n);
            if(MarcusTurn)ZePoints+=7;else MarcusPoints+=7;
            r.credited=7;MarcusTurn=!MarcusTurn;Phase=Sequence.Start;MayRefuse=true;
        }
        else if(potTarget)
        {
            if(MarcusTurn)MarcusPoints+=nominated;else ZePoints+=nominated;
            r.credited=nominated;
            if(color){r.respot.Add(nominated);Phase=Phase==Sequence.FreeColor?Sequence.OptionalColor:Sequence.MustOn;}
            else {Present[nominated]=false;Phase=Sequence.FreeColor;}
        }
        else {MarcusTurn=!MarcusTurn;Phase=Sequence.Start;MayRefuse=color;}
        int remaining=OnBall, gap=Math.Abs(MarcusPoints-ZePoints);
        bool end=remaining==0||(remaining==5&&gap>46)||(remaining==6&&gap>27)||(remaining==7&&gap>7);
        if(end)
        {
            if(MarcusPoints==ZePoints){Present[7]=true;Opening=true;Phase=Sequence.Start;MarcusTurn=!MarcusTurn;r.cueInHand=true;r.respot.Add(7);}
            else {Finished=true;Winner=MarcusPoints>ZePoints?0:1;}
        }
        return r;
    }
}
