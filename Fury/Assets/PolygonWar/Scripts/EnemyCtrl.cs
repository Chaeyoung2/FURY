using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCtrl : MonoBehaviour
{
    private Transform tr;

    public float moveSpeed = 1.0f; // 이동속도

    private float ent_distance = 0; // 총 이동거리

    private int Dir = -1;

    void Start()
    {
        tr = GetComponent<Transform>();
    }

    void OnCollisionEnter(Collision coll)
    {
        if(coll.collider.tag == "BULLET") // 총알 피격시 오브젝트 삭제
        {
            Destroy(coll.gameObject);
            Destroy(gameObject);
        }
    }

    void Update()
    {
        float moverange = Time.deltaTime * moveSpeed; // 초당 이동 거리
        tr.Translate(Vector3.forward * moverange);

        ent_distance += moverange;
        if (ent_distance >= 10.0f) // 총 이동거리가 10에 달하면 반대로 방향 전환
        {
            if(Dir == -1)
                tr.rotation = Quaternion.Euler(0, -90, 0);
            else
                tr.rotation = Quaternion.Euler(0, 90, 0);
            Dir *= -1;
            ent_distance = 0;
        }
    }
}
