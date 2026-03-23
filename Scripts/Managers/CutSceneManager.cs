using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class CutSceneManager : MonoBehaviour
{
    public Transform playerStartPoint;

    public UnityEvent OnStartCutScene;
    public UnityEvent OnEndCutScene;

    public bool AutoUpdatePos = true;

    public bool playOnAwake = false;

    public bool canSkip = true;

    public float stallTime;

    float moveValue;

    private PlayableDirector director;


    internal CanvasGroup skipGroup;
    internal bool isPlaying;

    private void Start()
    {
        if (playOnAwake)
        {
            Invoke("StartCutScene", .001f);
        }

        skipGroup = UIManager.instance.transform.Find("CutScene").Find("Skip").GetComponent<CanvasGroup>();
    }

    private void Update()
    {/*
        if (moveValue == 2)
        {
            UnitManager.instance.player.horizontal = 1;
            UnitManager.instance.player.moveSpeed = UnitManager.instance.player.maxMoveSpeed;
            UnitManager.instance.player.FastRun();
        }

        if (moveValue <= 0)
        {
            UnitManager.instance.player.horizontal = 0;
            UnitManager.instance.player.moveSpeed -= 3;
            moveValue = 0;
        }*/

        if (isPlaying)
        {
            if (!canSkip)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                StopCoroutine(OpenSkipButton());
                StartCoroutine(OpenSkipButton());
            }
        }
    }

    private IEnumerator OpenSkipButton()
    {
        skipGroup.alpha = 0;

        //Fade to Opaque
        while (skipGroup.alpha < 1)
        {
            skipGroup.alpha += .1f;
            
            yield return null;
        }

        //Stall for a bit
        yield return new WaitForSeconds(5);


        //Fade back to clear
        while (skipGroup.alpha > 0)
        {
            skipGroup.alpha -= .1f;

            yield return null;
        }

    }

    public void StartCutScene()
    {
        print("Entered Cut Scene");
        skipGroup.interactable = true;

        foreach(Transform unit in UnitManager.instance.allUnits)
        {
            foreach(Skill sk in unit.GetComponent<Unit>().skills)
            {
                sk.DeactivateAbility();
            }
        }

        //Generic
        CutSceneController.instance.cutSceneManager = this;
        UIManager.instance.DisableUI();

        if (UnitManager.instance.player != null)
            UnitManager.instance.player.transform.position = playerStartPoint.position;

        //UnitManager
        {
            UnitManager.instance.player.GetComponent<UnitController>().Control(false);
            UnitManager.instance.player.GetComponent<Unit>().state = Unit.MovementState.idle;
            UnitManager.instance.player.transform.Find("Container").Find("Sprite").GetComponent<Animator>().Play("Idle");
            UnitManager.instance.player.GetComponent<Unit>().horizontal = 0;
            UIManager.instance.transform.Find("CutScene").gameObject.SetActive(true);
            UnitManager.instance.player.gameObject.SetActive(false);
        }

        isPlaying = true;
        OnStartCutScene.Invoke();


        if (stallTime != 0)
            StartCoroutine(StalledCutScene());

    }


    public void SetPlayerPosition()
    {
        if (playerStartPoint != null)
            UnitManager.instance.player.transform.position = playerStartPoint.position;
    }

    public void EndCutScene()
    {
        OnEndCutScene.Invoke();
        isPlaying = false;

        //Reset the Controller
        if (CutSceneController.instance.cutSceneManager == this)
            CutSceneController.instance.cutSceneManager = null;

        //Skip Button
        skipGroup.alpha = 0;
        skipGroup.interactable = false;


        print("Ended Cut Scene");

        //Camera Focus
        Camera.main.transform.parent.GetComponent<CameraMovement>().target = UnitManager.instance.player.transform;

        //Reanable UI (If Necessary)
        UIManager.instance.EnableUI();

        //Give the player access to control again
        UnitManager.instance.player.GetComponent<UnitController>().Control(true);
        GameObject.FindGameObjectWithTag("DialogueManager").GetComponent<Animator>().Play("Out");
        UIManager.instance.transform.Find("CutScene").gameObject.SetActive(false);
        UnitManager.instance.player.gameObject.SetActive(true);
    }


    public IEnumerator StalledCutScene()
    {
        yield return new WaitForSeconds(stallTime);
        EndCutScene();
    }

    public void UpdateHeroAction(int movement)
    {
        try
        {
            UnitManager.instance.player.crouch = false;
            UnitManager.instance.player.attack = false;
            UnitManager.instance.player.horizontal = movement;
        }
        catch
        {

        }
    }

    public void PlayHeroAnimation(string animName)
    {
        try
        {
            UnitManager.instance.player.anim.Play(animName);
        }
        catch
        {

        }
    }

    public void UpdateHeroRun(int run)
    {
        try
        {
            moveValue = run;
        }
        catch
        {

        }
    }


    public void UpdateHeroDirection(int direction)
    {
        try
        {
            UnitManager.instance.player.direction = direction;
        }
        catch
        {

        }
    }
}
