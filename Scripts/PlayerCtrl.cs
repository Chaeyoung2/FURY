using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerCtrl : MonoBehaviourPun
{
    private Transform tr;

    // 캐릭터가 특정 상태(Climing)일때 플레이어가 조작을 할 수 없게 만들기 위한 플래그
    public static bool OnControll = true;
    // 클라이밍 애니메이션을 실행시키는 플래그
    public static bool IsCliming = false;
    public static bool UpFloor = false;
    public static bool DownFloor = false;

    int uptofloor2,downtofloor1 = 0;

    public GameObject Observer;
    public GameObject ObserverCam;
    public GameObject PlayerCh;

    // 캐릭터 이동관련 변수
    public int speedForward = 1; //전진 속도
    public int speedSide = 1;     //옆걸음 속도
    private float dirX = 0;
    private float dirZ = 0;

    // 캐릭터 회전관련 변수
    public int rotateSpeed = 60;
    private float rotDirY = 0;

    public static Vector3 pos;

    public static int CamNum = 0; // 씬 전환시 바라보는 카메라를 결정하는 변수
    public static bool Ob = false;
    public static bool Ap = false;

    Transform floor1, floor2_1,floor2_2,ch;
    
    Vector3 to2f;
    void Awake()
    {
        floor2_1 = GameObject.Find("상호작용지점").transform.Find("floor2-1");
        floor2_2 = GameObject.Find("상호작용지점").transform.Find("floor2-2");
        floor1 = GameObject.Find("상호작용지점").transform.Find("floor1");
    }

    void Start()
    {
        //자신의 기본 트랜스폼 저장
        tr = GetComponent<Transform>();

        
        if (GameController.exitPosition == true)
        {
            pos = GameController.remainpos;

        }

    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "WAY1")
        {
            Debug.Log("포병위치로");
            if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch)|| Input.GetKeyDown(KeyCode.L))
            {
                //Ap = true;
                //tr.gameObject.SetActive(false);
            }
        }

        if (col.gameObject.tag == "WAY2")
        {
            Debug.Log("관측병위치로");
            if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch) || Input.GetKeyDown(KeyCode.L))
            {
                Ob = true;
                GameController.remainpos = pos;
            }
        }
        
        if (col.gameObject.tag == "to1stfloor")
        {
            Debug.Log("1층 내려가기");
            if ((OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch) || Input.GetKeyUp(KeyCode.Y))&&uptofloor2==0)//올라가는중이나 내려가는중에 다시눌려지는거방지
            {
                tr.position = new Vector3(floor2_2.position.x, tr.position.y -0.5f, floor2_2.position.z);
                tr.rotation = Quaternion.Euler(0, -5, 0);

                downtofloor1 = 1;
                // 캐릭터 Climing상태 진입
                OnControll = false;
                UpFloor = false;
                DownFloor = true;
                Debug.Log("1층 내려가기");
            }
        }
        
        if (col.gameObject.tag == "to2floor")
        {
            Debug.Log("2층 올라가기");
            // if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
            if ((OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch) || Input.GetKeyUp(KeyCode.Y))&&downtofloor1==0)
            {
                tr.position = new Vector3(floor1.position.x, tr.position.y, floor1.position.z);
                tr.rotation = Quaternion.Euler(0, -5, 0);

                uptofloor2 = 1;
                IsCliming = true;
                OnControll = false;
                UpFloor = true;
                DownFloor = false;
                Debug.Log("2층 올라가기");
            }
        }

    }

    public IEnumerator ActiveObserver()
    {
        yield return new WaitForEndOfFrame();
    }

    void Update()
    {
        // 로컬 플레이어가 아닌 경우 입력 받지 않음 --> 서버에 연결하는 기능 넣고나서 주석 풀 것
        //if (!photonView.IsMine)
        //{
        //    return;
        //}

        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
            IsCliming = true;

        if (Ob)
        {
            tr.position = ObserverCam.transform.position;
        }
        if (OnControll)
        {
            MovePlayer();
            if(!Ob)
                RotatePlayer();
        }
        else
        {
            ClimingFloor();
        }
    }

    //플레이어 이동
    void MovePlayer()
    {
        dirX = 0;   //좌우 이동 방향 (L: -1, R: 1)
        dirZ = 0;   //앞뒤 이동 방향 (B: -1, F: 1)

        if (OVRInput.Get(OVRInput.Touch.PrimaryThumbstick))
        {
            Vector2 coord = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

            var absX = Mathf.Abs(coord.x);
            var absY = Mathf.Abs(coord.y);

            if (absX > absY)
            {
                //우측
                if (coord.x > 0)
                    dirX = +1;
                //좌측
                else
                    dirX = -1;
            }
            else
            {
                //앞
                if (coord.y > 0)
                    dirZ = +1;
                //뒤
                else if (coord.y < 0)
                    dirZ = -1;
            }
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            dirX = +5;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            dirX = -5;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            dirZ = +5;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            dirZ = -5;
        }
        //이동 방향 설정 후 이동
        Vector3 moveDir = new Vector3(dirX * speedSide, 0, dirZ * speedForward);
        transform.Translate(moveDir * Time.smoothDeltaTime);
        //프레임워크에 전해줄 포지션값 갱신
        pos = tr.position;
    }

    void RotatePlayer()
    {
        rotDirY = 0;

        if (OVRInput.Get(OVRInput.Touch.SecondaryThumbstick))
        {
            Vector2 coord = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

            if (coord.x > 0)
                rotDirY = +1;
            else if (coord.x < 0)
                rotDirY = -1;
        }

        transform.Rotate(0, rotDirY * rotateSpeed * Time.smoothDeltaTime, 0);
    }

    void ClimingFloor()
    {
        //올라가고 내려가는 부분 코루틴형태로 함수만들어서 충돌했을때 코루틴시작 목표지점에도착할때 코루틴끝 형태로 함수만들어주면 성능향상
        //1층에서 2층올라가는부분
        if (uptofloor2 == 1)
        {
            tr.position = Vector3.MoveTowards(tr.position, floor2_1.position, 2* Time.deltaTime);           
            if (Vector3.Distance(tr.position, floor2_1.position) < 0.01)
            {
                uptofloor2 = 2;
                //올라가는 애니끝내기
                IsCliming = false;
            }

        }
        if (uptofloor2 == 2)
        {
            tr.position = Vector3.MoveTowards(tr.position, floor2_2.position, 2* Time.deltaTime);
            if (Vector3.Distance(tr.position, floor2_2.position) < 0.01)
            {
                uptofloor2 = 0;
                Debug.Log("2층올라가기끝");
                tr.position += new Vector3(0, 0.5f, 0);
                OnControll = true;
            }
        }
        //2층에서 1층내려가는부분
        if (downtofloor1 == 1)
        {
            tr.position = Vector3.MoveTowards(tr.position, floor2_1.position, 2* Time.deltaTime);
            if (Vector3.Distance(tr.position, floor2_1.position) < 0.01)
            {
                downtofloor1 = 2;

                //사다리타기애니메이션켜기
                IsCliming = true;
            }
        }
        if (downtofloor1 == 2)
        {
            tr.position = Vector3.MoveTowards(tr.position, floor1.position, 2* Time.deltaTime);
            if (Vector3.Distance(tr.position, floor1.position) < 0.01)
            {
                //사다리타는에니메이션끄기
                downtofloor1 = 0;
                IsCliming = false;
                tr.position += new Vector3(0, 0.5f, 0);
                OnControll = true;
            }
        }
    }
}


