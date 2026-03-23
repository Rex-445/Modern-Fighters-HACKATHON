using System.Collections.Generic;
using UnityEngine;

public class GarrageDoorHandler : MonoBehaviour
{
    public Transform spawnLeft;
    public Transform spawnRight;
    public List<Unit> spawnUnits;

    [SerializeField]
    internal bool activeUnits = false;

    [SerializeField]
    internal bool respawn = false;


    private void Update()
    {
        activeUnits = transform.Find("SpawnedUnits").childCount > 0;


        if (!activeUnits && respawn && !GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Open"))
            InitiateBackups();
    }

    [System.Obsolete]
    public void SpawnUnits()
    {
        foreach(Unit unit in spawnUnits)
        {
            Vector3 point = new Vector3(Random.RandomRange(spawnLeft.position.x, spawnRight.position.x), 0f, spawnLeft.position.z);
            Instantiate(unit, point, Quaternion.identity, transform.Find("SpawnedUnits"));
        }
    }


    public void InitiateBackups()
    {
        GetComponent<Animator>().Play("Open", 0, 0);
    }
}
