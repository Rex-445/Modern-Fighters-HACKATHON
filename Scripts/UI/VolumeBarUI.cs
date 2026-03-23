using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class VolumeBarUI : MonoBehaviour
{
    public Gradient colorValue;
    public Gradient unlimitedValue;
    public Color targetColor;


    [Range(0, 12)] public float gaugeValue;

    public Image defaultBar;
    public List<GameObject> barList;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGuageColor();
    }

    void UpdateGuageColor()
    {
        int count = 0;

        foreach (GameObject bar in barList)
        {/*
            if (gaugeValue != 0 && count != 0)
                barList[count].GetComponent<Image>().color = targetColor;*/

            if ((int)gaugeValue > count)
            {
                targetColor = colorValue.Evaluate((float)count / barList.Count);
            }

            if (gaugeValue < count)
            {
                targetColor = defaultBar.color;
            }

            if (gaugeValue < 1 && gaugeValue > 0)
            {
                targetColor = defaultBar.color;
            }

            barList[count].GetComponent<Image>().color = targetColor;
            count++;
        }
    }

    private void OnDrawGizmos()
    {

#if UNITY_EDITOR
        //UpdateGuageColor();
#endif
    }
}
