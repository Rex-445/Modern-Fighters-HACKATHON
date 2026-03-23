using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpwaner : MonoBehaviour
{
    public float frequency;
    float timer;

    [SerializeField] private float destroyTime = 3f;

    public GameObject[] objects;

    public Vector3 distanceFromPoint;


    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            Spawn();
            timer = frequency;
        }
    }

    public void Spawn()
    {
        int id = Random.Range(-1, objects.Length);

        Vector3 point = new Vector3(transform.position.x + (Random.Range(-distanceFromPoint.x, distanceFromPoint.x)), transform.position.y, transform.position.z + Random.Range(-distanceFromPoint.z, distanceFromPoint.z));

        try
        {
            GameObject go = Instantiate(objects[id], point, Quaternion.identity);
            go.GetComponent<ObjectMovement>().direction = (int)transform.localScale.x;
            Destroy(go, destroyTime);
        }
        catch { }
    }
}
