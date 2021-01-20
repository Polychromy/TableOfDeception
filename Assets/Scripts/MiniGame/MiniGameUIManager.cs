using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;


public class MiniGameUIManager : MonoBehaviour
{

    protected MiniGameManager minigameManager;
    private GameManager gameManager;
    
    protected MiniGameManager MiniGameManager
    {
        get
        {
            if (minigameManager != null) { return minigameManager; }
            return minigameManager = FindObjectOfType<MiniGameManager>();
        }
    }

    public GameManager GameManager
    {
        get
        {
            if (gameManager != null) { return gameManager; }
            return gameManager = FindObjectOfType<GameManager>();
        }
    }

    [Header("Player Abilities")]
    [SerializeField] private TMP_Text nameTraitorText;
    [SerializeField] private TMP_Text nameKnightText;

    [Header("Player Abilities")]
    [SerializeField] private GameObject AbilityBox;
    [SerializeField] private TMP_Text[] abilityTexts;
    [SerializeField] private TMP_Text abilityDescReflectText;
    [SerializeField] private TMP_Text abilityDescSplitText;
    [SerializeField] private TMP_Text abilityDescNoEffectText;

    [Header("Healthbars")]
    [SerializeField] private GameObject healthBarKnight;
    [SerializeField] private TMP_Text healthBarValueKnight;
    [SerializeField] private GameObject healthBarTraitor;
    [SerializeField] private TMP_Text healthBarValueTraitor;

    [Header("Viewer Description")]
    [SerializeField] private GameObject FightInformationBox;
    [SerializeField] private TMP_Text FightInformationDescription;

    [Header("Animations")]
    [SerializeField] private GameObject roundFightAnimation;
    [SerializeField] private GameObject damageDealtToKnight;
    [SerializeField] private GameObject damageDealtToTraitor;
    [SerializeField] private GameObject fightWinAnimation;
    public AudioSource AnimationSounds;
    public AudioClip HealthLossSound;
    public AudioClip MiniGameEndSound;
    public AudioClip MiniGameStartSound;

    [Header("Footer")] 
    [SerializeField] private TMP_Text footerCountdownTimer;
    
    #region Fighter

    public void SetKnightSelf()
    {
        nameKnightText.text = "YOU";
        nameKnightText.color = Color.yellow;
    }

    public void SetKnightOther()
    {
        nameKnightText.text = "KNIGHT";
        nameKnightText.color = Color.white;
    }

    public void SetTraitorSelf()
    {
        nameTraitorText.text = "YOU";
        nameTraitorText.color = Color.yellow;
    }

    public void SetKnightName(string name)
    {
        nameKnightText.text = name;
        nameKnightText.color = Color.red;
    }

    public void SetTraitorName(string name)
    {
        nameTraitorText.text = name;
        nameTraitorText.color = Color.red;
    }

    public void SetTraitorOther()
    {
        nameTraitorText.text = "TRAITOR";
        nameTraitorText.color = Color.white;
    }

    public void SetAbilitiesKnight()
    {
        int abilityCounter = 0;
        foreach (TMP_Text abilityText in abilityTexts)
        {
            abilityText.text = MiniGameManager.KnightInstance.Defenses[abilityCounter].defenseName;
            abilityCounter++;
        }
    }

    public void SetAbilitiesTraitor()
    {
        int abilityCounter = 0;
        foreach (TMP_Text abilityText in abilityTexts)
        {
            abilityText.text = MiniGameManager.TraitorInstance.Attacks[abilityCounter].attackName;
            abilityCounter++;
        }
    }

    private void ChangeAbilityDescription(string abilityReflectText, string abilitySplitText, string abilityNoEffectText)
    {
        abilityDescReflectText.text = abilityReflectText;
        abilityDescSplitText.text = abilitySplitText;
        abilityDescNoEffectText.text = abilityNoEffectText;
    }

    public void UpdateHealthBars()
    {
        
        // normalise scale between 0 to 0.99
        float healthScaleKnight = MiniGameManager.KnightInstance.healthPoints / (float) 100.0;
        float healthScaleTraitor = MiniGameManager.TraitorInstance.healthPoints / (float)100.0;

        // avoid negative values
        if (healthScaleKnight <= 0.0f) 
        {
            healthScaleKnight = 0.0f;
        }
        if (healthScaleTraitor <= 0.0f) 
        { 
            healthScaleTraitor = 0.0f;
        }

        // update healthbar
        healthBarKnight.transform.localScale = new Vector3(healthScaleKnight, 1, 0);
        healthBarTraitor.transform.localScale = new Vector3(healthScaleTraitor, 1, 0);

        healthBarValueKnight.text = (uint)MiniGameManager.KnightInstance.healthPoints + " HP";
        healthBarValueTraitor.text = (uint)MiniGameManager.TraitorInstance.healthPoints + " HP";
    }

    public void HoverFirstAbility()
    {
        if (MiniGameManager.CurrentTeam == Team.KNIGHT)
        {
            ChangeAbilityDescription(MiniGameManager.KnightInstance.Defenses[0].DescriptionReflect,
                MiniGameManager.KnightInstance.Defenses[0].DescriptionSplit,
                MiniGameManager.KnightInstance.Defenses[0].DescriptionNoEffect);
        }
        else if (MiniGameManager.CurrentTeam == Team.TRAITOR)
        {
            ChangeAbilityDescription(MiniGameManager.TraitorInstance.Attacks[0].DescriptionReflect,
                MiniGameManager.TraitorInstance.Attacks[0].DescriptionSplit,
                MiniGameManager.TraitorInstance.Attacks[0].DescriptionNoEffect);
        }
    }

    public void HoverSecondAbility()
    {
        if (MiniGameManager.CurrentTeam == Team.KNIGHT)
        {
            ChangeAbilityDescription(MiniGameManager.KnightInstance.Defenses[1].DescriptionReflect,
                MiniGameManager.KnightInstance.Defenses[1].DescriptionSplit,
                MiniGameManager.KnightInstance.Defenses[1].DescriptionNoEffect);
        }
        else if (MiniGameManager.CurrentTeam == Team.TRAITOR)
        {
            ChangeAbilityDescription(MiniGameManager.TraitorInstance.Attacks[1].DescriptionReflect,
                MiniGameManager.TraitorInstance.Attacks[1].DescriptionSplit,
                MiniGameManager.TraitorInstance.Attacks[1].DescriptionNoEffect);
        }
    }

    public void HoverThirdAbility()
    {
        if (MiniGameManager.CurrentTeam == Team.KNIGHT)
        {
            ChangeAbilityDescription(MiniGameManager.KnightInstance.Defenses[2].DescriptionReflect,
                MiniGameManager.KnightInstance.Defenses[2].DescriptionSplit,
                MiniGameManager.KnightInstance.Defenses[2].DescriptionNoEffect);
        }
        else if (MiniGameManager.CurrentTeam == Team.TRAITOR)
        {
            ChangeAbilityDescription(MiniGameManager.TraitorInstance.Attacks[1].DescriptionReflect,
                MiniGameManager.TraitorInstance.Attacks[1].DescriptionSplit,
                MiniGameManager.TraitorInstance.Attacks[1].DescriptionNoEffect);
        }
    }

    public void HoverFourthAbility()
    {
        if (MiniGameManager.CurrentTeam == Team.KNIGHT)
        {
            ChangeAbilityDescription(MiniGameManager.KnightInstance.Defenses[3].DescriptionReflect,
                MiniGameManager.KnightInstance.Defenses[3].DescriptionSplit,
                MiniGameManager.KnightInstance.Defenses[3].DescriptionNoEffect);
        }
        else if (MiniGameManager.CurrentTeam == Team.TRAITOR)
        {
            ChangeAbilityDescription(MiniGameManager.TraitorInstance.Attacks[3].DescriptionReflect,
                MiniGameManager.TraitorInstance.Attacks[3].DescriptionSplit,
                MiniGameManager.TraitorInstance.Attacks[3].DescriptionNoEffect);
        }
    }

    public void ChangeViewCauseOfWin() 
    {
        AbilityBox.SetActive(false);
        FightInformationBox.SetActive(true);
    }

    public void DisplayAbilities()
    {
        FightInformationBox.SetActive(false);
        AbilityBox.SetActive(true);
    }

    #endregion 

    #region Viewer

    public void DisplayViewerView() 
    {
        FightInformationBox.SetActive(true);
        AbilityBox.SetActive(false);
    }

    public void ChangeFightViewerDescription(string attackName, string defenseName, int damageToTraitor, int damageToKnight) 
    {   
        if (defenseName == "") defenseName = "no defense";
        if (attackName == "") attackName = "a random offense";

        FightInformationDescription.text = "The traitor attacked with " + attackName + " against " + defenseName + " and dealed " + damageToKnight + " damage to the knight while he gets " + damageToTraitor + " damage back \n \n Now they are choosing abilities again.";
    }

    public void ChangeFightViewerDescriptionKnightWin()
    {
        FightInformationDescription.text = "The traitor reached 0 HP. The kinght won the Minigame and will survive the night!";
        DisplayViewerView();
    }

    public void ChangeFightViewerDescriptionTraitorWin()
    {
        FightInformationDescription.text = "The knight reached 0 HP. The traitor won the Minigame! The knight is dead…";
        DisplayViewerView();
    }

    public void ChangeFightViewerDescriptionReset()
    {
        DisplayViewerView();
        FightInformationDescription.text = "The fight started! The duelists are now choosing their abilities.";
    }

    public void HideAnimation()
    {
        fightWinAnimation.transform.GetComponent<Animator>().StopPlayback();
    }
    #endregion

    #region Animations

    public void DisplayRoundAnimation(uint roundCount)
    {
        roundFightAnimation.transform.GetComponentInChildren<TMP_Text>(true).text = "Round " + roundCount;
        roundFightAnimation.transform.GetComponent<Animator>().Play("RoundCounterAnimation");
    }

    public void DisplayDamageDealtToKnightAnimation(int damageDealt)
    {
        damageDealtToKnight.transform.GetComponentInChildren<TMP_Text>(true).text = damageDealt + " HP";
        damageDealtToKnight.transform.GetComponent<Animator>().Play("DamageDealtToKnightAnimation");
        PlayDamageDealtAnimationSound();
    }

    public void DisplayDamageDealtToTraitorAnimation(int damageDealt)
    {
        damageDealtToTraitor.transform.GetComponentInChildren<TMP_Text>(true).text = damageDealt + " HP";
        damageDealtToTraitor.transform.GetComponent<Animator>().Play("DamageDealtToTraitorAnimation");
        PlayDamageDealtAnimationSound();
    }
    
    public void DisplayFightWinAnimation(string winner)
    {
        fightWinAnimation.transform.GetComponentInChildren<TMP_Text>(true).text = winner + " wins";
        fightWinAnimation.transform.GetComponent<Animator>().Play("FightWinAnimation");
        PlayMiniGameEndSound();
    }

    public void PlayDamageDealtAnimationSound()
    {
        AnimationSounds.PlayOneShot(HealthLossSound);
    }

    public void PlayMiniGameEndSound()
    {
        AnimationSounds.PlayOneShot(MiniGameEndSound, 0.5f);
    }

    public void PlayMiniGameStartSound()
    {
        AnimationSounds.PlayOneShot(MiniGameStartSound);
    }

    #endregion

    #region Footer

    public void SetFooterCountdownTime(int time)
    {
        footerCountdownTimer.text = "Time left: " + time;
    }

    public void HideFooterCountdownTimer()
    {
        footerCountdownTimer.gameObject.SetActive(false);
    }

    public void DisplayFooterCountdownTimer()
    {
        footerCountdownTimer.gameObject.SetActive(true);
    }
    
    #endregion

    void Start()
    {
        MiniGameManager.HideUIOnLoad();
    }
}
