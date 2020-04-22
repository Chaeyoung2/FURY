using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBarrel : MonoBehaviour
{
    public float ySpeed = 100.0f; // 포신 상하 회전 속도
    private float angle; // 포신 y축 회전각도
    private float y = -2.0f;
    private float getmousey=0;
    private float curaxis = 0;
    public GameObject other;

    void Start()
    {
        angle = transform.localEulerAngles.y;
    }

    void Update()
    {
        SelectCamera cg = other.GetComponent<SelectCamera>();

        if (cg.CamNum == 1)
        {
            getmousey = Input.GetAxis("Mouse Y");
            angle = transform.localEulerAngles.y;
            y = getmousey * ySpeed * 0.015f;
            curaxis += y;

            if (!((angle < 8.483992E-07 && y > 0) || (angle > 8.570782E-07 && y < 0)))
                transform.Rotate(-y, 0, 0);
        }
    }
}
