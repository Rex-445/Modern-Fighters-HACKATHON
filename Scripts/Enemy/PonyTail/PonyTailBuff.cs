using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PonyTailBuff : MonoBehaviour
{
    public PonyTailAI AI;


    private void Start()
    {
        if (AI == null)
        {
            AI = transform.parent.parent.GetComponent<PonyTailAI>();
        }
    }


    public void CallBackups()
    {
        foreach (GarrageDoorHandler gdH in AI.garrageBackups)
        {
            //Only Spawn New Units when you have none left
            if (!gdH.activeUnits)
                gdH.InitiateBackups();
        }
    }
}
