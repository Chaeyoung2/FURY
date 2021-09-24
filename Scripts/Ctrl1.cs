using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Ctrl1 : MonoBehaviour
{
    private Transform tr;

    public int rotateSpeed = 60;
    private float rotDirY = 0;

    void Start()
    {
        tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        rotDirY = 0;

        if (OVRInput.Get(OVRInput.Touch.SecondaryThumbstick))
        {
            Vector2 coord = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

            if (coord.x > 0)
                rotDirY = +1;
            else if (coord.x < 0)
                rotDirY = -1;
        }

        transform.Rotate(0, rotDirY * rotateSpeed * Time.smoothDeltaTime, 0);
    }
}
