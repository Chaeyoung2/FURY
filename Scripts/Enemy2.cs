//Enemy2.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : LivingObject
{
    private Animator anim;
    public LineRenderer bulletTrail;

    private Transform target;
    private LivingObject targetObject; // 추적할 대상
    public LayerMask whatIsTarget; // 추적 대상 레이어

    public GameObject rocketPrefab;
    public Transform fireTr; // 총알 발사 위치
    public ParticleSystem muzzleFlashEffect; // 총구 화염 효과
    public float speedForce;

    private float AttackDist; // 이 범위 안에 들어오면 공격
    private float FollowRad; // 이 범위 안에 들어오면 어그로
    [SerializeField] private float lastFireTime; // 마지막 총 발사 후 지난 시간
    [SerializeField] private float betFireTime; // 총 발사 간격
    [SerializeField] private float damage; // 공격력
    [SerializeField] private float fireDistance; // 사정거리


    private float lifeTime = 0f; // 1분 이상 지나면 사라지도록
    private float lifeMaxTime = 60f;
    private bool enemyDead = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        target = GameObject.FindWithTag("Tank").GetComponent<Transform>();

        health = 30f;
        damage = 10;
        fireDistance = 300;
        betFireTime = Random.Range(3.5f, 4.5f);
        lastFireTime = Random.Range(0.5f, 3.5f);
        FollowRad = 150f;
        AttackDist = 20f;
        speedForce = 200f;
        lifeMaxTime = 60f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!enemyDead)
        {
            FollowTank();
            lifeTime += Time.deltaTime;
            if (lifeTime >= lifeMaxTime)
            {
                anim.SetTrigger("Die");
                enemyDead = true;
            }
        }
        else
        {
            Destroy(gameObject, 5f);
        }
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

    private void Fire()
    {
        anim.SetBool("Fire", true);
        lastFireTime += Time.deltaTime;
        if (lastFireTime >= betFireTime)
        {
            lastFireTime = 0;

            // 로켓 발사
            GameObject rocket = Instantiate(rocketPrefab, fireTr.position, transform.rotation, transform);
            //rocket.transform.LookAt(target);

            Rigidbody rb = rocket.GetComponent<Rigidbody>();
            //            rb.AddForce(Vector3.forward * speedForce, ForceMode.Impulse);
            Vector3 targetCenter = target.position;
            targetCenter.y += 3f;
            Vector3 dir = targetCenter - fireTr.position;
            rb.AddForceAtPosition(dir.normalized * speedForce, targetCenter);
            // 발사 이펙트 재생 
            //muzzleFlashEffect.Play(); // 총구 화염
            //SpawnBulletTrail(hitPosition);// Bullet Trail
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
        Destroy(gameObject, 10);
    }

    void FollowTank()
    {
        if (hasTarget)
        {
            float dist = Vector3.Distance(transform.position, target.position);
            //Debug.Log("dist : " + dist);
            transform.LookAt(target);
            transform.Rotate(new Vector3(0, 1, 0), 50f);
            if (dist > AttackDist)
            {
                Fire();
            }
            else
            {
                anim.SetBool("Fire", false);
            }
        }
        else // 추적 대상 없음 
        {
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
}