using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



[Serializable]
public class DialogueOptions
{
    public string title;
    public Sprite icon;
}


public class DialogueOptionTrigger : MonoBehaviour
{
    [Header("UI Variables")]
    public DialogueOptions[] dialogueTitles;



    [Header("On Trigger Events")]
    public UnityEvent OnTrigger;
    public UnityEvent OnEnd;

    [Space(20)]
    public UnityEvent OnClickA;
    public UnityEvent OnClickB;
    public UnityEvent OnClickC;


    internal DialogueOptionManager DOM;


    private void Start()
    {
        DOM = GameObject.FindGameObjectWithTag("DialogueOptionManager").GetComponent<DialogueOptionManager>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Unit")
        {
            if (other.GetComponent<Unit>().isPlayer)
                Trigger();
        }
    }

    public void Trigger()
    {
        //Invoke Start
        OnTrigger.Invoke();

        //Set the Dialogue Option Manager to this
        DOM.SetTrigger(this);

        UnitManager.instance.player.GetComponent<UnitController>().Control(false);
    }


    public void OptionA()
    {
        OnClickA.Invoke();
        OnEnd.Invoke();
    }
    public void OptionB()
    {
        OnClickB.Invoke();
        OnEnd.Invoke();
    }
    public void OptionC()
    {
        OnClickC.Invoke();
        OnEnd.Invoke();
    }
}
