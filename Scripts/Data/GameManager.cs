using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static bool firstGamePlay;


    private Unit player;
    //Player Bounds
    public Transform bound;
    public Transform[] bounds;
    public float boundOffset;
    int index;

    [Header("Player Helper")]
    public GameObject[] barrelList;
    public List<Barrel> barrels;
    public float barrelTime;
    internal float maxBarrelTime;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        else
        {
            Debug.LogWarning("Another Instance of GameManager is running!!");
        }

        //Add all available barrels
        maxBarrelTime = barrelTime;
        Barrel[] barrels2 = GameObject.FindObjectsOfType<Barrel>();
        foreach(Barrel barrel in barrels2)
        {
            barrels.Add(barrel);
        }

        Invoke("StartGame", .1f);

        if (bound == null)
        {
            bound = GameObject.Find("GlassWall_Right").transform;
        }

        RelocateBounds();
    }


    void StartGame()
    {
        DataManager.instance.StartGame();
    }

    private void Update()
    {
        for (int i = 0; i < barrels.Count; i++)
        {
            if (barrels[i] == null)
                barrels.RemoveAt(i);
        }

        UpdateBarrelSpawns();
        UpdateBounds();
    }

    private void UpdateBarrelSpawns()
    {
        float perc = UnitManager.instance.player.health / UnitManager.instance.player.maxHealth;

        if (perc <= 1f && barrels.Count == 0)
        {
            barrelTime -= Time.deltaTime;
            if (barrelTime <= 0)
            {
                barrelTime = maxBarrelTime;
                int rand = Random.Range(0, barrelList.Length);

                Vector3 point = new Vector3(Random.Range(Camera.main.transform.parent.parent.Find("Left").position.x, Camera.main.transform.parent.parent.Find("Right").position.x),
                    5, Random.Range(Camera.main.transform.parent.parent.Find("Left").position.z, Camera.main.transform.parent.parent.Find("Right").position.z));
                GameObject go = Instantiate(barrelList[rand].gameObject, point, Quaternion.identity);

                barrels.Add(go.transform.GetComponentInChildren<Barrel>());
            }
        }
    }

    private void UpdateBounds()
    {
        if (player == null)
            player = UnitManager.instance.player;

        //X Bounds
        if (player.transform.position.x > bound.position.x + boundOffset)
            player.transform.position = new Vector3(bound.position.x + boundOffset, player.transform.position.y, player.transform.position.z);
    }

    public void RelocateBounds()
    {
        bound.position = bounds[index].position;
        index++;
    }
}
