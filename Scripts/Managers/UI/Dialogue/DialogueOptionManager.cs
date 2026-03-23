using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueOptionManager : MonoBehaviour
{
    //Make an instance
    public static DialogueOptionManager instance;


    //Options
    [Header("Dialogue Option Variables")]
    public GameObject optionA;
    public GameObject optionB;
    public GameObject optionC;

    public Image iconA;
    public Image iconB;
    public Image iconC;

    public GameObject dialogueOptions;

    internal DialogueOptionTrigger targetTrigger;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        dialogueOptions.SetActive(false);
    }

    public void SetTrigger(DialogueOptionTrigger trigger)
    {
        targetTrigger = trigger;
        StartDialogue();
    }

    public void StartDialogue()
    {
        dialogueOptions.SetActive(true);
        UIManager.instance.DisableUI();



        //Deactivate and Actiave the Titles accordingly
        optionA.SetActive(targetTrigger.dialogueTitles.Length > 0);
        optionB.SetActive(targetTrigger.dialogueTitles.Length > 1);
        optionC.SetActive(targetTrigger.dialogueTitles.Length > 2);

        {
            //First Update the Values
            //Option A
            optionA.transform.Find("Button").Find("Title").GetComponent<TextMeshProUGUI>().text = targetTrigger.dialogueTitles[0].title;
            iconA.sprite = targetTrigger.dialogueTitles[0].icon;

            //Option B
            optionB.transform.Find("Button").Find("Title").GetComponent<TextMeshProUGUI>().text = targetTrigger.dialogueTitles[1].title;
            iconB.sprite = targetTrigger.dialogueTitles[1].icon;

            try
            {
                //Option C
                optionC.transform.Find("Button").Find("Title").GetComponent<TextMeshProUGUI>().text = targetTrigger.dialogueTitles[2].title;
                iconC.sprite = targetTrigger.dialogueTitles[2].icon;
            }
            catch { }
        }
    }

    public void EndDialogue()
    {
        dialogueOptions.SetActive(false);
    }

    public void OptionA()
    {
        targetTrigger.OptionA();
        EndDialogue();
    }

    public void OptionB()
    {
        targetTrigger.OptionB();
        EndDialogue();
    }

    public void OptionC()
    {
        targetTrigger.OptionC();
        EndDialogue();
    }
}
