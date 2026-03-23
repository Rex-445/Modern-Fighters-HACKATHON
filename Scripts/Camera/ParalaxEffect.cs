using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxEffect : MonoBehaviour
{
    public Vector2 offset;
    private Vector2 realPosition;

    private void Start()
    {
        realPosition = transform.position;        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(realPosition.x - Camera.main.transform.position.x * offset.x, realPosition.y);
    }
}
