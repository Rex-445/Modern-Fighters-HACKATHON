using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;

    public static bool paused = false;
    public static float time;

    //This deletes any NON-INGAME TIME
    public static float deleteTime;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Time.timeScale = 1;
        }

        else
        {
            Debug.LogWarning("Another Instance of TimeManager is running!!");
        }
        deleteTime = Time.time;
    }

    private void Update()
    {
        time = Time.time;
    }

    public static int GetPlayTime()
    {
        return (int)(time - deleteTime);
    }

    public void Play()
    {
        //Animations
        Animator[] animators = GameObject.FindObjectsOfType<Animator>();
        foreach (Animator anim in animators)
        {
            anim.speed = 1;
        }

        //Physics
        Rigidbody[] rbs = GameObject.FindObjectsOfType<Rigidbody>();
        foreach (Rigidbody rb in rbs)
        {
            rb.constraints = new RigidbodyConstraints();
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    public void PlayObject(Transform reference, Rigidbody rb)
    {
        //Animations
        //reference.Find("Sprite").GetComponent<Animator>().speed = 1;

        //Physics
        //Check if this rb is not the same as the reference object
        rb.constraints = new RigidbodyConstraints();
        reference.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
    }

    //Stops all movement and animation of anything but the specified object reference presented
    public void Stop(Transform reference)
    {
        //Animations
        Animator[] animators = GameObject.FindObjectsOfType<Animator>();
        foreach (Animator anim in animators)
        {
            anim.speed = 0;

            //Avoid Stopping the UI animations
            //Check if this animator is not the same as the reference object
            if (anim.gameObject.layer == 5 || anim == reference.Find("Container").Find("Sprite").GetComponent<Animator>())
            {
                //If you find a UI layer then reset it
                anim.speed = 1;
            }
        }

        //Physics
        Rigidbody[] rbs = GameObject.FindObjectsOfType<Rigidbody>();
        foreach (Rigidbody rb in rbs)
        {
            //Check if this rb is not the same as the reference object
            if (rb != reference.GetComponent<Rigidbody>())
                rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }



    //Stops all movement and animation of the player
    public void StopPlayer()
    {
        //Animations
        Animator[] animators = GameObject.FindObjectsOfType<Animator>();
        foreach (Animator anim in animators)
        {
            anim.speed = 0;
            try
            {
                if (anim.transform.parent.GetComponent<Unit>() != null || anim.transform.CompareTag("MainCamera") == true)
                {
                    if (anim.transform.parent.GetComponent<Unit>().isPlayer == false)
                    {
                        anim.speed = 1;
                    }
                }
            }
            catch
            {
                anim.speed = 1;
                //Debug.LogWarning("This GameObject ,"+ anim.name + ", does not have a parent or it does not have the <Unit> component attached");
            }
        }

        //Physics
        Rigidbody2D[] rbs = GameObject.FindObjectsOfType<Rigidbody2D>();
        foreach (Rigidbody2D rb in rbs)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    public void StepPausePlayer(float value)
    {
        StopPlayer();
        Invoke("Play", value);
    }

    public void StepPause(float value, Transform reference)
    {
        Stop(reference);
        Invoke("Play", value);
    }

    public void RealStepPause(float value)
    {
        StopAllCoroutines();
        StartCoroutine(StallPause(value));
        TimePause();
    }

    IEnumerator StallPause(float value)
    {
        yield return new WaitForSecondsRealtime(value);
        TimePlay();
    }


    public void AnimPause(float duration, Animator reference)
    {
        StartCoroutine(AnimPlay(duration, reference));
    }

    IEnumerator AnimPlay(float duration, Animator reference)
    {
        reference.speed = 0f;
        yield return new WaitForSeconds(duration);
        reference.speed = 1f;
    }


    //In Game Pause
    public void TimePause()
    {
        paused = true;
        Time.timeScale = 0;
        AudioListener.pause = true;
        return;
    }


    //In Game Play
    public void TimePlay()
    {
        paused = false;
        Time.timeScale = 1;
        AudioListener.pause = false;
        return;
    }

    //In Game Slow Mo
    public void SlowMotion(float duration, float value)
    {
        StartCoroutine(SlowMo(duration, value));
    }

    IEnumerator SlowMo(float duration, float value)
    {
        Time.timeScale = value;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1;
    }
}
