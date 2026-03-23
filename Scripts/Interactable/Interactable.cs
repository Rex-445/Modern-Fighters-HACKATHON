using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent OnTrigger;
    public UnityEvent OnExit;

    public bool isDestroy;


    public void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Unit")
        {
            //Case of Hero
            if (collision.GetComponent<Unit>().isPlayer)
            {
                Trigger();
                if (isDestroy)
                    Destroy(this.gameObject);
            }
        }
    }

    public void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Unit"))
        {
            //Case of Hero
            if (collision.GetComponent<Unit>().isPlayer)
            {
                OnExit.Invoke();
            }
        }
    }

    public void Trigger()
    {
        OnTrigger.Invoke();
    }
}
