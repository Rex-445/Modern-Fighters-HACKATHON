using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneController : MonoBehaviour
{
    public CutSceneManager cutSceneManager;

    public static CutSceneController instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void SkipCutScene()
    {
        if (cutSceneManager != null)
        {
            cutSceneManager.StopAllCoroutines();
            cutSceneManager.EndCutScene();
        }
    }
}
