using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BackgroundManager : MonoBehaviour
{
    public Tilemap MAP;
    public static BackgroundManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else { print("Another instance of 'BackgroundManager' is running on the object: " + this.name); }

        MAP.color = Color.white;
    }



    public void ActivateWorld()
    {
        MAP.color = new Color(1, 1, 1, 1);
    }

    IEnumerator SlowDeactivate()
    {
        while(MAP.color.r > 0.1f)
        {
            MAP.color = Color.Lerp(MAP.color, new Color(0, 0, 0, 1), 2 * Time.deltaTime);
            yield return null;
        }
    }

    public void DeactivateWorld(Color theme)
    {
        MAP.color = theme;

        //MAP.color = new Color(0, 0, 0, 1);
    }

    IEnumerator SlowActivate()
    {
        while (MAP.color.r < 0.9f)
        {
            MAP.color = Color.Lerp(MAP.color, new Color(1, 1, 1, 1), 2 * Time.deltaTime);
            yield return null;
        }
    }
}
