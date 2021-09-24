using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using UnityEngine;

public class CameraSetup : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine) // 로컬 플레이어라면
        {
            // 씬에 있는 시네머신 가상 카메라 컴포넌트를 찾음
            CinemachineVirtualCamera followCam = FindObjectOfType<CinemachineVirtualCamera>();
            // 가상 카메라의 추적 대상 및 주시 대상을 자신의 트랜스폼 컴포넌트로 지정
            followCam.Follow = transform;
            followCam.LookAt = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
