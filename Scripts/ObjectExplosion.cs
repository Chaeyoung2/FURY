using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectExplosion : MonoBehaviour
{
    public float cubeSize = 0.2f;
    public int cubesInRow = 10;
    float cubesPivotDistance;
    Vector3 cubesPivot;

    public GameObject ExplodeEffect;
    public GameObject DestroyEffect;

    public CameraShake cameraShake;

    // Start is called before the first frame update
    void Start()
    {
        cubesPivotDistance = cubeSize * cubesInRow / 2;
        cubesPivot = new Vector3(cubesPivotDistance, cubesPivotDistance, cubesPivotDistance);

        cameraShake = GameObject.Find("ObserverCam").GetComponent<CameraShake>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Tank")
        {
            //Debug.Log("OnTriggerEnter : collider is " + other.tag);
            // explode(); -> 폐기
            CreateDestroyEffect();
            StartCoroutine(cameraShake.Shake(0.15f, 0.1f)); 
            Destroy(gameObject, 0.5f);
        }
    }

    private void CreateDestroyEffect()
    {
        GameObject effect =
           (GameObject)Instantiate(DestroyEffect, transform.position, transform.rotation);
        Vector3 dir = new Vector3(0f, 3f, -3f);
        effect.transform.Translate(dir);
        Destroy(effect, 1.5f);
    }

    private void CreateExplosionEffect()
    {
        GameObject effect =
            (GameObject)Instantiate(ExplodeEffect, transform.position, transform.rotation);
        Vector3 dir = new Vector3(0f, 3f, -3f);
        effect.transform.Translate(dir);
        Destroy(effect, 2.5f);
    }

    public void explode()
    {
        //gameObject.SetActive(false);

        //for(int x = 0; x < cubesInRow; x++)
        //{
        //    for(int y = 0; y < cubesInRow; y++)
        //    {
        //        for(int z = 0; z < cubesInRow; z++)
        //        {
        //            CreatePiece(x, y, z);
        //        }
        //    }
        //}

        
        //Vector3 explosionPos = transform.position;
        //Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
        //foreach (Collider hit in colliders)
        //{
        //    Rigidbody rb = hit.GetComponent<Rigidbody>();
        //    if(rb != null)
        //    {
        //        rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpward);
        //    }
        //}
    }

    void CreatePiece(int x, int y, int z)
    {
        GameObject piece;
        piece = GameObject.CreatePrimitive(PrimitiveType.Cube);

        piece.transform.position = transform.position + new Vector3(cubeSize * x, cubeSize * y, cubeSize * z);
        piece.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);

        piece.AddComponent<Rigidbody>();
        piece.GetComponent<Rigidbody>().mass = cubeSize;

    }
}
