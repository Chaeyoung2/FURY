using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TitleTextEffect : MonoBehaviour
{
    Text flashingText;

    // Start is called before the first frame update
    void Start()
    {
        flashingText = GetComponent<Text>();
        StartCoroutine(BlinkText());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator BlinkText()
    {
        while (true)
        {
            flashingText.text = "";
            yield return new WaitForSeconds(.5f);
            flashingText.text = "FURY";
            yield return new WaitForSeconds(.5f);
        }
    }
}
