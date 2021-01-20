using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScrollAnimation : MonoBehaviour
{
    public Animator playerout;
    public Animator playerin;
    
    public void AnimationPlayerout()
    {
        playerout.SetTrigger("Playerout");
    }

    public void AnimationPlayerIn()
    {
        playerin.SetTrigger("Playerin");
    }
}
