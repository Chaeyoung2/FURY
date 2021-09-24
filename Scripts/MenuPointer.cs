using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPointer : MonoBehaviour
{
    public float m_defaultLength = 5f;
    public GameObject m_dot;
    //public OVRInput m_InputModule;
    private LineRenderer m_lineRenderer = null;

    // Start is called before the first frame update
    void Start()
    {
        m_lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLine();
    }

    private void UpdateLine()
    {
        float targetLength = m_defaultLength;

        RaycastHit hit = CreateRayCast(targetLength);

        Vector3 endPos = transform.position + (transform.forward * targetLength);

        if (hit.collider != null)
            endPos = hit.point;

        m_dot.transform.position = endPos;

        m_lineRenderer.SetPosition(0, transform.position);
        m_lineRenderer.SetPosition(1, endPos);
    }

    private RaycastHit CreateRayCast(float length)
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit, m_defaultLength);
        return hit;
    }
}
