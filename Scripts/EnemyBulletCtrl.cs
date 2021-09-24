using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletCtrl : MonoBehaviour
{
    public int damage = 20;

    public float speed = 4000.0f;

    private float ent_distance = 0.0f;

    public GameObject bullet3CollEffect;

    public int type = 0; //1:enemy1, 2:enemy2

    public Transform tankTr;

    // Start is called before the first frame update
    void Start()
    {
        tankTr = GameObject.Find("PlayerTank").GetComponent<Transform>();
        if (type == 1) // enemy1의 bullet일 경우
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * speed);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (type == 1) // enemy1의 bullet일 경우
        {
            ent_distance += Time.deltaTime * speed;

            if (ent_distance >= 10000)
                Destroy(gameObject);
        }
        else if (type == 2)// enemy2의 bullet일 경우
        {
            //Debug.Log("ent_distance :" + ent_distance);
            //Debug.Log("tankTr : " + tankTr.position.x + ", " + tankTr.position.y + ", " + tankTr.position.z);
            transform.LookAt(tankTr);
            ent_distance += Time.deltaTime;
            if (ent_distance >= 2)
                Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        if (type == 2)
        {
            //Debug.Log("OnCollisionEnter");
            if (coll.collider.tag == "Terrain") // 오브젝트 삭제
            {
                //Debug.Log("coll.collider.tag : " + coll.collider.tag);
                CreateFlame();
                Destroy(gameObject);
            }
        }
    }
    void CreateFlame()
    {
        GameObject flame = (GameObject)Instantiate(bullet3CollEffect, transform.position, transform.rotation);

        //Debug.Log("transform.position : " + transform.position.x + ", " + transform.position.y + ", " + transform.position.z);
        //Debug.Log("CreateFlame : " + flame);
        //Debug.Log("flame.GetComponent<ParticleSystem>().duration : " + flame.GetComponent<ParticleSystem>().duration);
        Destroy(flame, 3/*flame.GetComponent<ParticleSystem>().duration*/);
    }
}
