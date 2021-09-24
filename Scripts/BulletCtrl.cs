using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    public int damage = 20;

    public float speed = 4000.0f;

    private float ent_distance = 0.0f;

    public GameObject flameEffect;

    private Transform collisionTr;

    // Start is called before the first frame update
    void Start()
    {
            GetComponent<Rigidbody>().AddForce(transform.forward * speed);

    }

    void Update()
    {
        ent_distance += Time.deltaTime * speed;

        if (ent_distance >= 10000)
            Destroy(gameObject);
    }

    void OnCollisionEnter(Collision coll)
    {
       // Debug.Log("OnCollisionEnter");
       // Debug.Log("coll.collider.tag : " + coll.collider.tag);
        if (coll.collider.tag == "MapObject") // 오브젝트 삭제
        {
            //Debug.Log("OnCollisionEnter - MapObject");
            Destroy(gameObject);
        }
        else if (coll.collider.tag == "enemy") // 오브젝트 삭제
        {
            //Debug.Log("OnCollisionEnter - enemy");
            collisionTr = coll.transform;
            CreateFlame();
            Destroy(gameObject);
        }
    }

    void CreateFlame()
    {
        GameObject flame = (GameObject)Instantiate(flameEffect, transform.position, transform.rotation);

        //Debug.Log("transform.position : " + transform.position.x + ", " + transform.position.y + ", " + transform.position.z);
        //Debug.Log("CreateFlame : " + flame);
        //Debug.Log("flame.GetComponent<ParticleSystem>().duration : " + flame.GetComponent<ParticleSystem>().duration);
        Destroy(flame, 3/*flame.GetComponent<ParticleSystem>().duration*/);
    }
}
