using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallexScroller : MonoBehaviour
{
    Rigidbody rb;

    public int direction = 1;
    public float speed = 5;

    public Transform endPoint;
    public Transform startPoint;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        rb.velocity = new Vector2(speed * direction, rb.velocity.y);

        if (Mathf.Abs(transform.position.x - endPoint.position.x) < 1)
        {
            transform.position = startPoint.position;
        }
    }
}
