using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIHandler : MonoBehaviour
{
    public float size = 2;
    public Vector3 offset;

    private Unit unit;

    public List<GameObject> enemyLeft;
    public List<GameObject> enemyRight;

    internal Vector3 enemyPointLeft;
    internal Vector3 enemyPointRight;

    private void Start()
    {
        unit = GetComponent<Unit>();
    }
    // Update is called once per frame
    void Update()
    {
        enemyPointLeft = unit.transform.position - new Vector3(offset.x, -offset.y, -offset.z);
        enemyPointRight = unit.transform.position + offset;
        RemoveLeftAndRight();
        GetEnemiesOnSides();
    }

    private void RemoveLeftAndRight()
    {
        //Left Side
        Collider[] enemiesLeft = Physics.OverlapSphere(transform.position - new Vector3(offset.x, -offset.y, -offset.z), size);
        for (int i=0; i < enemyLeft.Count; i++)
        {
            bool contained = false;
            foreach(Collider col in enemiesLeft)
            {
                if (col.gameObject == enemyLeft[i])
                {
                    contained = true;
                }
            }

            if (contained == false)
            {
                enemyLeft.RemoveAt(i);
            }
        }

        //Right Side
        Collider[] enemiesRight = Physics.OverlapSphere(transform.position + offset, size);
        for (int i = 0; i < enemyRight.Count; i++)
        {
            bool contained = false;
            foreach (Collider col in enemiesRight)
            {
                if (col.gameObject == enemyRight[i])
                {
                    contained = true;
                }
            }

            if (contained == false)
            {
                enemyRight.RemoveAt(i);
            }
        }
    }

    private void GetEnemiesOnSides()
    {
        //Left Side
        Collider[] enemiesLeft = Physics.OverlapSphere(transform.position - new Vector3(offset.x, -offset.y, -offset.z), size);

        foreach (Collider col in enemiesLeft)
        {
            if (col.tag == "Unit")
            {
                if (col.gameObject != unit.gameObject)
                {
                    if (!enemyLeft.Contains(col.gameObject))
                    {
                        if (enemyLeft.Count < 2)
                            enemyLeft.Add(col.gameObject);
                    }
                }
            }
        }

        //Right Side
        Collider[] enemiesRight = Physics.OverlapSphere(transform.position + offset, size);
        foreach (Collider col in enemiesRight)
        {
            if (col.tag == "Unit")
            {
                if (col.gameObject != unit.gameObject)
                {
                    if (!enemyRight.Contains(col.gameObject) && enemyRight.Count < 2)
                    {
                        enemyRight.Add(col.gameObject);
                    }
                }
            }
        }

    }

    private void OnDrawGizmos()
    {
        //Left Side
        Gizmos.DrawWireSphere(transform.position - new Vector3(offset.x, -offset.y, -offset.z), size);


        //Right Side
        Gizmos.DrawWireSphere(transform.position + offset, size);
    }
}
