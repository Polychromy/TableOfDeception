using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerSlotScript : MonoBehaviour
{
    public Boolean active = false;
    public Boolean isInverted = false;
    public uint id;
    public TMP_Text playername;
    public SpriteRenderer playericon;
    public SpriteRenderer statusicon;
    public GameObject badge_death;
    public SpriteRenderer[] vote;
    public GameObject votescontainer;
    public int voteCounter;
    public Button button;
    public GameObject highlite;
}
