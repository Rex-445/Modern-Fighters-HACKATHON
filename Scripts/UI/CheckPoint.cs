using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckPoint : MonoBehaviour
{
    public List<GameObject> progress;
    public List<GameObject> destroyProgress;

    public UnityEvent OnActivateCheckPoint;

    public Transform startPoint;
    public int ID;

    public void ActivateCheckPoint()
    {
        StartCoroutine(StalledActivation());
        Camera.main.transform.parent.transform.position = startPoint.position;
        OnActivateCheckPoint.Invoke();
    }

    private IEnumerator StalledActivation()
    {
        yield return new WaitForSeconds(.1f);
        UnitManager.instance.player.transform.position = startPoint.position;
        DeleteList(progress);
        this.gameObject.SetActive(false);
    }

    internal void DeleteList(List<GameObject> list)
    {
        for (int i=0; i < list.Count; i++)
        {
            list[i].gameObject.SetActive(false);
        }

        for (int i=0; i < destroyProgress.Count; i++)
        {
            Destroy(destroyProgress[i]);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Unit"))
        {
            if (collision.GetComponent<Unit>().isPlayer)
            {
                SaveCheckPoint();
            }
        }
    }

    public void SaveCheckPoint()
    {
        CheckPointManager.instance.id = ID;
        DataManager.instance.dataPersistence.SaveGame();
    }
}
