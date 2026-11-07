using System;
using System.Collections.Generic;

// Session-only pacing. Independent of physics, rules, Unity random and saved progress.
public sealed class SinucaPerceptionSchedule
{
    public enum Event { None, UnderTable, FalseBall, WaterInPipes, PartyEcho }
    private readonly Random random;
    private readonly List<Event> bag = new List<Event>();
    private Event previous;
    private int turns, earliestTurn;
    private float remaining;
    public Event Active { get; private set; }
    public float Age { get; private set; }
    public float Remaining => remaining;

    public SinucaPerceptionSchedule(int seed) { random = new Random(seed); Reset(); }
    public void Reset()
    {
        bag.Clear(); previous = Active = Event.None; Age = 0; turns = 0;
        remaining = Range(40, 85); earliestTurn = random.Next(3, 6);
    }
    public void TurnStarted() { turns++; }
    public Event Tick(float seconds, bool eligible, bool presentationBusy)
    {
        if (seconds <= 0 || float.IsNaN(seconds) || float.IsInfinity(seconds)) return Event.None;
        if (Active != Event.None) { Age += seconds; return Event.None; }
        // Speech and inspection have their own tail; silence starts after they finish.
        if (presentationBusy) return Event.None;
        remaining = Math.Max(0, remaining - seconds);
        if (!eligible || remaining > 0 || turns < earliestTurn) return Event.None;
        if (bag.Count == 0)
        {
            bag.Add(Event.UnderTable); bag.Add(Event.FalseBall);
            bag.Add(Event.WaterInPipes); bag.Add(Event.PartyEcho);
            for (int i = bag.Count - 1; i > 0; i--)
            { int j = random.Next(i + 1); var value = bag[i]; bag[i] = bag[j]; bag[j] = value; }
            if (bag[bag.Count - 1] == previous)
            { var value = bag[0]; bag[0] = bag[bag.Count - 1]; bag[bag.Count - 1] = value; }
        }
        Active = bag[bag.Count - 1]; bag.RemoveAt(bag.Count - 1); Age = 0;
        return Active;
    }
    public void Finish()
    {
        if (Active == Event.None) return;
        previous = Active; Active = Event.None; Age = 0;
        remaining = Range(55, 115); earliestTurn = turns + random.Next(1, 4);
    }
    private float Range(float min, float max) => min + (float)random.NextDouble() * (max - min);
}
