using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class SoundOnOff : MonoBehaviour
{
    Toggle soundToggle;

    // Start is called before the first frame update
    void Start()
    {
        soundToggle = GetComponent<Toggle>();
        if (AudioListener.volume == 0)
        {
            soundToggle.isOn = false;
        }
    }

    public void switchOnOffSound(bool soundOn)
    {
        if (soundOn)
        {
            AudioListener.volume = 1;
        }
        else
        {
            AudioListener.volume = 0;
        }
    }
}
