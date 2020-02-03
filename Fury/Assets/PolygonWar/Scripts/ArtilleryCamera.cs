using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryCamera : MonoBehaviour
{
    public Transform targetTr;

    public float dist = 0.0f;

    public float xSpeed = 220.0f;
    public float ySpeed = 100.0f;

    private float x = 0.0f;
    private float y = 0.0f;

    public float yMinLimit = -10.0f;
    public float yMaxLimit = -2.0f;

    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Vector3 angles = transform.eulerAngles;

        x = angles.y;
        y = angles.x;
    }

    // Update is called once per frame
    void Update()
    {
        x += Input.GetAxis("Mouse X") * xSpeed * 0.015f;
        y -= Input.GetAxis("Mouse Y") * ySpeed * 0.015f;

        y = ClampAngle(y, yMinLimit, yMaxLimit);

        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 position = rotation * new Vector3(0, 0.0f, -dist) + targetTr.position + new Vector3(0.0f, 0, 0.0f);

        transform.rotation = rotation;
        targetTr.rotation = Quaternion.Euler(0, x, 0);
        transform.position = position;
    }
}
