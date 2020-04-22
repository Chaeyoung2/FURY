using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCtrl : MonoBehaviour
{
    public GameObject bullet;
    public GameObject flameEffect;
    public Transform firePos;
    public Transform flamePos;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }

    void Fire()
    {
        CreateBullet();
        CreateFlame();
    }

    void CreateBullet()
    {
        Instantiate(bullet, firePos.position, firePos.rotation);
    }

    void CreateFlame()
    {
        GameObject flame = (GameObject)Instantiate(flameEffect, flamePos.position, flamePos.transform.rotation);
        Destroy(flame, flame.GetComponent<ParticleSystem>().duration);
    }
}
