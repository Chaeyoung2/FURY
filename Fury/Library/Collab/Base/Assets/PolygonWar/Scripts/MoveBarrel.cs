using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBarrel : MonoBehaviour
{
    public float ySpeed = 100.0f;

    private float y = -2.0f;

    private float yMinLimit = -0.3f;
    private float yMaxLimit = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Transform tr = GameObject.FindGameObjectWithTag("Turret").GetComponent<Transform>();
        y = Input.GetAxis("Mouse Y") * ySpeed * 0.015f;
        Debug.Log(y);
        transform.Rotate(-y, 0, 0);
    }
}
