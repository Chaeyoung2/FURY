using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTankHealth : LivingObject
{
    public Slider healthSlider; // 체력을 표시할 UI 슬라이더
                                //private Animator playetankrAnimator; // 탱크 애니메이터? 필요할진 모르겠는데
                                //private PlayerTankMovement playertankMovement; // 탱크 움직임 컴포넌트

    public CameraShake cameraShake;
    public GameObject HitEffect;

    private void Start()
    {
        cameraShake = GameObject.Find("ObserverCam").GetComponent<CameraShake>();
    }
    private void Awake()
    {
        //playetankrAnimator = GetComponent<Animator>();
        //playertankMovement = GetComponent<PlayerTankMovement>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "EnemyBullet")
        {
            CreateDestroyEffect(collision.gameObject.transform);
            Destroy(collision.gameObject);
            StartCoroutine(cameraShake.Shake(0.15f, 0.1f));
        }
    }

    private void CreateDestroyEffect(Transform hitTr)
    {
        GameObject effect =
           (GameObject)Instantiate(HitEffect, hitTr.position, hitTr.rotation);
        effect.transform.localScale += new Vector3(2f, 2f, 2f);
        Destroy(effect, 1.5f);
    }

    protected override void OnEnable()
    {
        base.OnEnable(); // LivingEntity의 OnEnable() 실행 (상태 초기화)

        startingHealth = 500; // 기본 체력 값 설정
        health = startingHealth;

        //healthSlider.gameObject.SetActive(true); // 체력 슬라이더 활성화
        //healthSlider.maxValue = startingHealth; // 체력 슬라이더의 최대값을 기본 체력값으로 변경
        //healthSlider.value = health; // 체력 슬라이더의 값을 현재 체력값으로 변경

        //playertankMovement.enabled = true; // 플레이어 조작을 받는 컴포넌트들 활성화

    }

    // 체력 회복
    public override void OnDamage(float damage, Vector3 hitPoint,
        Vector3 hitDirection)
    {
        // LivingEntity의 OnDamage() 실행(데미지 적용)
        base.OnDamage(damage, hitPoint, hitDirection);
        // 1갱신된 체력을 체력 슬라이더에 반영
        //healthSlider.value = health;
        // 2UI로 표시 -> VR UI로 변경
            // UIManager.instance.UpdatePlayerTankHPText(health);
        //Debug.Log("Tank Health: " + health);
    }

    // 데미지 처리
    public override void RestoreHealth(float newHealth)
    {
        // LivingEntity의 RestoreHealth() 실행 (체력 증가)
        base.RestoreHealth(newHealth);
        // 체력 갱신
        //healthSlider.value = health;
    }
    public override void Die()
    {
        // 부모 클래스의 Die() 실행(사망 적용)
        base.Die();

        //// 체력 슬라이더 비활성화
        //healthSlider.gameObject.SetActive(false);

        //// 애니메이터의 Die 트리거를 발동시켜 사망 애니메이션 재생
        //playerAnimator.SetTrigger("Die");

        //// 플레이어 조작을 받는 컴포넌트들 비활성화
        //playerMovement.enabled = false;

        //// 5초 뒤에 리스폰
        //Invoke("Respawn", 5f);
    }

    private void OnTriggerStay(Collider other)
    {
    }
}
