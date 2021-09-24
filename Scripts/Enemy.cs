//Enemy.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum enemy_state { state_idle, state_run, state_damaged, state_attack, state_dead }

public class Enemy : LivingObject
{
    private LivingObject targetObject; // 추적할 대상
    private Transform target;
    private NavMeshAgent agent;
    private Animator anim;
    public LineRenderer bulletTrail;
    public CapsuleCollider coll;

    public LayerMask whatIsTarget; // 추적 대상 레이어

    private enemy_state state = enemy_state.state_idle; // 적 상태

    private float AttackDist;
    private float FollowRad;
    [SerializeField] private float followSpeed; // 따라가는 속도
    public int aiType; // ai 유형 // 0.어그로 1.배회 2.저격수 3.전차?

    public Transform fireTr; // 총알 발사 위치
    [SerializeField] private float lastFireTime; // 마지막 총 발사 후 지난 시간
    [SerializeField] private float betFireTime; // 총 발사 간격
    public ParticleSystem muzzleFlashEffect; // 총구 화염 효과
    [SerializeField] private float damage; // 공격력
    [SerializeField] private float fireDistance; // 사정거리
    private int fireCnt = 0; // 발사 횟수 // 장전 위함
    private int fireMaxCnt = 0; // 발사 최대 횟수

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
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        target = GameObject.FindWithTag("Tank").GetComponent<Transform>();
        coll = GetComponent<CapsuleCollider>();

        // agent 설정 값
        agent.updateRotation = false;

        // 코루틴
        //StartCoroutine(followTank()); -> 코루틴 말고 Update 에서 돌리는 걸로 변경.
        dead = false;
        health = 30f;

        // ai 타입에 따른 설정 값 변경
        InitializeInType(aiType);
    }

    void InitializeInType(int aiType)
    {
        damage = Random.Range(1, 1); // 5 // 개발할땐 일단 0-1로
        AttackDist = Random.Range(100f, 150f); // 20 // 더 넓히자 한 40?
        betFireTime = Random.Range(1f, 1.5f); // 0.15
        FollowRad = Random.Range(200, 210);
        fireDistance = 1000f;
        followSpeed = Random.Range(2f, 5f);
        fireMaxCnt = Random.Range(20, 25);
        waitingMaxTime = Random.Range(2f, 6f);
        runningMaxTime = Random.Range(30f, 60f);
        lifeMaxTime = 660f;

    }
    // Update is called once per frame
    void Update()
    {
        if (!enemyDead)
        {
            if (!waiting)
            {
                FollowTank();
                runningTime += Time.deltaTime;
                if (runningTime > runningMaxTime)
                {
                    waiting = true;
                    anim.SetBool("RifleIdle", true);
                    runningTime = 0f;
                }
            }
            else
            {
                Wait();
                waitingTime += Time.deltaTime;
                if (waitingTime > waitingMaxTime)
                {
                    waiting = false;
                    anim.SetBool("RifleIdle", false);
                    waitingTime = 0f;
                }
            }

            //Debug.Log("lifeTime : " + lifeTime);
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
        }
        else
        {
            Destroy(gameObject, 5f);
        }
        //anim.SetBool("HasTarget", hasTarget);

        //transform.rotation = Quaternion.LookRotation(agent.velocity.normalized);
        //transform.rotation = Quaternion.LookRotation(target.position);
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

    void Fire()
    {
        //Debug.Log("fireCnt : " + fireCnt);
        //Debug.Log("anim.GetCurrentAnimatorStateInfo(0).GetHashCode() : " + anim.GetCurrentAnimatorStateInfo(0).GetHashCode().ToString());
        //Debug.Log("lastFireTime : "  + lastFireTime);
        //Debug.Log("Time.deltaTime : " + Time.deltaTime);

        // 일정 횟수 더 장전했다면 fire하지말고 Reload 애니메이션
        if (fireCnt >= fireMaxCnt)
        {
            anim.SetTrigger("Reload");
            //anim.SetBool("ClosedTarget", false);
            fireCnt = 0;
        }
        else
        {
            anim.SetBool("ClosedTarget", true);
            // 장전 애니메이션 중일 때는 fire ㄴㄴ
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Reload"))
            {
                lastFireTime += Time.deltaTime;

                if (lastFireTime >= betFireTime)
                {
                    fireCnt++;


                    lastFireTime = 0;

                    // 레이캐스트에 의한 충돌 정보를 저장하는 컨테이너
                    RaycastHit hit;
                    // 총알이 맞은 곳을 저장할 변수
                    Vector3 hitPosition = Vector3.zero;

                    // 0809:방향을 fireTr.forward가 아닌 tank까지의 방향으로
                    //Debug.Log("target.transform.position : " + target.transform.position.x + ", " + target.transform.position.y + ", " + target.transform.position.z);
                    Vector3 targetCenter = target.transform.position;
                    targetCenter.y += 2.7f;
                    Vector3 dir = targetCenter - fireTr.transform.position;

                    // 레이캐스트(시작지점, 방향, 충돌 정보 컨테이너, 사정거리)
                    if (Physics.Raycast(fireTr.position,
                        /*fireTr.forward*/dir, out hit, fireDistance)) // 레이가 어떤 물체와 충돌한 경우
                    {
                        // 충돌한 상대방으로부터 IDamageable 오브젝트를 가져오기 시도
                        IDamageable target =
                            hit.collider.GetComponent<IDamageable>();


                        // 상대방으로 부터 IDamageable 오브젝트를 가져오는데 성공했다면
                        if (target != null)
                        {
                            // 상대방의 OnDamage 함수를 실행시켜서 상대방에게 데미지 주기
                            target.OnDamage(damage, hit.point, hit.normal);
                        }
                        else
                        {
                            //Debug.Log("targetㅇ ㅣ없음");
                        }

                        // 레이가 충돌한 위치 저장
                        hitPosition = hit.point;

                        //Debug.Log("충돌 hitPosition : (" + hitPosition.x + ", " + hitPosition.y + "," + hitPosition.z);
                        //Debug.Log("fireTr : (" + fireTr.position.x + ", " + fireTr.position.y + "," + fireTr.position.z);

                    }
                    else // 레이가 다른 물체와 충돌하지 않았다면
                    {
                        // 총알이 최대 사정거리까지 날아갔을때의 위치를 충돌 위치로 사용
                        hitPosition = fireTr.position + /*fireTr.forward*/dir * fireDistance;
                        //Debug.Log("충돌X hitPosition : (" + hitPosition.x + ", " + hitPosition.y + "," + hitPosition.z);
                    }



                    // 발사 이펙트 재생 
                    muzzleFlashEffect.Play(); // 총구 화염
                    SpawnBulletTrail(hitPosition);// Bullet Trail
                }

            }
            else
            {
                ///Debug.Log("Reload 중이라고 함");
            }

        }


    }

    void FaceTarget(Vector3 destination)
    {
        Vector3 lookPos = destination - transform.position;
        lookPos.y = 0;
        Quaternion rot = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 3f);
    }

    private bool hasTarget
    {
        get
        {
            if (targetObject != null && !targetObject.dead)
            {
                return true;
            }
            return false;
        }
    }

    private IEnumerator followTank()
    {
        while (!dead)
        {
            agent.speed = followSpeed;

            if (hasTarget) // 타겟이 죽지 않았거나 null이 아닐 때 
            {
                // Debug.Log(target.position.x + "," + target.position.y + "," + target.position.z);
                float dist = Vector3.Distance(transform.position, target.position);
                //float dist = Vector3.Distance(transform.position, targetObject.transform.position);
                //Debug.Log("dist: " + dist);


                //Debug.Log("target.position : " + target.position.x + ", " + target.position.y + ", " + target.position.z);

                agent.SetDestination(target.position);

                if (dist > AttackDist)
                {
                    //Debug.Log("dist > AttackDist O 진입");
                    agent.isStopped = false;
                    agent.updatePosition = true;
                    agent.updateRotation = true;
                    anim.SetBool("HasTarget", true);
                    anim.SetBool("ClosedTarget", false);
                    transform.LookAt(target);
                    transform.Rotate(new Vector3(0, 1, 0), 50f);
                }
                else
                {
                    // 계속 dest 찾아가려고 부들부들거림 방지.
                    //agent.ResetPath();

                    //---------------------매끄러운 AI를 위한 흔적-------------------------
                    //Debug.Log("dist > AttackDist X 진입");
                    agent.isStopped = true;
                    agent.updatePosition = false;
                    agent.updateRotation = false;
                    //anim.SetBool("HasTarget", false);
                    anim.SetBool("ClosedTarget", true);

                    //Vector3 temp = target.position - transform.position;
                    //temp.y = 0;
                    //Quaternion targetrot = Quaternion.LookRotation(temp) ;
                    //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetrot, 30*Time.deltaTime);

                    //Transform targetTemp; 
                    //targetTemp.Rotate(new Vector3(0, 1, 0), 30f);

                    transform.LookAt(target);
                    transform.Rotate(new Vector3(0, 1, 0), 50f); // 애니메이션 자체가 잘못된듯.. 총 겨누는 방향이 이상함. 자체적으로 회전시키기.

                    //var rot = transform.rotation;
                    //rot.y -= 90;
                    //transform.rotation = rot;
                    //----------------------------------------------

                    Fire();
                }

                //FaceTarget(transform.position);
            }

            else
            {
                //Debug.Log("hasTarget X 진입");
                // 추적 대상 없음 : AI 이동 중지
                agent.isStopped = true;
                agent.updatePosition = false;
                agent.updateRotation = false;
                //anim.SetBool("HasTarget", false);
                //anim.SetBool("ClosedTarget", false);

                // 20 유닛의 반지름을 가진 가상의 구를 그렸을때, 구와 겹치는 모든 콜라이더를 가져옴
                // 단, targetLayers에 해당하는 레이어를 가진 콜라이더만 가져오도록 필터링
                Collider[] colliders =
                    Physics.OverlapSphere(transform.position, FollowRad, whatIsTarget);

                // 모든 콜라이더들을 순회하면서, 살아있는 플레이어를 찾기
                for (int i = 0; i < colliders.Length; i++)
                {
                    // 콜라이더로부터 livingObject 컴포넌트 가져오기
                    LivingObject livingObject = colliders[i].GetComponent<LivingObject>();

                    // livingObject 컴포넌트가 존재하며, 해당 livingObject 살아있다면,
                    if (livingObject != null && !livingObject.dead)
                    {
                        // 추적 대상을 해당 livingObject 설정
                        targetObject = livingObject;

                        //Debug.Log(aiType +  "번째 enemy 추적 대상 없음 : 추적 대상 찾아 livingObject로 설정함");

                        // for문 루프 즉시 정지
                        break;

                    }
                }

            }
            // Debug.Log(aiType + "번째 enemy FollowTank 호출");
            //Debug.Log(aiType + "번째 dead : " + dead);

            // 0.25초 주기로 처리 반복
            yield return new WaitForSeconds(0.001f);
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (!dead && // 사망하지 않았으며,
            Time.time >= lastFireTime + betFireTime) // 최근 공격 시점에서 bet 이상 시간이 지났으면 공격 가능
        {
            // 상대방으로부터 LivingObject 가져옴
            LivingObject attackTarget = other.GetComponent<LivingObject>();

            // 상대방 LivingObject가 enemy의 추적 대상이라면 공격 실행
            if (attackTarget != null && attackTarget == target)
            {
                // 최근 공격 시간 갱신
                lastFireTime = Time.time;
            }

        }
    }

    public void FollowTank()
    {
        agent.speed = followSpeed;

        if (hasTarget) // 타겟이 죽지 않았거나 null이 아닐 때 
        {
            // Debug.Log(target.position.x + "," + target.position.y + "," + target.position.z);
            float dist = Vector3.Distance(transform.position, target.position);
            //float dist = Vector3.Distance(transform.position, targetObject.transform.position);
            //Debug.Log("dist: " + dist);


            //Debug.Log("target.position : " + target.position.x + ", " + target.position.y + ", " + target.position.z);

            agent.SetDestination(target.position);

            if (dist > AttackDist)
            {
                //Debug.Log("dist > AttackDist O 진입");
                agent.isStopped = false;
                agent.updatePosition = true;
                agent.updateRotation = true;
                anim.SetBool("HasTarget", true);
                anim.SetBool("ClosedTarget", false);
                transform.LookAt(target);
                transform.Rotate(new Vector3(0, 1, 0), 50f);
            }
            else
            {
                // 계속 dest 찾아가려고 부들부들거림 방지.
                //agent.ResetPath();

                //---------------------매끄러운 AI를 위한 흔적-------------------------
                //Debug.Log("dist > AttackDist X 진입");
                agent.isStopped = true;
                agent.updatePosition = false;
                agent.updateRotation = false;
                //anim.SetBool("HasTarget", false);

                //Vector3 temp = target.position - transform.position;
                //temp.y = 0;
                //Quaternion targetrot = Quaternion.LookRotation(temp) ;
                //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetrot, 30*Time.deltaTime);

                //Transform targetTemp; 
                //targetTemp.Rotate(new Vector3(0, 1, 0), 30f);

                transform.LookAt(target);
                transform.Rotate(new Vector3(0, 1, 0), 50f); // 애니메이션 자체가 잘못된듯.. 총 겨누는 방향이 이상함. 자체적으로 회전시키기.

                //var rot = transform.rotation;
                //rot.y -= 90;
                //transform.rotation = rot;
                //----------------------------------------------

                Fire();
            }

            //FaceTarget(transform.position);
        }

        else
        {
            //Debug.Log("hasTarget X 진입");
            // 추적 대상 없음 : AI 이동 중지
            agent.isStopped = true;
            agent.updatePosition = false;
            agent.updateRotation = false;
            //anim.SetBool("HasTarget", false);
            //anim.SetBool("ClosedTarget", false);

            // 20 유닛의 반지름을 가진 가상의 구를 그렸을때, 구와 겹치는 모든 콜라이더를 가져옴
            // 단, targetLayers에 해당하는 레이어를 가진 콜라이더만 가져오도록 필터링
            Collider[] colliders =
                Physics.OverlapSphere(transform.position, FollowRad, whatIsTarget);

            // 모든 콜라이더들을 순회하면서, 살아있는 플레이어를 찾기
            for (int i = 0; i < colliders.Length; i++)
            {
                // 콜라이더로부터 livingObject 컴포넌트 가져오기
                LivingObject livingObject = colliders[i].GetComponent<LivingObject>();

                // livingObject 컴포넌트가 존재하며, 해당 livingObject 살아있다면,
                if (livingObject != null && !livingObject.dead)
                {
                    // 추적 대상을 해당 livingObject 설정
                    targetObject = livingObject;

                    //Debug.Log(aiType + "번째 enemy 추적 대상 없음 : 추적 대상 찾아 livingObject로 설정함");

                    // for문 루프 즉시 정지
                    break;

                }
            }

        }
    }

    public void SpawnBulletTrail(Vector3 hitPoint)
    {
        GameObject bulletTrailEffect = Instantiate(bulletTrail.gameObject, fireTr.position, Quaternion.identity);

        LineRenderer lineR = bulletTrailEffect.GetComponent<LineRenderer>();

        lineR.SetPosition(0, fireTr.position);
        lineR.SetPosition(1, hitPoint);

        Destroy(bulletTrailEffect, 0.05f);
        //Destroy(bulletTrailEffect, 0.5f); // 디버깅용,, 총알이 이상하게 날아가는것같다
    }

    public void Wait()
    {

    }

    // Set
    public void setAIType(int type)
    {
        aiType = type;
    }


}