using UnityEngine;

public class PhasetextAnimation : MonoBehaviour
{
    public Animator sunrisephase;
    public Animator dayphase;
    public Animator nightphase;

    public void AnimationSunrisephase()
    {
        sunrisephase.SetTrigger("Sunrisephase");
    }

    public void AnimationDayphase()
    {
        dayphase.SetTrigger("Dayphase");
    }

    public void AnimationNightphase()
    {
        nightphase.SetTrigger("Nightphase");
    }
}
