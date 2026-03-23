using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public enum StormType
{
    normal,
    seeking
}
public class LightningBolt : MonoBehaviour
{
    //This is the duration of the 
    public float strikeTime;
    bool alive = true;

    //This is the lightning gameobject that does the damage to anything in the area
    public GameObject actualLightning;

    //This is the vfx that is to be displayed in anticpation of the damaging lightning
    public GameObject vfxLightning;

    public ElectrisAI owner;

    public StormType stormType = StormType.normal;

    private void Start()
    {
        //If this is a seeking storm then set its position to the player
        if (stormType == StormType.seeking)
        {
            transform.position = owner.target.transform.position;
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }

        transform.Find("Actual_Lightning").Find("Hitbox").GetComponent<Hitbox>().owner = owner.GetComponent<Unit>();
    }

    private void Update()
    {
        if (strikeTime > 0)
        {
            strikeTime -= Time.deltaTime;
            if (strikeTime < 0 && alive)
            {
                alive = false;
                actualLightning.SetActive(true);
                vfxLightning.SetActive(false);
                Destroy(this.gameObject, 5f);
            }
        }
    }
}
