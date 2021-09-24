using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePointToPoint : MonoBehaviour
{
    private Rigidbody rg;
    private Transform tr;
    private Transform[] points;
    private int waypointIndex = 1; // 왜 0부터 시작하지 않지????????????? 일단 1로 넣어야 됨
    private int num = 0;
    [SerializeField] private float speed;
    [SerializeField] private float rotspeed;
    public EnemySpawner enemySpawner;

    private bool arrived = false;
    private float targetAngle = 0f;
    private float curAngle = 0f;
    private bool running = false;


    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        points = GameObject.Find("WayPointGroup").GetComponentsInChildren<Transform>(); //웨이포인트그룹 오브젝트멤버들의 Transform컴포넌트 정보를 배열에 저장
        rg = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            running = !running;
        }
        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch) || Input.GetKeyDown(KeyCode.L))
        {
            running = !running;
        }
        if (running)
        {
            if (arrived)
                Rotate();
            else
                Move();
        }

    }

    void OnTriggerEnter(Collider coll)
    {
        //웨이포인트 태그가 붙은 게임 오브젝트와 충돌체크
        if (coll.CompareTag("WAY_POINT"))
        {
            //Debug.Log("arrived at " + waypointIndex);
            enemySpawner.CreateEnemy(waypointIndex);

            //현재 타겟 포인트가 마지막 포인트가 아니면 타겟 인덱스를 증가
            if (waypointIndex != points.Length - 1)
                waypointIndex++;
            else
            {
                //마지막 웨이포인트라면 스크립트 종료
                tr.GetComponent<MovePointToPoint>().enabled = false;
            }

            // 다음에 갈 지점과의 각도 측정
            if (arrived == false) // 최초 한번만 해야 해서.
            {
                arrived = true;
                Vector3 dir = points[waypointIndex].position - tr.position;
                //dotValue = Vector3.Dot(dir, tr.forward);// 이렇게 해도 되긴 되는데 ..
                targetAngle = Vector3.Angle(tr.forward, dir);
                if (targetAngle < 0)
                    targetAngle = 360f + targetAngle;
            }
        }
    }

    private void Rotate()
    {
        //Debug.Log("Rotate");
        //// 회전할 필요가 없으면 바로 끔.
        //if (targetAngle < 5f)
        //{
        //    curAngle = 0f;
        //    targetAngle = 0f;
        //    arrived = false;
        //}
        //else
        //{
        //    // 목표 각도와 현재 각도 차이가 5 이상 -> 회전할 필요가 있따
        //    float rotValue = 1f;
        //    curAngle += rotValue;
        //    //transform.Rotate(0f, rotValue, 0f, Space.World);
        //    //transform.rotation = Quaternion.Euler(0, rotValue, 0);


        //    if (targetAngle <= curAngle)
        //    {
        //        curAngle = 0f;
        //        targetAngle = 0f;
        //        arrived = false;
        //    }
        //}

        ///////////////////////////////////////////////////////////////
        Vector3 dir = points[waypointIndex].position - tr.position;
        Quaternion rot = Quaternion.LookRotation(dir);                              //계산된 벡터를 쿼터니언 타입으로 변환
        tr.rotation = Quaternion.Slerp(tr.rotation, rot, Time.deltaTime * rotspeed);//현재 각도에서 목표로하는 각도까지 회전
        targetAngle = Vector3.Angle(tr.forward, dir);
        if (targetAngle < 0)
            targetAngle = 360f + targetAngle;
        if (targetAngle <= 1)
        {
            arrived = false;
        }
        //Debug.Log("targetAngle : " + targetAngle);
    }

    private void Move()
    {
        //Debug.Log("Move");
        ////////////////////////////////////////////////////////////////////////////////// 방법1
        //Debug.Log("waypointIndex : " + waypointIndex);

        ////Debug.Log("waypointIndex : " + waypointIndex);
        ////Debug.Log("points[waypointIndex].position : " + points[waypointIndex].position.x + ", " + points[waypointIndex].position.y + ", " + points[waypointIndex].position.z);
        //Debug.Log("tr.position : " + tr.position.x + ", " + tr.position.y + ", " + tr.position.z);
        //Vector3 dir = points[waypointIndex].position - tr.position;                 //진행 방향 벡터를 계산

        //float dist = Vector3.Distance(points[waypointIndex].position, tr.position);

        //Quaternion rot = Quaternion.LookRotation(dir);                              //계산된 벡터를 쿼터니언 타입으로 변환
        //tr.rotation = Quaternion.Slerp(tr.rotation, rot, Time.deltaTime * rotspeed);//현재 각도에서 목표로하는 각도까지 회전
        //                                                                            //Debug.Log("rotation");

        //tr.Translate(Vector3.forward * Time.deltaTime * speed);                     //진행 방향으로 이동
        //                                                                            //Debug.Log("Translate");

        //////////////////////////////////////////////////////////////////////////////////방법2
        //transform.position = Vector3.MoveTowards(transform.position, points[waypointIndex].position, speed * Time.deltaTime);


        //////////////////////////////////////////////////////////////////////////////////방법3 // 뭔가 이상함.. freeze rotation 안해줄순 없나..
        //Vector3 dir = points[waypointIndex].position - tr.position;   
        //rg.AddForceAtPosition(dir.normalized, transform.position);


        //////////////////////////////////////////////////////////////////////////////////방법4
        Vector3 dir = points[waypointIndex].position - tr.position;
        rg.MovePosition(transform.position + dir.normalized * Time.fixedDeltaTime * speed);
    }

}