using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    [TextArea(3, 7)]public string[] words;
    public float time = 10f;
    internal float maxTime;
    public bool useTime = false;

    internal GameObject speechBubble;
    public Transform speechPoint;

    int index;
    public Text text;
    public Sprite face;
    GameObject player;
    public UnityEvent OnStart;
    public UnityEvent OnEndDialogue;

    bool active;
    public bool playOnAwake = false;
    public bool ender = false;
    public bool enableUI = true;
    Animator dialogueAnim;
    private Transform targetPoint;

    public Transform localHero;
    public GameObject cameraTarget;

    internal AudioSource voice;

    public AudioSource next;
    public bool toCutScene;


    bool animating = false;
    string originalText;
    private string newText;

    private void Awake()
    {
        dialogueAnim = GameObject.FindGameObjectWithTag("DialogueManager").GetComponent<Animator>();
        maxTime = time;
        try
        {
            voice = GetComponent<AudioSource>();
        }
        catch
        {

        }

        try
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).name.ToLower().Contains("speechbubble"))
                {
                    speechBubble = transform.GetChild(i).gameObject;
                    speechBubble.SetActive(false);
                }
            }
        }
        catch { }

    }

    private void Update()
    {
        if (active)
        {
            //This is an automated dialogue system
            if (useTime)
            {
                //Stall for a moment (depending on Time)
                time -= Time.deltaTime;
                if (time <= 0)
                {
                    //Reset
                    Next();
                    time = maxTime;
                }
            }

            //This is a user controlled dialogue system
            else
            {
                if (Input.GetButtonDown("Click") && TimeManager.paused == false)
                {
                    Next();
                }
            }
        }



        if (playOnAwake && !active)
            Begin();
    }

    public void Next()
    {

        //If this is already playing then skip to the next
        if (animating)
        {
            StopAllCoroutines();
            animating = false;
            dialogueAnim.transform.Find("Variables").Find("NextArrow").gameObject.SetActive(true);
            text.text = newText;
            index++;

            return;
        }

        if (index < words.Length)
            StartCoroutine(AnimateText());

        //Ended
        else
        {
            active = false;
            if (player != null)
            {
                player.GetComponent<UnitController>().Control(true);
                Camera.main.transform.parent.GetComponent<CameraMovement>().target = player.transform;
            }

            OnEndDialogue.Invoke();
            if (ender)
            {
                text.text = "";

                //If this Dialogue is not going to a cutscene then this is a basic dialogue
                // And it will require a UI animation reset
                if (!toCutScene)
                    GameObject.FindGameObjectWithTag("DialogueManager").GetComponent<Animator>().Play("Out");

                if (enableUI)
                    UIManager.instance.EnableUI();
            }

            //Check if this is attacked to a DialogueEndManager
            try
            {
                if (!toCutScene)
                {
                    transform.parent.GetComponent<DialogueEndManager>().Next();
                }
            }
            catch { }

            if (speechBubble)
                Destroy(speechBubble.gameObject);

            Destroy(this.gameObject);
        }
    }

    IEnumerator AnimateText()
    {
        next.Play();
        dialogueAnim.transform.Find("Variables").Find("NextArrow").gameObject.SetActive(false);
        animating = true;
        originalText = words[index];
        char[] characters = words[index].ToCharArray();
        int skipAmount = 5;
        newText = "";

        if (originalText.StartsWith("Hero:"))
        {
            targetPoint = player.transform;
            text.text = player.GetComponent<Unit>().unitName.ToUpper() + ": ";
            newText = player.GetComponent<Unit>().unitName.ToUpper() + ": ";
            face = player.GetComponent<Unit>().image;
        }

        if (!originalText.StartsWith("Hero:") && !originalText.StartsWith("Char:"))
        {
            text.text = "";
            skipAmount = 0;
            targetPoint = transform;
        }


        //Update New Text
        for (int i = skipAmount; i < characters.Length; i++)
        {
            newText += characters[i];
        }

        if (face != null)
            dialogueAnim.transform.Find("Variables").Find("Face").GetComponent<Image>().sprite = face;


        //IF the Targetpoint is Not Null meaning this is a Player Cam
        if (targetPoint != null)
            Camera.main.transform.parent.GetComponent<CameraMovement>().target = targetPoint;

        //Prioritize the Camera Target even if the targetPoint is Not Null
        if (cameraTarget != null)
            Camera.main.transform.parent.GetComponent<CameraMovement>().target = cameraTarget.transform;

        for (int i = skipAmount; i < characters.Length; i++)
        {
            text.text += characters[i];

            if (voice != null && voice.isPlaying == false)
                voice.Play();

            if (characters[i] == '.')
            {
                yield return new WaitForSeconds(.05f);
            }
            else
            {
                yield return null;
            }
        }



        index++;
        animating = false;

        if (!useTime)
            dialogueAnim.transform.Find("Variables").Find("NextArrow").gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Unit"))
        {
            if (collision.GetComponent<Unit>().isPlayer)
            {
                Begin();
            }
        }

        if (collision.CompareTag("Activator"))
            Begin();
    }

    public void Begin()
    {
        if (!Application.isPlaying && active == false)
            return;

        foreach (Transform unit in UnitManager.instance.allUnits)
        {
            try
            {
                foreach (Skill sk in unit.GetComponent<Unit>().skills)
                {
                    sk.DeactivateAbility();
                }
            }
            catch { }
        }

        //Get Player
        if (UnitManager.instance.player != null)
        {
            OnStart.Invoke();
            active = true;
            player = UnitManager.instance.player.transform.gameObject;
            if (!toCutScene)
                dialogueAnim.Play("In");

            if (localHero != null)
            {
                player.transform.position = localHero.transform.position;
                player.GetComponent<Unit>().direction = (int)localHero.transform.localScale.x;
            }

            //Speech Bubble
            if (speechBubble)
                speechBubble.SetActive(true);

            //Disable Player Controls
            player.GetComponent<UnitController>().Control(false);
            player.transform.Find("Container").Find("Sprite").GetComponent<Animator>().Play("Idle");
            player.GetComponent<Unit>().horizontal = 0;
            player.GetComponent<Unit>().vertical = 0;

            //Disable player super skill
            if (player.GetComponent<Unit>().inSkill)
            {
                foreach(Skill skill in player.GetComponent<Unit>().skills)
                {
                    skill.DeactivateAbility();
                }
            }

            //Update The Camera target
            if (cameraTarget != null)
            {
                Camera.main.transform.parent.GetComponent<CameraMovement>().target = cameraTarget.transform;
            }

            //Take care of other UI
            UIManager.instance.DisableUI();


            if (speechBubble == null)
            {
                Next();
                return;
            }

            //Speech Point
            if (speechPoint != null)
            {
                speechBubble.transform.parent = speechPoint.transform;
                speechBubble.transform.localPosition = Vector3.zero;
                speechBubble.transform.position += new Vector3(-.3f, .1f, 0);
            }

            else if (words[index].StartsWith("Hero:"))
            {
                //Speech Point
                speechPoint = UnitManager.instance.player.transform.Find("HealthPoint").transform;
                speechBubble.transform.parent = speechPoint.transform;
                speechBubble.transform.localPosition = Vector3.zero;
                speechBubble.transform.position += new Vector3(-.3f, .1f, 0);

            }


            Next();
        }
    }
}
