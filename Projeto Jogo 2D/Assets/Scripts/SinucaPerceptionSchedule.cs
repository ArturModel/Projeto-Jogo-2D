using System;
using System.Collections.Generic;

// Session-only pacing. Independent of physics, rules, Unity random and saved progress.
// The default event value represents no active event.
public class MinigamePerceptionSchedule<T> where T : struct, Enum
{
    private readonly Random random;
    private readonly T[] events;
    private readonly List<T> bag = new List<T>();
    private T previous;
    internal bool IsForced { get; private set; }
    private int turns, earliestTurn;
    private float remaining;
    public T Active { get; private set; }
    public float Age { get; private set; }
    public float Remaining => remaining;

    public MinigamePerceptionSchedule(int seed, params T[] events)
    {
        if (events == null || events.Length < 2)
            throw new ArgumentException("At least two perceptions are required.", nameof(events));

        this.events = (T[])events.Clone();
        random = new Random(seed);
        Reset();
    }

    public void Reset()
    {
        bag.Clear();
        previous = Active = default;
        IsForced = false;
        Age = 0;
        turns = 0;
        remaining = Range(40, 85);
        earliestTurn = random.Next(3, 6);
    }

    public void TurnStarted() { turns++; }

    internal T StartForced(T next)
    {
        if (!Same(Active, default) || Same(next, default) || Array.IndexOf(events, next) < 0)
            return default;
        Active = next;
        IsForced = true;
        Age = 0;
        return Active;
    }

    public T Tick(float seconds, bool eligible, bool presentationBusy)
    {
        if (seconds <= 0 || float.IsNaN(seconds) || float.IsInfinity(seconds)) return default;
        if (!Same(Active, default)) { Age += seconds; return default; }
        // Speech and inspection have their own tail; silence starts after they finish.
        if (presentationBusy) return default;

        remaining = Math.Max(0, remaining - seconds);
        if (!eligible || remaining > 0 || turns < earliestTurn) return default;
        if (bag.Count == 0)
        {
            bag.AddRange(events);
            for (int i = bag.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (bag[i], bag[j]) = (bag[j], bag[i]);
            }
            if (Same(bag[bag.Count - 1], previous))
                (bag[0], bag[bag.Count - 1]) = (bag[bag.Count - 1], bag[0]);
        }

        Active = bag[bag.Count - 1];
        bag.RemoveAt(bag.Count - 1);
        Age = 0;
        return Active;
    }

    public void Finish()
    {
        if (Same(Active, default)) return;
        if (!IsForced) previous = Active;
        IsForced = false;
        Active = default;
        Age = 0;
        remaining = Range(55, 115);
        earliestTurn = turns + random.Next(1, 4);
    }

    private static bool Same(T first, T second) => EqualityComparer<T>.Default.Equals(first, second);
    private float Range(float min, float max) => min + (float)random.NextDouble() * (max - min);
}

public sealed class SinucaPerceptionSchedule : MinigamePerceptionSchedule<SinucaPerceptionSchedule.Event>
{
    public enum Event { None, UnderTable, FalseBall, Blackout, HauntedZe, WindowKnock }

    public SinucaPerceptionSchedule(int seed)
        : base(seed, Event.UnderTable, Event.FalseBall, Event.Blackout, Event.HauntedZe, Event.WindowKnock) { }
}
