using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignArrow : MonoBehaviour
{
    public Transform camTransform;
    Quaternion originRot;
    // Start is called before the first frame update
    void Start()
    {
        originRot = transform.rotation;
        //camTransform = GameObject.Find("ObserverCam").GetComponent<Transform>(); // 옵저버는 당연히 계속 rot 값이 0이지..
        camTransform = GameObject.Find("PlayerTank").GetComponent<Transform>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Debug.Log("AlignArrow - LateUpdate()");
        //방1
        //transform.rotation = camTransform.rotation * originRot;

        //방2
        //transform.LookAt(Camera.main.transform.position);
        //transform.forward = Camera.main.transform.forward;

        //방3
        //transform.LookAt(transform.position + Camera.main.transform.forward);

        //방4
        //transform.rotation = Camera.main.transform.rotation * originRot;

        //방5
        //transform.LookAt(Camera.main.transform.position);
        //transform.Rotate(0, -140, 0);

        //방5
        transform.LookAt(camTransform.position);
    }
}
