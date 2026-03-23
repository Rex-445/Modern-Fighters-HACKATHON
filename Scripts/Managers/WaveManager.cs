using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{

    public GameObject[] respawn;

    public bool randomRespawn = false;

    public int respawnCount = 0;
    public int respawnID = 0;

    public List<GameObject> enemies;

    public UnityEvent OnWaveCleared;

    [Tooltip("This checks if an enemy has been beaten, a cooldown is included")]
    public UnityEvent OnEnemyDowned;
    [SerializeField] float timer = 3;
    float maxTime;

    public bool cleared;

    Dictionary<string, int> difficulty = new Dictionary<string, int>();

    private void Start()
    {
        maxTime = timer;
    }

    private void LateUpdate()
    {
        timer -= Time.deltaTime;
    }

    internal void SetEnemiesLevel()
    {
        //Set Difficulty
        DataManager dm = DataManager.instance;

        //Easy
        int easy = (int)Mathf.Round(dm.dataPersistence.gameData.unitLevel * .5f);
        if (easy < 1) easy = 1;

        //Normal
        int normal = (int)Mathf.Round(dm.dataPersistence.gameData.unitLevel * .8f);
        if (normal < 1) normal = 1;

        //Hard
        int hard = (int)Mathf.Round(dm.dataPersistence.gameData.unitLevel + (dm.dataPersistence.gameData.unitLevel * .15f));

        //Hard
        int rough = (int)Mathf.Round(dm.dataPersistence.gameData.unitLevel + (dm.dataPersistence.gameData.unitLevel * .4f));



        difficulty.Add("Easy", easy);
        difficulty.Add("Normal", normal);
        difficulty.Add("Hard", hard);
        difficulty.Add("Rough", rough);


        if (enemies.Count == 0)
        {
            //Set all enemies
            for (int i = 0; i < transform.childCount; i++)
            {
                enemies.Add(transform.GetChild(i).gameObject);
            }
        }


        //Enemy List
        foreach (GameObject enemy in enemies)
        {
            if (!enemy.GetComponent<Unit>().isBoss)
                enemy.GetComponent<Unit>().unitLevel = difficulty[dm.dataPersistence.gameData.difficulty];
            else
            {
                enemy.GetComponent<Unit>().unitLevel = difficulty["Normal"];
            }
        }
    }

    private void Update()
    {
        for (int i=0; i < enemies.Count; i++)
        {
            if (enemies[i] == null)
            {
                enemies.RemoveAt(i);
                //SFX
                if (timer < 0)
                {
                    timer = maxTime;
                    OnEnemyDowned.Invoke();
                }

                Respawn();
            }
        }

        Check();
    }

    void Check()
    {
        if (enemies.Count == 0 && !cleared)
        {
            cleared = true;
            OnWaveCleared.Invoke();
            try
            {
                transform.parent.GetComponent<EnemyWaveManager>().WaveCleared(this);
            }
            catch { }
            Destroy(this.gameObject, 1);
        }
    }


    void Respawn()
    {
        DataManager dm = DataManager.instance;

        if (respawnCount > 0)
        {
            respawnCount--;
            int rand = 0;
            if (randomRespawn)
            {
                rand = Random.Range(0, respawn.Length - 1);
            }
            else
            {
                rand = respawnID;
                respawnID++;
                if (respawnID >= respawn.Length)
                    respawnID = 0;
            }

            GameObject go = Instantiate(respawn[rand], transform.position, Quaternion.identity) as GameObject;
            go.transform.parent = this.transform;
            go.GetComponent<Unit>().unitLevel = difficulty[dm.dataPersistence.gameData.difficulty];
            enemies.Add(go.gameObject);
        }
    }
}
