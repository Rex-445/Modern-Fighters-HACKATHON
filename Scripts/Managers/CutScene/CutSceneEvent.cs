using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class CutSceneEvent : MonoBehaviour
{
    public bool useTime;
    public float timer;
    float maxTime;

    public List<SceneEvent> sceneEvents;
    public int sceneIndex;

    bool inPlay;

    public UnityEvent OnBegin;
    public UnityEvent OnEnd;

    // Start is called before the first frame update
    void Start()
    {
        maxTime = timer;

        ArrangeEvents();
    }

    private void ArrangeEvents()
    {
        //Rearrange the events in order
        for (int i = 0; i < transform.childCount; i++)
        {
            sceneEvents.Add(transform.GetChild(i).GetComponent<SceneEvent>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inPlay)
        {
            if (!useTime)
                return;

            timer -= Time.deltaTime;
            if (timer < 0)
            {
                Next();
                timer = maxTime;
            }
        }
    }

    public void Next()
    {
        sceneIndex++;
        //End Recent Event
        sceneEvents[sceneIndex - 1].EndEvent();


        if (sceneIndex > sceneEvents.Count - 1)
        {
            inPlay = false;
            transform.parent.GetComponent<CutSceneEventManager>().Next();
            OnEnd.Invoke();
            Destroy(this.gameObject);
            return;
        }

        //Start New Event
        maxTime = sceneEvents[sceneIndex].time;
        sceneEvents[sceneIndex].StartEvent();
    }

    public void Begin()
    {
        if (!inPlay)
        {
            inPlay = true;
            timer = sceneEvents[0].time;
            sceneEvents[0].StartEvent();
            OnBegin.Invoke();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Unit")
        {
            Begin();
        }
    }
}
