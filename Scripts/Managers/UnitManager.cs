using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public int enemyLimit = 2;
    public Dictionary<string, int> enemyLimitDifficulty;
    public List<GameObject> enemiesAttackingPlayer;

    public List<GameObject> enemyList;
    public List<Transform> allUnits;


    public UnitIndicator unitIndicator;


    public Unit player;
    public Unit boss;

    public static UnitManager instance;
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        enemyLimitDifficulty = new Dictionary<string, int>();

        enemyLimitDifficulty.Add("Easy", 1);
        enemyLimitDifficulty.Add("Normal", 2);
        enemyLimitDifficulty.Add("Hard", 3);
        enemyLimitDifficulty.Add("Rough", 4);

        DataManager dataManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<DataManager>();
        enemyLimit = enemyLimitDifficulty[dataManager.difficulty];
    }


    private void Update()
    {
        foreach(GameObject go in enemiesAttackingPlayer)
        {
            if (go == null)
            {
                enemiesAttackingPlayer.Remove(go);
                return;
            }
            go.GetComponent<EnemyAI>().canAttackPlayer = true;
        }

        for (int i=0; i < enemyList.Count; i++)
        {
            if (enemyList[i] == null)
            {
                enemyList.RemoveAt(i);
            }
        }
    }

    public void EnemyAgroRequest(GameObject enemy)
    {
        if (enemiesAttackingPlayer.Contains(enemy))
            return;

        if (enemy.GetComponent<EnemyAI>() == false)
            return;

        //Check if there is space in the list
        if (enemiesAttackingPlayer.Count < enemyLimit)
        {
            enemiesAttackingPlayer.Add(enemy);
            enemiesAttackingPlayer[enemiesAttackingPlayer.Count - 1].GetComponent<EnemyAI>().canAttackPlayer = true;
        }

        //If not replace an already added enemy with a new one (that requested for an agro)
        else
        {
            if (enemiesAttackingPlayer.Count == 0)
                return;

            int rand = Random.Range(0, enemiesAttackingPlayer.Count);

            //Stop the old enemy from attacking
            if (enemiesAttackingPlayer.Count == 1)
            {
                rand = 0;
            }
            try
            {
                enemiesAttackingPlayer[rand].GetComponent<EnemyAI>().canAttackPlayer = false;
                enemiesAttackingPlayer[rand] = null;
                enemiesAttackingPlayer[rand] = enemy;
            }
            catch
            {
                enemiesAttackingPlayer[0].GetComponent<EnemyAI>().canAttackPlayer = false;
                enemiesAttackingPlayer.RemoveAt(0);
                enemiesAttackingPlayer[0] = enemy;
            }
        }
    }

    public void RemoveAgro(GameObject enemy)
    {
        if (enemiesAttackingPlayer.Contains(enemy))
        {
            enemiesAttackingPlayer.Remove(enemy);
        }
    }
    public void AddUnit(Unit unit)
    {
        if (unit.isPlayer == false)
        {
            //Add to the list of available Units
            allUnits.Add(unit.transform);

            if (unit.isNone)
                return;

            enemyList.Add(unit.gameObject);
            if (unit.isBoss)
            {
                boss = unit;
            }
        }

        else
        {
            if (player == null)
            {
                player = unit;
                //Add to the list of available Units
                allUnits.Add(player.transform);
            }
        }

        allUnits[allUnits.Count - 1].GetComponent<Unit>().SpawnUnitIndicator(unitIndicator.gameObject);
    }


    public void RemoveUnit(Unit unit)
    {
        allUnits.Remove(unit.transform);
        if (unit.isPlayer == false)
        {
            DataManager.instance.SetEnemy();
            RemoveAgro(unit.gameObject);

            foreach (GameObject go in enemiesAttackingPlayer)
            {
                if (go == unit.gameObject)
                {
                    enemiesAttackingPlayer.Remove(go);
                    break;
                }
            }

            if (unit.isBoss)
            {
                Invoke("RemoveBoss", 1.5f);
            }
            enemyList.Remove(unit.gameObject);

            //Get another enemy Agro'd
            for (int i = 0; i < enemyList.Count; i++)
            {
                try
                {
                    if (enemyList[i].GetComponent<EnemyAI>().canAttackPlayer == false)
                    {
                        EnemyAgroRequest(enemyList[i]);
                        break;
                    }
                }
                catch
                {
                    Unit enemyUnit = enemyList[i].GetComponent<Unit>();
                    //Check if this is PonyTail
                    if (enemyUnit.unitName == "PonyTail" || enemyUnit.unitName == "Electris")
                    {
                        continue;
                    }
                }
            }

            Destroy(unit.gameObject, unit.deathTime);
        }
    }

    void RemoveBoss()
    {
        boss = null;
    }
}
