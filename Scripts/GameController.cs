using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Photon.Pun;

public class GameController : MonoBehaviourPunCallbacks, IPunObservable
{
    private bool ObsOn = false;
    // 씬 전환시 플레이어 컨트롤러로부터 넘겨받는 전환직전 플레이어의 포지션
    public static Vector3 remainpos;

    public static bool exitPosition = false; // 씬3에서 씬1로의 씬 전환이 일어날때 전달 변수

    public static float remainRotationY = 0.0f; // 씬 전환 직전 넘겨받는 포대의 회전각도

    public GameObject playerPrefab; // 생성할 플레이어 캐릭터 프리팹

    // 활성, 비활성화할 오브젝트
    public GameObject Player;
    public GameObject PlayerCam;

    public GameObject Observer;
    public GameObject ObserverCam;

    public GameObject AtillCh;
    public GameObject AtillCam;

    public GameObject DummyBody;

    // 탱크 내부 연기,불 이펙트 관련 변수
    public GameObject flameEffect;
    public Transform flamePos1;
    public Transform flamePos2;
    public Transform flamePos3;
    public Transform flamePos4;

    public GameObject smokeEffect;
    public Transform smokePos1;
    public Transform smokePos2;
    public Transform smokePos3;

    private GameObject smoke1;
    private GameObject smoke2;
    private GameObject smoke3;

    private GameObject flame1;
    private GameObject flame2;
    private GameObject flame3;
    private GameObject flame4;

    public int health = 100;
    private bool flame = false;
    private bool smoke = false;
    void Start()
    {
        //// 다른 플레이어 생성 -> 나중에
        ///// 생성할 랜덤 위치 지정, 위치 y 값은 0으로
        //Vector3 randomSpawnPos = Random.insideUnitSphere * 5f;
        //randomSpawnPos.y = 0f;
        ///// 네트워크 상 모든 클라이언트에서 생성 실행
        ///// 해당 게임 오브젝트의 주도권은 생성 메서드를 직접 실행한 클라이언트에 있음
        //PhotonNetwork.Instantiate(playerPrefab.name, randomSpawnPos, Quaternion.identity);
    }

    void Update()
    {
        if (PlayerCtrl.Ap)
        {
            //AtillCh.gameObject.SetActive(true);
            //AtillCam.gameObject.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            health -= 35;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            health += 35;
        }
        /*
        if (PlayerCtrl.InTank == true)
        {
            remainpos = PlayerCtrl.pos;
        }
        else
        {
            remainRotationY = Ctrl1.rotationY;
        }
        
        if(Ctrl1.exitPosition == true)
        {
            remainRotationY = Ctrl1.rotationY;
        }
        */
        // 탱크 체력에 따른 이펙트 출력
        if (health < 70 && smoke == false)
        {
            smoke1 = (GameObject)Instantiate(smokeEffect, smokePos1.position, smokePos1.transform.rotation);
            smoke2 = (GameObject)Instantiate(smokeEffect, smokePos2.position, smokePos2.transform.rotation);
            smoke3 = (GameObject)Instantiate(smokeEffect, smokePos3.position, smokePos3.transform.rotation);
            smoke = true;
        }
        else if (health >= 70 && smoke == true)
        {
            Destroy(smoke1, smoke1.GetComponent<ParticleSystem>().duration);
            Destroy(smoke2, smoke2.GetComponent<ParticleSystem>().duration);
            Destroy(smoke3, smoke3.GetComponent<ParticleSystem>().duration);

            smoke = false;
        }
        if (health < 50 && flame == false)
        {
            flame1 = (GameObject)Instantiate(flameEffect, flamePos1.position, flamePos1.transform.rotation);
            flame2 = (GameObject)Instantiate(flameEffect, flamePos2.position, flamePos2.transform.rotation);

            flame3 = (GameObject)Instantiate(flameEffect, flamePos3.position, flamePos3.transform.rotation);
            flame4 = (GameObject)Instantiate(flameEffect, flamePos4.position, flamePos4.transform.rotation);

            flame = true;
        }
        else if (health >=50 && flame == true)
        {
            Destroy(flame1, flame1.GetComponent<ParticleSystem>().duration);
            Destroy(flame2, flame2.GetComponent<ParticleSystem>().duration);

            Destroy(flame3, flame3.GetComponent<ParticleSystem>().duration);
            Destroy(flame4, flame4.GetComponent<ParticleSystem>().duration);
            flame = false;
        }
    }

    
    //////////////////////
public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)// 주기적으로 자동 실행 되는 동기화 메서드
    {
        if (stream.IsWriting) // 로컬 오브젝트라면 쓰기 부분 실행
        {
            // 네트워크를 통해 ~~값 보내기
           // stream.SendNext(score);
        }
        else // 리모트 오브젝트라면 읽기 부분 실행
        {
            // 네트워크를 통해 ~~ 값 받기
            // score = (int)stream.ReceiveNext();
        }
    }

    public override void OnLeftRoom()     // 룸을 나갈 때 자동 실행 되는 메서드
    {
        // 룸을 나가면 로비 씬으로 돌아감
        //SceneManager.LoadScene("Lobby");
        
    }

}
