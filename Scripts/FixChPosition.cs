using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixChPosition : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform tr;
    private GameObject cam;
    void Start()
    {
        cam = GameObject.Find("RightEyeAnchor");
        tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        //카메라 포지션에따라 캐릭터를고정시킴
        tr.position = new Vector3(cam.transform.position.x, tr.position.y, cam.transform.position.z-0.2f);
    }
}
