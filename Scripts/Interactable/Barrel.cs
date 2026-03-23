using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public AudioClip[] clips;
    public HitEffector hitEffector;
    AudioSource source;

    public GameObject dropOff;

    public Sprite broken;

    public float health;

    public GameObject hitVfx;
    bool alive = true;
    public LayerMask nullLayer;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    private void Update()
    {
        transform.parent.parent.Find("Shadow").transform.position = new Vector3(transform.position.x - 0.04f, -1.2f, transform.position.z);
        transform.parent.parent.Find("Shadow").transform.eulerAngles = Vector3.zero;
    }



    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody>().velocity.magnitude > 1)
            print("YES");

        int rand = Random.Range(0, clips.Length);
        source.clip = clips[rand];
        source.Play();
    }


    void SFXandVFX(int dmgType)
    {
        Vector3 point = transform.position;
        Vector2 dir = new Vector2(point.x, point.y);
        if (point.x < transform.position.x)
            dir.x = 1;

        if (point.x > transform.position.x)
            dir.x = -1;
        Vector2 offset = new Vector2(0, 0);

        //Lightning
        if (dmgType == 2)
        {
            SoundManager.instance.PlaySound(8, transform);
            SpawnManager.instance.SpawnObject(6, transform.position, 2);
        }

        //Fire
        else if (dmgType == 4)
        {
            SoundManager.instance.PlaySound(10, transform);
            SpawnManager.instance.SpawnObject(8, transform.position, 2);
        }

        //Default
        else
        {
            SpawnManager.instance.SpawnObject(0, transform.position, 2);
        }
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.GetComponent<Hitbox>() != null)
        {
            //SFX and VFX
            SFXandVFX((int)collision.transform.GetComponent<Hitbox>().damageType);
            int rand = Random.Range(0, clips.Length);
            source.clip = clips[rand];
            source.Play();

            //Hitbox Damage
            Hitbox hitBox = collision.transform.GetComponent<Hitbox>();
            transform.parent.parent.GetComponent<Rigidbody>().velocity = new Vector3(hitBox.force * hitBox.owner.transform.localScale.x, hitBox.forceUp, 0);

            health -= hitBox.damage;

            if (health <= 0 && alive)
            {
                alive = false;
                gameObject.layer = nullLayer;
                GetComponent<SpriteRenderer>().sprite = broken;
                Instantiate(dropOff, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);
                GameObject go = Instantiate(hitVfx, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity) as GameObject;
                Destroy(go, 1);
                DataManager.instance.barrels += 1;
                DataManager.instance.SetBarrel();
                Destroy(this.transform.parent.parent.gameObject, 2);
                DataManager.instance.dataPersistence.gameData.barrelsDestroyed += 1;
                hitEffector.Blink(3);
            }
        }
    }
}
