using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Event
{
    public EventType eventType;
    public float time;

    public Event(EventType p_eventType, float p_time)
    {
        eventType = p_eventType;
        time = p_time;
    }
}

public enum EventType
{
    CreatureCreated,
    AttackMissed,
    AttackHit,
    SpellLoaded,
    SpellMissed,
    SpellHit,
    UsePotion,
}

public class Events : MonoBehaviour
{
    private List<Event> m_events = new List<Event>();

    public void AddEvent(EventType p_eventType, float p_time)
    {
        m_events.Add(new Event(p_eventType, p_time));
    }

    public string ResumeEvents()
    {
        var l_types = new Dictionary<EventType, int>();
        for (int t = 1; t <= 6; t++)
        {
            for (int i = 0; i < m_events.Count; i++)
            {
                if (m_events[i].eventType == (EventType)t)
                {
                    if (l_types.ContainsKey((EventType)t))
                    {
                        l_types[(EventType)t]++;
                    }
                    else
                    {
                        l_types.Add((EventType)t, 1);
                    }
                }
            }
        }

        var result = "";

        for (int t = 1; t <= 6; t++)
        {
            if (l_types.ContainsKey((EventType)t))
            {
                result += ((EventType)t).ToString() + ": " + l_types[(EventType)t] + "\n";
            }
            else
            {
                result += ((EventType)t).ToString() + ": 0" + "\n";
            }
        }

        Debug.Log(result);

        return result;
    }

    public void ResetEvents()
    {
        m_events.Clear();
    }
}
