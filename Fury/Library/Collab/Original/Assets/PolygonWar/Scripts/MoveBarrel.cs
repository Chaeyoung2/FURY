using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBarrel : MonoBehaviour
{
    public float ySpeed = 100.0f;

    private float y = -2.0f;

    public float yMinLimit = -10.0f;
    public float yMaxLimit = -2.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        y = Input.GetAxis("Mouse Y") * ySpeed * 0.015f;

        //transform.rotation = rotation;
        transform.Rotate(-y, 0, 0);
    }
}
