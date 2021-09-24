using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCamera : MonoBehaviour
{
    public Camera ObserverCam;
    public Camera ArtilleryCam;
    public GameObject SetScope;

    public int CamNum = 0;

    void Awake()
    {
        CamNum = PlayerCtrl.CamNum; // 플레이어 컨트롤러에서 카메라 넘버 변수를 전달받음
    }

    void Update()
    {
        if (CamNum == 1) // 포병시점 (스코프On)
        {
            ArtilleryCam.enabled = true;
            ObserverCam.enabled = false;
            //SetScope.SetActive(true);
        }
        else if(CamNum == 2) // 정찰병시점 (스코프Off)
        {
            ArtilleryCam.enabled = false;
            ObserverCam.enabled = true;
            //SetScope.SetActive(false);
        }

    }
}