using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public List<GameObject> spawns;
    public static SpawnManager instance;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            print("Another instance of 'SpawnManager' is running on the gameObject '" + this.name + "'.");
        }
    }

    public void SpawnObject(int spawnType, Vector3 point, float duration = 2)
    {
        GameObject go = Instantiate(spawns[spawnType], point, spawns[spawnType].transform.rotation);
        Destroy(go, duration);
    }
}
