using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public IEnumerator Shake(float dur, float mag)
    {
        Vector3 oriPos = transform.localPosition;
        float elapsed = 0.0f;
        while(elapsed < dur)
        {
            //Debug.Log("Shake");
            float value = 1f;
            float x = Random.Range(-value, value) * mag;
            float y = Random.Range(-value, value) * mag;
            float z = Random.Range(-value, value) * mag;

            transform.localPosition = new Vector3(x, y, z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = oriPos;
    }
}
