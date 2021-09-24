//BuildingDestroyedEffect.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDestroyedEffect : MonoBehaviour
{
    public GameObject DestroyEffect;
    public GameObject DestroyedBuilding;
    public int type = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnTriggerEnter");
        if (other.gameObject.tag == "BULLET")
        {
            //Debug.Log("OnTriggerEnter : BULLET");
            CreateDestroyEffect();
            CreateDestroyedBuilding();
            Destroy(gameObject, 0.5f);
        }
    }

    private void CreateDestroyEffect()
    {
        GameObject effect =
           (GameObject)Instantiate(DestroyEffect, transform.position, transform.rotation);
        Vector3 dir = new Vector3(0f, 3f, -3f);
        effect.transform.Translate(dir);
        Destroy(effect, 2.5f);
    }

    private void CreateDestroyedBuilding()
    {
        GameObject destroyedBuilding = Instantiate(DestroyedBuilding, transform.position, transform.rotation);
        destroyedBuilding.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
        if(type == 2)
        {
            //transform.Rotate(new Vector3(0f, 1f, 0f), 180f);
        }
    }
}
