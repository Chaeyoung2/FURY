using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    private Transform tr;

    private float h = 0.0f;
    private float v = 0.0f;

    Animator animator;

    private bool IsWalking = false;

    public float moveSpeed = 1.0f;
    public float rotSpeed = 200.0f;

    public static float xpos = 0.0f;
    public static float ypos = 1.0f;
    public static float zpos = 0.0f;

    public static int CamNum = 0; // 씬 전환시 바라보는 카메라를 결정하는 변수
    public static bool InTank = true;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        if(GameController.exitPosition == true)
        {
            InTank = true;

            xpos = GameController.remainXpos;
            ypos = GameController.remainYpos;
            zpos = GameController.remainZpos;
        }
        
        tr = GetComponent<Transform>();
        tr.position = new Vector3(xpos, ypos, zpos);
    }

    void OnTriggerStay(Collider col)
    {
        if(col.gameObject.tag == "WAY1")
        {
            Debug.Log("Access Artillery");
            if (Input.GetKeyDown(KeyCode.Space))
            {
                InTank = false;
                CamNum = 1;
                SceneManager.LoadScene("TestScene3");
            }
        }

        if(col.gameObject.tag == "WAY2")
        {
            Debug.Log("Access Observer");
            if (Input.GetKeyDown(KeyCode.Space))
            {
                InTank = false;
                CamNum = 2;
                SceneManager.LoadScene("TestScene3");
            }
        }
    }
    
    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        if(v != 0)
        {
            IsWalking = true;
        }
        else
        {
            IsWalking = false;
        }

        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        tr.Translate(moveDir * Time.deltaTime * moveSpeed, Space.Self);

        tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X"));

        xpos = tr.position.x;
        ypos = tr.position.y;
        zpos = tr.position.z;

        AnimationUpdate();
    }

    void AnimationUpdate()
    {
        if(IsWalking == true)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
    }
}
