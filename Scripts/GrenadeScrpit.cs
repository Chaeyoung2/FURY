using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeScrpit : MonoBehaviour
{
    GameObject pin, head, body;
    ParticleSystem ps;
    HingeJoint hinge;
    public int InHand;
    public float time;
    public bool explosion;
    public bool exani;

    // Start is called before the first frame update
    void Start()
    {
        ps.Pause();
    }
    private void Awake()
    {
        pin = GameObject.Find("safepin");
        body = GameObject.Find("Grenade");
        ps = GetComponent<ParticleSystem>();

        hinge  = pin.GetComponent<HingeJoint>();
        explosion = false;
        exani = false;
    }

    void OnJointBreak(float breakForce)
    {
        Debug.Log("핀이뽑혔다 다음과같은힘과함께, force: " + breakForce);

        explosion = true;
        time = Time.fixedTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (explosion == true)
        {
            
            if (Time.fixedTime - time > 3)
            {
                ps.Play();
                time= Time.fixedTime;
                exani = true;
            }
            if(Time.fixedTime - time >1&&exani==true)
            {
                ps.Stop();
                Debug.Log("애니끝");
                Object.DestroyImmediate(body);
            }
        }
    }
}
