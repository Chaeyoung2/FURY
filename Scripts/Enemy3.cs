//Enemy3.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy3 : LivingObject // 3.배회
{
    private Animator anim;
    private Transform target;
    private NavMeshAgent agent;
    private Transform[] patrolPoints;
    public CapsuleCollider coll;
    private int currentControlPointIndex = 1;
    private bool patrol = false;
    private int group = 0;

    private float runningTime = 0f; // 너무 총만 쏘면 이상하니까 잠깐 두리번거리는 모션
    private float runningMaxTime = 0f;
    private float waitingTime = 0f;
    private float waitingMaxTime = 0f;
    private bool waiting = false;

    private float lifeTime = 0f; // 1분 이상 지나면 사라지도록
    private float lifeMaxTime = 0;
    private bool enemyDead = false;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindWithTag("Tank").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = Random.Range(4f, 7f);
        anim = GetComponent<Animator>();
        coll = GetComponent<CapsuleCollider>();

        setPatrolPointInGroup();

        MoveToNextPatrolPoint();


        health = 30f;

        waitingMaxTime = Random.Range(2f, 10f);
        runningMaxTime = Random.Range(20f, 30f);
        lifeMaxTime = 10f;
        anim.SetBool("Run", true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!enemyDead)
        {
            if (!waiting)
            {
                //Debug.Log("currentControlPointIndex : " + currentControlPointIndex);
                //Debug.Log("patrolPoints.Length : " + patrolPoints.Length);
                //Debug.Log("agent.pathPending : " + agent.pathPending);
                //Debug.Log("agent.remainingDistance : " + agent.remainingDistance);
                patrol = false;
                patrol = !dead && patrolPoints.Length > 0;

                if (!patrol)
                    agent.SetDestination(transform.position);
                if (patrol)
                {
                    if (!agent.pathPending && agent.remainingDistance < 2f)
                    {
                        MoveToNextPatrolPoint();
                        //Debug.Log("MoveToNextPatrolPoint()");
                    }
                }
                runningTime += Time.deltaTime;
                if (runningTime > runningMaxTime)
                {
                    waiting = true;
                    anim.SetBool("Idle", true);
                    runningTime = 0f;
                }

                Move();
            }
            else
            {
                Wait();
                waitingTime += Time.deltaTime;
                if (waitingTime > waitingMaxTime)
                {
                    waiting = false;
                    anim.SetBool("Idle", false);
                    waitingTime = 0f;
                }
            }

            lifeTime += Time.deltaTime;
            if (lifeTime >= lifeMaxTime)
            {
                anim.SetTrigger("Die");
                enemyDead = true;
                agent.isStopped = true;// 이렇게 안하면 콜라이더가 그대로라서 붕 뜸;;
                agent.enabled = false;
                coll.height = 0.12f;
                coll.radius = 0.2f;
            }

            //Debug.Log("patrol : " + patrol);
            //anim.SetBool("Run", patrol);


        }
        else
        {
            Destroy(gameObject, 30f);
        }
    }

    void MoveToNextPatrolPoint()
    {
        if (patrolPoints.Length > 0)
        {
            agent.destination = patrolPoints[currentControlPointIndex].position;
            currentControlPointIndex++;
            currentControlPointIndex %= patrolPoints.Length;
            if (currentControlPointIndex == 0)
                currentControlPointIndex = 1;
        }
    }

    private void Move()
    {
    }

    private void AvoidTank()
    {
        float dist = Vector3.Distance(transform.position, target.position);
        if (dist < 5f) // 탱크와 너무 가까워지면 경로 변경
        {
            agent.destination = patrolPoints[currentControlPointIndex].position;
            currentControlPointIndex--;
            currentControlPointIndex %= patrolPoints.Length;
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.tag == "BULLET") // 총알 피격시 오브젝트 삭제
        {
            Destroy(coll.gameObject);
            //Destroy(gameObject);
            DeductHealth(10);
            anim.SetTrigger("hit");
        }
    }
    void DeductHealth(float deductHealth)
    {
        health -= deductHealth;
        if (health <= 0)
        {
            EnemyDead();
        }
    }
    void EnemyDead()
    {
        anim.SetTrigger("Die");
        enemyDead = true;
    }

    public void setGroup(int _group)
    {
        group = _group;

    }
    public void setPatrolPointInGroup()
    {
        if (group == 0)
        {
            patrolPoints = GameObject.Find("PatrolPoints0").GetComponentsInChildren<Transform>();
        }
        else if (group == 1)
        {
            patrolPoints = GameObject.Find("PatrolPoints1").GetComponentsInChildren<Transform>();

        }
    }

    public void Wait()
    {

    }
}