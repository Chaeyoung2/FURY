using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxFrameRate : MonoBehaviour
{
   void Awake()
    {
        Application.targetFrameRate = 40;
    }
}
