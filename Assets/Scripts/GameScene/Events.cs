using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*              Pegados Errados
 *     Golpes      3       7
 *     Hechizos    6       0  > 600ms prom entre carga y golpe
 *     Posiciones  1
 * 
 */


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

    public void PrintEvents()
    {
        foreach (var e in m_events)
        {
            Debug.Log(e.eventType + " " + e.time);
        }
    }

    public void ResetEvents()
    {
        m_events.Clear();
    }
}
