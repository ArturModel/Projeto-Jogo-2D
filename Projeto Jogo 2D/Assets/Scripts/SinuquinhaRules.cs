using System;
using System.Collections.Generic;
// House eight-ball: 15 object balls, black last. Presentation by suits or number ranges.
public sealed class SinuquinhaRules
{
 public enum Sequence{Start,MustOn,FreeColor,OptionalColor}
 public readonly bool[] Present=new bool[16];
 public int MarcusPoints{get;private set;} public int ZePoints{get;private set;}
 public bool MarcusTurn{get;private set;} public bool Opening{get;private set;}=true;
 public Sequence Phase{get;private set;} public bool Finished{get;private set;} public int Winner{get;private set;}=-1;
 public bool MayRefuse=>false; public int MarcusGroup{get;private set;} // 0 open, 1 solids, 2 stripes
 public sealed class Result{public bool foul,replayOpening,cueInHand,revertShot;public string reason;public int credited;public readonly List<int> respot=new List<int>();}
 public SinuquinhaRules(bool starts){MarcusTurn=starts;for(int i=1;i<16;i++)Present[i]=true;}
 public static int Group(int n)=>n==8||n<1||n>15?0:n<8?1:2;
 public int OnBall{get{for(int i=1;i<16;i++)if(CanNominate(i))return i;return 0;}}
 public bool CanNominate(int n){if(n<1||n>15||!Present[n])return false;if(Opening)return n!=8;int group=MarcusGroup==0?0:MarcusTurn?MarcusGroup:3-MarcusGroup;if(n==8){if(group==0)return false;for(int i=1;i<16;i++)if(Present[i]&&Group(i)==group)return false;return true;}return group==0||Group(n)==group;}
 public void Refuse(){}
 public Result Resolve(int nominated,int first,IList<int> potted,bool scratch,int marked,bool defense=false)
 {
  if(Finished)throw new InvalidOperationException("Partida encerrada");var r=new Result();bool legal=CanNominate(first);bool blackLegal=CanNominate(8);int shooter=MarcusTurn?0:1;
  if(marked>0&&marked!=8&&potted.Contains(marked)){r.foul=r.cueInHand=r.revertShot=true;r.reason="Bola marcada caiu: tacada anulada";Opening=false;MarcusTurn=!MarcusTurn;return r;}
  r.foul=scratch||!legal||(!Opening&&defense&&potted.Count==0);r.cueInHand=r.foul;r.reason=scratch?"Branca na caçapa":!legal?"Primeiro contato fora do seu grupo":defense?"Nenhuma bola tocou tabela ou caçapa":"";
  bool own=false;int group=MarcusGroup==0?0:MarcusTurn?MarcusGroup:3-MarcusGroup;
  foreach(int n in potted){if(n<1||n>15)continue;if(n==8){if(Opening){r.respot.Add(8);continue;}Finished=true;Winner=blackLegal&&!r.foul?shooter:1-shooter;Present[8]=false;continue;}
   if(!r.foul&&group==0){group=Group(n);MarcusGroup=MarcusTurn?group:3-group;}
   Present[n]=false;if(Group(n)==group){own=true;if(MarcusTurn)MarcusPoints++;else ZePoints++;r.credited++;}
  }
  Opening=false;if(r.foul||!own)MarcusTurn=!MarcusTurn;return r;
 }
}
