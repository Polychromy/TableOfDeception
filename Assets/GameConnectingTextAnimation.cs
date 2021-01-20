using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameConnectingTextAnimation : MonoBehaviour
{
    private uint textDotCounter = 0;
    TMP_Text text;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
        StartCoroutine(TextAnimation());
    }

    private IEnumerator TextAnimation()
    {
        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(1);
            if(textDotCounter == 2)
            {
                text.text = "Connecting";
                textDotCounter = 0;
            }
            else
            {
                text.text += ".";
            }
        }

    }
}
