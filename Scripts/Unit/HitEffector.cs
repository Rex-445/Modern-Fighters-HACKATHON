using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffector : MonoBehaviour
{
    float blinkTime;

    public float blinkDuration;
    public float blinkFreq;

    float blinkLength;
    float blinkFreqTime;

    internal bool vulnerable = true;

    SpriteRenderer sprite;

    private void Start()
    {
        blinkLength = blinkDuration;
        Blink();
        sprite = transform.Find("Container").Find("Sprite").GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        vulnerable = blinkLength <= 0;
        if (blinkLength > 0)
        {
            blinkLength -= Time.deltaTime;
            blinkFreqTime -= Time.deltaTime;
            if (blinkFreqTime <= 0)
            {
                blinkFreqTime = blinkFreq;
                sprite.enabled = !sprite.enabled;
            }
        }

        else
        {
            sprite.enabled = true;
        }
    }

    public void Blink(float time=0)
    {
        blinkLength = blinkDuration;
        vulnerable = false;

        if (time != 0)
            blinkLength = time;
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        Transform sprite = transform.Find("Container");
        Vector3 originalPos = Vector3.zero;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1, 1) * magnitude;

            sprite.localPosition = originalPos + new Vector3(x, 0, 0);

            elapsed += Time.deltaTime;

            yield return null;
        }

        sprite.localPosition = originalPos;
    }
}
