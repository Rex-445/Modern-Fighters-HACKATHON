using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public IEnumerator Shake(float duration, float magnitude, bool isCrit)
    {
        Vector3 originalPos = new Vector3(0, 2, -4);

        float elapsed = 0.0f;

        Animator anim = UIManager.instance.transform.Find("Camera Intensity").GetComponent<Animator>();

        if (isCrit)
            anim.Play("Camera Intensity");

        while (elapsed < duration)
        {
            float x = Random.Range(-1, 1) * magnitude;
            float y = Random.Range(-1, 1) * magnitude;

            transform.localPosition = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;

            yield return null;
        }

        if (isCrit)
            anim.Play("End Camera Intensity");

        transform.localPosition = originalPos;
    }
}
