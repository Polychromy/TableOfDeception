using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialOnOff : MonoBehaviour
{
    public TMP_Text TutorialStateText;

    public bool GetTutorialState()
    {
        if (PlayerPrefs.HasKey("isTutorialEnabled"))
        {
            return PlayerPrefs.GetInt("isTutorialEnabled") == 1;
        }
        PlayerPrefs.SetInt("isTutorialEnabled", 1);
        return true;
    }

    public void ToggleTutorialState()
    {
        if (GetTutorialState())
        {
            PlayerPrefs.SetInt("isTutorialEnabled", 0);
        }
        else
        {
            PlayerPrefs.SetInt("isTutorialEnabled", 1);
        }

        SetTutorialStateText();
    }

    private void Awake()
    {
        SetTutorialStateText();
    }

    public void SetTutorialStateText()
    {
        if(GetTutorialState())
        {
            TutorialStateText.text = "Tutorial: enabled";
        }
        else
        {
            TutorialStateText.text = "Tutorial: disabled";
        }
        
    }

}
