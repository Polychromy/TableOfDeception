using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScrollInfo : MonoBehaviour
{
    public TMP_Text text;
    public AudioSource scrollSlideAudioSource ;
    public AudioClip scrollSlideClip;


    public void CloseScroll()
    {
        Destroy(this.gameObject);
    }

    public void PlayScrollAudio()
    {
        scrollSlideAudioSource.PlayOneShot(scrollSlideClip);
    }
}
