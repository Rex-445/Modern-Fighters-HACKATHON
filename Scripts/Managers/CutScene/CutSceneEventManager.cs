using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CutSceneEventManager : MonoBehaviour
{
    public static CutSceneEventManager instance;

    public List<CutSceneEvent> events;
    public bool playOnAwake;


    public UnityEvent OnEnd;
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        ArrangeEvents();
        if (playOnAwake && events.Count > 0)
        {
            Invoke("Next", .01f);
        }
    }

    private void ArrangeEvents()
    {
        //Rearrange the events in order
        for (int i = 0; i < transform.childCount; i++)
        {
            events.Add(transform.GetChild(i).GetComponent<CutSceneEvent>());
        }
    }

    public void Next()
    {
        //Check if this is the last one
        if (events.Count == 0)
        {
            OnEnd.Invoke();
            return;
        }


        events[0].Begin();
        events.Remove(events[0]);
    }
}
