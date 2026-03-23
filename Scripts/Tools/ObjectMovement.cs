using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    Rigidbody rb;

    public int direction = 1;
    public float speed = 5;
    public float lifeTime = 0;
    bool isAlive = false;

    public bool seek;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        isAlive = lifeTime > 0;

        if (seek)
        {
            transform.position = UnitManager.instance.player.transform.position;
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
    }

    private void Update()
    {
        if (isAlive)
        {
            lifeTime -= Time.deltaTime * 2;
            if (lifeTime <= 0)
            {
                Destroy(this.gameObject);
            }
        }

        if (seek)
            return;

        transform.localScale = new Vector2(direction, 1f);
        rb.velocity = new Vector2(speed * direction, rb.velocity.y);
    }
}
