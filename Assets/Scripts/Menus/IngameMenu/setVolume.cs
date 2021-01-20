using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class setVolume : MonoBehaviour
{
    public AudioMixer mixer;

    public void setMasterVol(float sliderValue)
    {
        mixer.SetFloat("MasterVol", Mathf.Log10(sliderValue) * 20);
    }
    public void setMusicVol(float sliderValue)
    {
        mixer.SetFloat ("MusicVol", Mathf.Log10 (sliderValue) * 20);
    }

    public void setButtonVol(float sliderValue)
    {
        mixer.SetFloat ("ButtonVol", Mathf.Log10 (sliderValue) * 20);
    }

    public void setChatVol(float sliderValue)
    {
        mixer.SetFloat ("ChatVol", Mathf.Log10 (sliderValue) * 20);
    }
}

