using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public string moveAxisName = "Vertical"; // 앞뒤 움직임 입력축
    public string rotateAxisName = "Horizontal"; // 좌우 회전 입력축

    public float move { get; private set; } // 감지된 움직임 입력 값
    public float rotate { get; private set; } // 감지된 회전 입력 값
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 로컬 플레이어가 아닌 경우 입력을 받지 않는다.
        //if ()
        //{
        //    return;
        //}

        // 게임 오버인 경우 사용자 입력을 감지하지 않는다.
        //if ()
        //{
        //    return;
        //}

        // move에 관한 입력 감지
        move = Input.GetAxis(moveAxisName);
        rotate = Input.GetAxis(rotateAxisName);
        
    }
}
