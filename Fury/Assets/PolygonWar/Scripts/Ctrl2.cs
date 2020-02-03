using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ctrl2 : MonoBehaviour
{
    //private float h = 0.0f;
    //private float v = 0.0f;

    private Transform tr;

    public float moveSpeed = 10.0f;

    public float rotSpeed = 200.0f;

    private bool Ctrlstatus = false;
    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        
    }

    // Update is called once per frame
    void Update()
    {
        SelectCamera cg = GetComponent<SelectCamera>();
        //h = Input.GetAxis("Horizontal");
        //v = Input.GetAxis("Vertical");

        //Debug.Log("H =" + h.ToString());
        //Debug.Log("V =" + v.ToString());

        //Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        //tr.Translate(moveDir * Time.deltaTime * moveSpeed, Space.Self);
        if (cg.Cam2 == true)
        {
            Ctrlstatus = true;
        }
        else
        {
            Ctrlstatus = false;
        }

        if (Ctrlstatus == true)
        {
            tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X"));
        }
    }
}
