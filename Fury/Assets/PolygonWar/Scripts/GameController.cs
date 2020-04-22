using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // 씬 전환시 플레이어 컨트롤러로부터 넘겨받는 전환직전 플레이어의 포지션
    public static float remainXpos = 0.0f;
    public static float remainYpos = 1.0f;
    public static float remainZpos = 0.0f;

    public static bool exitPosition = false; // 씬3에서 씬1로의 씬 전환이 일어날때 전달 변수

    public static float remainRotationY = 0.0f; // 씬 전환 직전 넘겨받는 포대의 회전각도
    void Start()
    {
        
    }

    void Update()
    {
        if(PlayerCtrl.InTank == true)
        {
            remainXpos = PlayerCtrl.xpos;
            remainYpos = PlayerCtrl.ypos;
            remainZpos = PlayerCtrl.zpos;
        }
        else
        {
            remainRotationY = Ctrl1.rotationY;
        }

        if(Ctrl1.exitPosition == true)
        {
            remainRotationY = Ctrl1.rotationY;
        }
    }
}
