using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBarrel : MonoBehaviour
{
    public float ySpeed = 100.0f; // 포신 상하 회전 속도
    private float angle; // 포신 y축 회전각도z
    private float y = 0.0f;
    private float getmousey=0;
    public GameObject other;

    // 캐릭터 회전관련 변수
    public int rotateSpeed = 10;
    private float rotDirX = 0;

    void Start()
    {
        //angle = transform.localEulerAngles.y;
    }

    void Update()
    {
        /*
        SelectCamera cg = other.GetComponent<SelectCamera>();

        if (cg.CamNum == 1)
        {
            getmousey = Input.GetAxis("Mouse Y");
            angle = transform.localEulerAngles.y;
            Debug.Log(angle);
            y = getmousey * ySpeed * 0.015f;

            if (!((angle < 8.483992E-07 && y > 0) || (angle > 8.570782E-07 && y < 0)))
                transform.Rotate(-y, 0, 0);
        }
        */
        rotDirX = 0;

        if (OVRInput.Get(OVRInput.Touch.PrimaryThumbstick))
        {
            Vector2 coord = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

            if (coord.y > 0)
                rotDirX = -1;
            else if (coord.y < 0)
                rotDirX = +1;
        }

        transform.Rotate(rotDirX * rotateSpeed * Time.smoothDeltaTime, 0, 0);
    }
}
