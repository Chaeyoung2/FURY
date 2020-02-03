using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBarrel : MonoBehaviour
{
    public float ySpeed = 100.0f;
    private float angle;
    private float y = -2.0f;
    private float getmousey=0;
    private float curaxis = 0;
    public GameObject other;

    // Start is called before the first frame update
    void Start()
    {
        angle = transform.localEulerAngles.y;


    }

    // Update is called once per frame
    void Update()
    {
        SelectCamera cg = other.GetComponent<SelectCamera>();
        //Transform tr = GameObject.FindGameObjectWithTag("Turret").GetComponent<Transform>();
        if (cg.Cam1 == true)
        {
            getmousey = Input.GetAxis("Mouse Y");
            angle = transform.localEulerAngles.y;
            y = getmousey * ySpeed * 0.015f;
            curaxis += y;
            Debug.Log(angle);
            if (!((angle < 8.483992E-07 && y > 0) || (angle > 8.570782E-07 && y < 0)))
                transform.Rotate(-y, 0, 0);
        }
    }
}
