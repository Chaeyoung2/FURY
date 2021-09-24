using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.VR;

public class VRAnimatorController : MonoBehaviour
{
    public float speedTreshold = 0.1f;
    [Range(0, 1)]
    public float smoothing = 1;

    private Transform tr;
    private Animator animator;
    private Vector3 previousPos;
    private VRRig vrRig;

    private bool StopHandTracking = false;

    Transform ch_rail01, ch_rail02, ch_rail03;
    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        vrRig = GetComponent<VRRig>();
        previousPos = vrRig.head.vrTarget.position;

        ch_rail01 = GameObject.Find("상호작용지점").transform.Find("ch_rail01");
        ch_rail02 = GameObject.Find("상호작용지점").transform.Find("ch_rail02");
        ch_rail03 = GameObject.Find("상호작용지점").transform.Find("ch_rail03");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 headsetSpeed = (vrRig.head.vrTarget.position - previousPos) / Time.deltaTime;
        headsetSpeed.y = 0;

        Vector3 headsetLocalSpeed = transform.InverseTransformDirection(headsetSpeed);
        previousPos = vrRig.head.vrTarget.position;

        float previousDirectionX = animator.GetFloat("DirectionX");
        float previousDirectionY = animator.GetFloat("DirectionY");

        animator.SetBool("isMoving", headsetLocalSpeed.magnitude > speedTreshold);
        animator.SetFloat("DirectionX", Mathf.Lerp(previousDirectionX, Mathf.Clamp(headsetLocalSpeed.x, -1, 1), smoothing));
        animator.SetFloat("DirectionY", Mathf.Lerp(previousDirectionY, Mathf.Clamp(headsetLocalSpeed.z, -1, 1), smoothing));

        if (!PlayerCtrl.OnControll)
        {
            if (!StopHandTracking)
            {
                vrRig.enabled = false;
                gameObject.GetComponent<VRFootIK>().enabled = false;
                gameObject.GetComponent<RigBuilder>().enabled = false;
                StopHandTracking = true;

                if (PlayerCtrl.UpFloor)
                {
                    tr.position = new Vector3(ch_rail03.position.x, ch_rail03.position.y, ch_rail03.position.z);
                    tr.rotation = Quaternion.Euler(0, -5, 0);
                }
                if (PlayerCtrl.DownFloor)
                {
                    tr.position = new Vector3(ch_rail02.position.x, ch_rail02.position.y, ch_rail02.position.z);
                    tr.rotation = Quaternion.Euler(0, -5, 0);
                }
            }

            if (PlayerCtrl.UpFloor)
            {
                if (PlayerCtrl.IsCliming)
                {
                    animator.SetBool("isCliming", true);
                    tr.position = Vector3.MoveTowards(tr.position, ch_rail01.position, 2 * Time.deltaTime);
                }
                else
                {
                    animator.SetBool("isCliming", false);
                    animator.Update(Time.deltaTime);
                    tr.position = Vector3.MoveTowards(tr.position, ch_rail02.position, Time.deltaTime);
                }
            }

            if (PlayerCtrl.DownFloor)
            {
                if (!PlayerCtrl.IsCliming)
                {
                    animator.SetBool("isCliming", false);
                    tr.position = Vector3.MoveTowards(tr.position, ch_rail01.position, Time.deltaTime);
                }
                else
                {
                    animator.SetBool("isCliming", true);
                    animator.Update(Time.deltaTime);
                    tr.position = Vector3.MoveTowards(tr.position, ch_rail03.position, 2 * Time.deltaTime);
                }
            }
        }
        else
        {
            if (StopHandTracking)
            {
                animator.SetBool("isCliming", false);
                vrRig.enabled = true;
                gameObject.GetComponent<VRFootIK>().enabled = true;
                gameObject.GetComponent<RigBuilder>().enabled = true;
                StopHandTracking = false;
            }
        }
    }
}