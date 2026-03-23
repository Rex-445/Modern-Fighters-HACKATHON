using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyWaveManager : MonoBehaviour
{

    public UnityEvent OnWaveCleared;

    public List<WaveManager> waveManager;


    private void Start()
    {
        for (int i=0; i < transform.childCount; i ++)
        {
            waveManager.Add(transform.GetChild(i).GetComponent<WaveManager>());
            waveManager[i].SetEnemiesLevel();
        }

        waveManager[0].gameObject.SetActive(true);
    }

    public void WaveCleared(WaveManager wave)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            waveManager.Remove(wave);

            if (waveManager.Count > 0)
                waveManager[0].gameObject.SetActive(true);
        }

        if (waveManager.Count == 0)
        {
            OnWaveCleared.Invoke();
        }
    }
}
