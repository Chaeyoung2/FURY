using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Ctrl2 : MonoBehaviour
{
    private Transform tr;

    public float moveSpeed = 10.0f;

    public float rotSpeed = 200.0f;

    public static bool exitPosition = false;

    private bool Ctrlstatus = false;

    void Start()
    {
        tr = GetComponent<Transform>();
    }

    void Update()
    {
        SelectCamera cg = GetComponent<SelectCamera>();

        if (cg.CamNum == 2)
        {
            Ctrlstatus = true;
        }
        else
        {
            Ctrlstatus = false;
        }

        if (Ctrlstatus == true)
        {
            if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
            {
                Debug.Log("exit scene");
                SceneManager.LoadScene("TestScene1");
            }
            tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X"));
        }
    }
}
