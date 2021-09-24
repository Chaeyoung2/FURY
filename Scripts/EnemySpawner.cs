//EnemySpawner.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//

public class EnemySpawner : MonoBehaviour
{
    public Enemy enemyPrefab; // 생성할 적 AI
    public Enemy2 enemyPrefab2; // 생성할 적 AI
    public Enemy3 enemyPrefab3; // 생성할 적 AI
    public Enemy4 enemyPrefab4; // 생성할 적 AI
    public Transform[] spawnPoints; // 적 AI를 소환할 위치들

    public float speedMax = 3f; // 최대 속도
    public float speedMin = 1f; // 최소 속도

    private List<LivingObject> enemies = new List<LivingObject>(); // 생성된 적들을 담는 리스트

    private int enemyCount = 3; // 남은 적의 수
    private int wave = 0; // 현재 웨이브


    // Start is called before the first frame update
    void Start()
    {
        // 적의 세기를 0%에서 100% 사이에서 랜덤 결정
        float enemyIntensity = Random.Range(0f, 1f);
        // 적 생성 처리 실행 // start에서는 첫번째 웨이브 -> 이제 WayPointGroup이 호출해줌.
        // CreateEnemy(0);
    }

    // Update is called once per frame
    void Update()
    {
        // 적을 모두 물리친 경우 다음 스폰 실행
        //if (enemies.Count <= 0)
        //{   
        //    // 웨이브 1 증가
        //    wave++;
        //    SpawnWave();
        //}

        // UI 갱신
        //UpdateUI();
    }

    private void SpawnWave()
    {
        //// 현재 웨이브 * 1.5에 반올림 한 개수 만큼 적을 생성
        //int spawnCount = Mathf.RoundToInt(wave * 1.5f);

        //// spawnCount 만큼 적을 생성
        //for (int i = 0; i < spawnCount; i++)
        //{
        //    // 적의 세기를 0%에서 100% 사이에서 랜덤 결정
        //    float enemyIntensity = Random.Range(0f, 1f);
        //    // 적 생성 처리 실행
        //    CreateEnemy(2);
        //}
    }

    // 적을 생성하고 생성한 적에게 추적할 대상을 할당
    public void CreateEnemy(int wave)
    {
        // 생성할 위치를 랜덤으로 결정
        // Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        float xPos = 0;
        float yPos = 0;
        float zPos = 0;
        float angle = Random.Range(-60f, 60f);
        Quaternion qRot = Quaternion.Euler(0f, angle, 0f);
        switch (wave)
        {
            case 1: // wave1 //로켓런처(Enemy2)
                {
                    xPos = 243.7f;
                    yPos = 4.2f;
                    zPos = 1997.4f;
                    Enemy2 createdEnemy = Instantiate(enemyPrefab2, new Vector3(xPos, yPos, zPos), Quaternion.identity);
                    xPos = 246.11f;
                    yPos = 7.88f;
                    zPos = 1997.25f;
                    createdEnemy = Instantiate(enemyPrefab2, new Vector3(xPos, yPos, zPos), Quaternion.identity);
                    xPos = 240.81f;
                    yPos = 0.06f;
                    zPos = 1998.23f;
                    createdEnemy = Instantiate(enemyPrefab2, new Vector3(xPos, yPos, zPos), Quaternion.identity);
                }
                break;
            case 2: //wave2 //1:어그로공격
                {
                    for (int i = 0; i < 5; ++i)
                    {
                        xPos = Random.Range(-40f, -58.5f);
                        zPos = Random.Range(1989.7f, 1964.8f);
                        Enemy createdEnemy = Instantiate(enemyPrefab, new Vector3(xPos, yPos, zPos), qRot);
                        createdEnemy.setAIType(0);
                        //Debug.Log("Create Enemy");
                    }
                    for (int i = 0; i < 2; ++i)
                    {
                        xPos = Random.Range(-40f, -58.5f);
                        zPos = Random.Range(1989.7f, 1964.8f);
                        Enemy3 createdEnemy3 = Instantiate(enemyPrefab3, new Vector3(xPos, yPos, zPos), qRot);
                        createdEnemy3.setGroup(0);
                        //Debug.Log("Create Enemy");
                    }
                }
                break;
            case 3: // wave3 //3:배회
                {
                    for (int i = 0; i < 3; ++i)
                    {
                        xPos = Random.Range(-108.3f, -79.5f);
                        zPos = Random.Range(2030.1f, 2011.4f);
                        Enemy3 createdEnemy3 = Instantiate(enemyPrefab3, new Vector3(xPos, yPos, zPos), qRot);
                        createdEnemy3.setGroup(0);
                        //Debug.Log("Create Enemy");
                    }
                    for (int i = 0; i < 2; ++i)
                    {
                        xPos = Random.Range(-140f, -121.1f);
                        zPos = Random.Range(2030.1f, 1993.2f);
                        Enemy3 createdEnemy3 = Instantiate(enemyPrefab3, new Vector3(xPos, yPos, zPos), qRot);
                        createdEnemy3.setGroup(0);
                        //Debug.Log("Create Enemy");
                    }
                    for (int i = 0; i < 1; ++i)
                    {
                        xPos = Random.Range(-140f, -127.2f);
                        zPos = Random.Range(2040.6f, 2031.7f);
                        Enemy3 createdEnemy3 = Instantiate(enemyPrefab3, new Vector3(xPos, yPos, zPos), qRot);
                        createdEnemy3.setGroup(0);
                        //Debug.Log("Create Enemy");
                    }
                }
                break;
            case 4: //wave4 //1:어그로
                {
                    for (int i = 0; i < 5; ++i)
                    {
                        xPos = Random.Range(-303.1f, -287.6f);
                        zPos = Random.Range(2014.9f, 1991.2f);
                        Enemy createdEnemy = Instantiate(enemyPrefab, new Vector3(xPos, yPos, zPos), qRot);
                        createdEnemy.setAIType(0);
                        //Debug.Log("Create Enemy");
                    }
                }
                break;
            case 5: // wave5 //4:전차
                {
                    //xPos = Random.Range(-303.1f, -287.6f);
                    //zPos = Random.Range(2014.9f, 1991.2f);
                    //Enemy4 createdEnemy = Instantiate(enemyPrefab4, new Vector3(xPos, yPos, zPos), qRot);
                    ////Debug.Log("Create Enemy");
                }
                break;
            case 7: // wave7 // 3배회, 1어그로
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        xPos = Random.Range(-191.5f, -176.6f);
                        zPos = Random.Range(2103f, 2061.6f);
                        Enemy createdEnemy = Instantiate(enemyPrefab, new Vector3(xPos, yPos, zPos), qRot);
                        createdEnemy.setAIType(0);
                        //Debug.Log("Create Enemy");
                    }
                }
                break;
            case 8: // wave8 // 1어그로
                {
                    for (int i = 0; i < 5; ++i)
                    {
                        xPos = Random.Range(87.2f, 113.8f);
                        zPos = Random.Range(2063.1f, 2036.7f);
                        Enemy createdEnemy = Instantiate(enemyPrefab, new Vector3(xPos, yPos, zPos), qRot);
                        createdEnemy.setAIType(0);
                        //Debug.Log("Create Enemy");
                    }
                    for (int i = 0; i < 2; ++i)
                    {
                        xPos = Random.Range(51.9f, 60.4f);
                        zPos = Random.Range(2095.8f, 2069.9f);
                        Enemy3 createdEnemy3 = Instantiate(enemyPrefab3, new Vector3(xPos, yPos, zPos), qRot);
                        createdEnemy3.setGroup(0);
                        //Debug.Log("Create Enemy");
                    }
                }
                break;
            case 9: // wave9 // 1어그로
                {
                    for (int i = 0; i < 3; ++i)
                    {
                        xPos = Random.Range(260f, 291f);
                        zPos = Random.Range(2076.7f, 2031.7f);
                        Enemy createdEnemy = Instantiate(enemyPrefab, new Vector3(xPos, yPos, zPos), qRot);
                        //Debug.Log("Create Enemy");
                    }
                }
                break;
            case 10: // wave10 // 2로켓런처
                {
                    xPos = 402.8f;
                    yPos = 1f;
                    zPos = 2161.48f;
                    Enemy2 createdEnemy = Instantiate(enemyPrefab2, new Vector3(xPos, yPos, zPos), Quaternion.identity);
                    xPos = 412.63f;
                    yPos = 1f;
                    zPos = 2161.48f;
                    createdEnemy = Instantiate(enemyPrefab2, new Vector3(xPos, yPos, zPos), Quaternion.identity);
                    xPos = 406.98f;
                    yPos = 4.3f;
                    zPos = 2161.48f;
                    createdEnemy = Instantiate(enemyPrefab2, new Vector3(xPos, yPos, zPos), Quaternion.identity);
                }
                break;
            case 11: // wave11 // 1어그로
                {
                    for (int i = 0; i < 2; ++i)
                    {
                        xPos = Random.Range(419.9f, 401f);
                        zPos = Random.Range(2186.2f, 2178.4f);
                        Enemy createdEnemy = Instantiate(enemyPrefab, new Vector3(xPos, yPos, zPos), qRot);
                        //Debug.Log("Create Enemy");
                    }
                    for (int i = 0; i < 4; ++i)
                    {
                        xPos = Random.Range(377f, 397.3f);
                        zPos = Random.Range(2209.9f, 2190.2f);
                        Enemy createdEnemy = Instantiate(enemyPrefab, new Vector3(xPos, yPos, zPos), qRot);
                        //Debug.Log("Create Enemy");
                    }
                }
                break;
            case 12: // wave12 // 2로켓런처
                {
                    xPos = 83.88f;
                    yPos = 0.3f;
                    zPos = 2217.1f;
                    Enemy2 createdEnemy = Instantiate(enemyPrefab2, new Vector3(xPos, yPos, zPos), Quaternion.identity);
                    xPos = 83.88f;
                    yPos = 3.38f;
                    zPos = 2220.08f;
                    createdEnemy = Instantiate(enemyPrefab2, new Vector3(xPos, yPos, zPos), Quaternion.identity);
                    xPos = 83.88f;
                    yPos = 7f;
                    zPos = 2224.96f;
                    createdEnemy = Instantiate(enemyPrefab2, new Vector3(xPos, yPos, zPos), Quaternion.identity);
                }
                break;
            case 13: // wave13 // 4전차
                {
                    //xPos = Random.Range(-303.1f, -287.6f);
                    //zPos = Random.Range(2014.9f, 1991.2f);
                    //Enemy4 createdEnemy = Instantiate(enemyPrefab4, new Vector3(xPos, yPos, zPos), qRot);
                    ////Debug.Log("Create Enemy");
                }
                break;
            case 14: // wave14 // 1어그로 3배회
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        xPos = Random.Range(-156.2f, -119.9f);
                        zPos = Random.Range(2289.6f, 2265.9f);
                        Enemy createdEnemy = Instantiate(enemyPrefab, new Vector3(xPos, yPos, zPos), qRot);
                        //Debug.Log("Create Enemy");
                    }
                    for (int i = 0; i < 2; ++i)
                    {
                        xPos = Random.Range(-138.6f, -131.1f);
                        zPos = Random.Range(2388.3f, 2368.4f);
                        Enemy createdEnemy = Instantiate(enemyPrefab, new Vector3(xPos, yPos, zPos), qRot);
                        //Debug.Log("Create Enemy");
                    }
                }
                break;
            case 15: // wave15 // 3배회
                {
                    for (int i = 0; i < 3; ++i)
                    {
                        xPos = Random.Range(-402.6f, -339f);
                        zPos = Random.Range(2358.5f, 2308f);
                        Enemy3 createdEnemy3 = Instantiate(enemyPrefab3, new Vector3(xPos, yPos, zPos), qRot);
                        createdEnemy3.setGroup(0);
                        //Debug.Log("Create Enemy");
                    }
                }
                break;
            case 16: // wave16 // 1어그로 3배회
                {
                    for (int i = 0; i < 3; ++i)
                    {
                        xPos = Random.Range(-436f, -485.3f);
                        zPos = Random.Range(2442.1f, 2420.6f);
                        Enemy createdEnemy = Instantiate(enemyPrefab, new Vector3(xPos, yPos, zPos), qRot);
                        //Debug.Log("Create Enemy");
                    }
                }
                break;
        }


    }
}