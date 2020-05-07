using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    public int damage = 20;

    public float speed = 4000.0f;

    private float ent_distance = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * speed);
    }

    void Update()
    {
        ent_distance += Time.deltaTime * speed;
        //Debug.Log(Time.deltaTime * speed);
        if (ent_distance >= 10000)
            Destroy(gameObject);
    }
}
