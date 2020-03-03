using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager m_instance;
    public bool Connected;


    // 싱글톤 접근용 프로퍼티
    public static UIManager instance
    {
        get
        {
            if(m_instance == null)
            {
                m_instance = FindObjectOfType<UIManager>();
            }
            return m_instance;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        Connected = false;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
