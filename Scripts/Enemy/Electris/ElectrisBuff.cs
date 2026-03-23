using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ElectrisBuff : MonoBehaviour
{
    public Material buffMat;
    public Material defaultMat;

    public float damageBuff;
    public float buffTime;
    float timer;

    public GameObject electricChargeBuff;
    public float level = .5f;

    public PlaySound levelUpSound;


    //Lightning Spawns
    public GameObject lightningRail;

    private void Update()
    {
        timer -= Time.deltaTime * 2;
        if (timer <= 0)
        {
            Debuff();
        }

        electricChargeBuff.transform.localScale = new Vector3(level, level, level);
    }

    public void LevelUp()
    {
        level += .15f;
        if (level > 1)
            level = 1;

        levelUpSound.PlayRandomSound();
    }
    public void Buff()
    {
        transform.GetComponent<SpriteRenderer>().material = buffMat;
        timer = buffTime;

        //Buff Damage
        transform.Find("Hitbox").GetComponent<Hitbox>().damage += damageBuff;
    }

    public void Debuff()
    {
        transform.GetComponent<SpriteRenderer>().material = defaultMat;
    }

    [System.Obsolete]
    public void SpawnLightningRail()
    {
        Transform target = transform.parent.parent.GetComponent<ElectrisAI>().target.transform;
        Vector3 point = new Vector3(Random.RandomRange(target .position.x - 5, target.position.x + 5), 0, Random.RandomRange(-3, 5));
        GameObject go = Instantiate(lightningRail, point, Quaternion.identity);
        go.GetComponent<LightningBolt>().owner = transform.parent.parent.GetComponent<ElectrisAI>();
    }

    [System.Obsolete]
    public void SpawnSeekingLightningRail()
    {
        Vector3 point = new Vector3(Random.RandomRange(-11, 7), 0, Random.RandomRange(-5, 5));
        GameObject go = Instantiate(lightningRail, point, Quaternion.identity);
        go.GetComponent<LightningBolt>().owner = transform.parent.parent.GetComponent<ElectrisAI>();
        go.GetComponent<LightningBolt>().stormType = StormType.seeking;
    }
}
