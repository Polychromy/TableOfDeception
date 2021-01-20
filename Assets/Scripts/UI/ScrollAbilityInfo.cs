using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScrollAbilityInfo : MonoBehaviour
{
    public TMP_Text text;
    public TMP_Text abilityName;
    public SpriteRenderer sprite;

    public void CloseScroll()
    {
        Destroy(this.gameObject);
    }
}
