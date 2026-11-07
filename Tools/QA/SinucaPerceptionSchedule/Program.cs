using E = SinucaPerceptionSchedule.Event;
static class Program
{
    static int checks;
    static void Check(bool ok, string name) { checks++; if(!ok)throw new Exception(name); }
    static void Main()
    {
        CheckForcedEvents();
        var starts = new HashSet<int>(); var kinds = new HashSet<E>();
        for(int seed=0;seed<1000;seed++)
        {
            var s=new SinucaPerceptionSchedule(seed);
            Check(s.Remaining>=40 && s.Remaining<=85,"initial interval");
            for(int j=0;j<5;j++)s.TurnStarted();
            float before=s.Remaining;
            s.Tick(900,true,true);Check(s.Remaining==before && s.Active==E.None,"speech freezes idle timer");
            s.Tick(0,true,false);s.Tick(float.NaN,true,false);s.Tick(float.PositiveInfinity,true,false);
            Check(s.Remaining==before,"invalid/paused delta ignored");
            s.Tick(900,false,false);Check(s.Active==E.None,"unsafe state defers trigger");
            var first=s.Tick(.1f,true,false);Check(first!=E.None,"safe state starts one event");kinds.Add(first);
            s.Tick(900,true,false);Check(s.Active==first,"active event never replaced");
            s.Finish();Check(s.Active==E.None && s.Remaining>=55 && s.Remaining<=115,"finish gives silence");
            s.Tick(900,true,false);Check(s.Active==E.None,"new turns required");
            s.Reset();for(int j=0;j<5;j++)s.TurnStarted();
            float elapsed=0;E previous=E.None;var bag=new HashSet<E>();
            for(int n=0;n<25;n++)
            {
                for(int j=0;j<3;j++)s.TurnStarted();
                while(s.Active==E.None){s.Tick(.5f,true,false);elapsed+=.5f;Check(elapsed<10000,"bounded schedule");}
                if(n==0)starts.Add((int)elapsed);
                Check(s.Active!=previous,"no adjacent repetition");
                Check(bag.Add(s.Active),"each shuffled bag contains five distinct events");
                if(n%5==4)bag.Clear();
                previous=s.Active;s.Finish();
                float cooldown=s.Remaining;s.Finish();Check(s.Remaining==cooldown,"hand end does not restart idle interval");
            }
            Check(elapsed>1200,"events spread over long series");
        }
        Check(starts.Count>30,"opening varies across sessions");Check(kinds.Count==5,"any event can open");
        Console.WriteLine($"PASS {checks} checks / 1000 seeds / 25000 random events; {starts.Count} distinct opening seconds; all five opening types.");
    }

    static void CheckForcedEvents()
    {
        var schedule = new SinucaPerceptionSchedule(317);
        Check(schedule.StartForced(E.None) == E.None, "none cannot be forced");
        Check(schedule.StartForced((E)99) == E.None, "unknown event cannot be forced");
        foreach (var evento in Enum.GetValues<E>().Where(e => e != E.None))
        {
            Check(schedule.StartForced(evento) == evento, "manual events bypass waiting");
            Check(schedule.IsForced && schedule.Age == 0, "manual event starts fresh");
            Check(schedule.StartForced(E.Blackout) == E.None, "an active event must be closed first");
            schedule.Tick(.5f, false, true);
            Check(schedule.Active == evento && schedule.Age == .5f, "manual events follow normal presentation timing");
            schedule.Finish();
            Check(schedule.Active == E.None && !schedule.IsForced, "manual event ends normally");
        }
        schedule.StartForced(E.WindowKnock);
        schedule.Reset();
        Check(schedule.Active == E.None && !schedule.IsForced, "new session clears manual events");
    }
}
