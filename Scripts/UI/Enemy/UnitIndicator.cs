using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UnitIndicator : MonoBehaviour
{
    public Transform target;

    CutSceneManager cutSceneManager;

    [System.Obsolete]
    // Update is called once per frame
    void Update()
    {
        Unit player = UnitManager.instance.player;
        //Seek
        if (target)
        {
            Vector3 point = Camera.main.WorldToScreenPoint(target.transform.position);
            float dist = target.transform.position.x - Camera.main.transform.position.x;

            transform.position = new Vector2(transform.position.x, point.y);

            //Left Side
            transform.Find("Content").Find("Left").gameObject.SetActive(dist < -6.5f);
            
            //Right Side
            transform.Find("Content").Find("Right").gameObject.SetActive(dist > 6.5f);

            //If in any kind of cutscene that would cause the player to be stripped from controls
            if (player)
            {
                if (player.GetComponent<UnitController>().canControl == false || target.gameObject.active == false)
                {
                    //Left Side
                    transform.Find("Content").Find("Left").gameObject.SetActive(false);

                    //Right Side
                    transform.Find("Content").Find("Right").gameObject.SetActive(false);
                }
            }
        }

        if (target == null)
            Destroy(this.gameObject);
    }
}
