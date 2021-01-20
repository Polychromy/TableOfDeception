using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public Image banner;
    public TMP_Text text;
    public AudioSource tutoialInOutSound;
   

    public void DisplayTutorial(string info)
    {
        GetComponent<Animator>().StopPlayback();
        text.text = info;
        GetComponent<Animator>().Play("TutorialIn");
    }

    public void HideTutorial()
    {
        GetComponent<Animator>().StopPlayback();
        GetComponent<Animator>().Play("TutorialOut");
        text.text = "";
    }

    // public void Update()
    // {
    //     if (tester)
    //     {
    //         DisplayTutorial("TEst");
    //     }
    //     else
    //     {
    //         HideTutorial();
    //     }
    // }
}
