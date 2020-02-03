using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCamera : MonoBehaviour
{
    public Camera ObserverCam;
    public Camera ArtilleryCam;
    public GameObject SetScope;

    public bool Cam1 = true;
    public bool Cam2 = false;

    void Swap()
    {
        if (Cam1 == true)
        {
            Cam1 = false;
            Cam2 = true;
        }
        else
        {
            Cam1 = true;
            Cam2 = false;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Swap();
        }

        if (Cam1 == true)
        {
            ArtilleryCam.enabled = true;
            ObserverCam.enabled = false;
            SetScope.SetActive(true);
        }
        else
        {
            ArtilleryCam.enabled = false;
            ObserverCam.enabled = true;
            SetScope.SetActive(false);
        }

    }
}