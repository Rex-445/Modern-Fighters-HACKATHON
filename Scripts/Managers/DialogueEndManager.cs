using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueEndManager : MonoBehaviour
{
    public List<DialogueManager> dialogues;

    public bool playOnAwake = false;

    public UnityEvent OnEnd;

    public bool enableUI;
    public bool ender = true;

    // Start is called before the first frame update
    void Start()
    {
        ArrangeDialogues();
        if (playOnAwake && dialogues.Count > 0)
        {
            Invoke("Next", .01f);
        }
    }

    private void ArrangeDialogues()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            dialogues.Add(transform.GetChild(i).GetComponent<DialogueManager>());
            dialogues[i].ender = false;
            dialogues[i].enableUI = false;
        }

        //Set the last one in the list to be an ender
        if (ender)
        {
            dialogues[dialogues.Count - 1].ender = true;
            dialogues[dialogues.Count - 1].enableUI = enableUI;
        }
    }
    
    public void End()
    {
        OnEnd.Invoke();
    }

    public void Next()
    {
        //Check if this is the last one
        if (dialogues.Count == 0)
        {
            End();
        }


        dialogues[0].Begin();
        dialogues.Remove(dialogues[0]);
    }
}
