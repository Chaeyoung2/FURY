using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Ctrl1 : MonoBehaviour
{
    private Transform tr;

    public float moveSpeed = 10.0f;

    public float rotSpeed = 200.0f;

    public static float rotationY = 0;

    public static bool exitPosition = false;

    private bool Ctrlstatus = false;
   
    void Start()
    {
        tr = GetComponent<Transform>();

        tr.rotation = Quaternion.Euler(0, GameController.remainRotationY, 0);
    }

    // Update is called once per frame
    void Update()
    {
        SelectCamera cg = GetComponent<SelectCamera>();

        if (cg.CamNum == 1)
        {
            Ctrlstatus = true;
        }
        else
        {
            Ctrlstatus = false;
        }

        if (Ctrlstatus == true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                exitPosition = true;
                SceneManager.LoadScene("TestScene1");
            }
            tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X"));

            Vector3 angles = transform.eulerAngles;

            rotationY = angles.y;
            Debug.Log("rotate value : ("+ rotationY + ")");
        }
    }
}
