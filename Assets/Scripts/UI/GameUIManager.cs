using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class GameUIManager : MonoBehaviour
{

    [Header("Abilities")]
    [SerializeField] private GameObject abilityArea;
    [SerializeField] private TMP_Text abilityNameText;
    [SerializeField] private TMP_Text abilityCostText;
    [SerializeField] private TMP_Text abilityDescriptionText;
    
    [Header("Animationen")] [SerializeField]
    private GameObject phasenanimation;
    
    [Header("Backgrounds")] [SerializeField]
    private GameObject background_dayphase;

    [SerializeField] private GameObject background_nightphase;
    [SerializeField] private GameObject background_sunrisephase;
    [SerializeField] private GameObject background_death;
    [SerializeField] private GameObject background_win_knights;
    [SerializeField] private GameObject background_win_traitors;

    [Header("Paper_Scrolls")] 
    [SerializeField] private GameObject paper_scrolls;
    [SerializeField] private GameObject scroll_info;

    [Header("StatusInfo")] 
    [SerializeField] private GameObject none;
    [SerializeField] private GameObject toxified;
    [SerializeField] private GameObject blinded;
    [SerializeField] private GameObject cursed;
    [SerializeField] private GameObject confused;
    [SerializeField] private GameObject protect;

    [Header("Player Positions")] [SerializeField]
    private GameObject playerPositionsContainer;

    [SerializeField] private GameObject statusicon_knight;
    [SerializeField] private GameObject statusicon_traitor;
    [SerializeField] private GameObject statusicon_death;
    
    private List<GameObject> playerPositions = new List<GameObject>();
    private int usedPositionsCount = 0;

    [Header("Header")] 
    [SerializeField] private GameObject header_arrows;
    [SerializeField] private GameObject header_player_sprite;
    [SerializeField] private GameObject dead_player_sprite;
    [SerializeField] private TMP_Text header_player_team_text;

    [Header("Chat")] 
    [SerializeField] private GameObject chat;

    [Header("Tutorial")] 
    [SerializeField] private GameObject tutorial;
    [SerializeField] private GameObject highlights;
    
    [Header("Player Healthbar")]
    [SerializeField] private GameObject playerHealthBar;
    [SerializeField] private TMP_Text playerHealthBarText;
    [SerializeField] private TMP_Text HealthUpdateAnimation;

    
    [Header("Footer")] 
    [SerializeField] private TMP_Text footer_countdown_text;

    [Header("WinScreens")] 
    [SerializeField] private GameObject players_knights_win;

    [SerializeField] private GameObject players_traitors_win;

    [Header("Audio")]
    [SerializeField] private AudioSource inGameMusic;
    
    
    [Header("ReadyButtonSection")]
    [SerializeField] private Button readyButton;
    [SerializeField] private TMP_Text readyCounterText;
    [SerializeField] private GameObject loadingScreen;

    public bool IsAbilityUsable { private set; get; }
    
    private GameObject minigame_ui;
    //
    // public bool tester;
    //
    // public void Update()
    // {
    //     if (tester)
    //     {
    //         background_death.transform.GetComponent<Animator>().Play("DeathAnimation");
    //         tester = false;
    //     }
    //
    //    
    // }

    public void HideLoadingScreen()
    {
        loadingScreen.SetActive(false);
    }

    #region AbilityRegion

    public void SetAbility(Ability ability)
    {
        abilityArea.transform.GetComponent<AbilityArea>().ability.sprite = ability.GetSprite();
        abilityNameText.text = ability.GetAbilityName();
        abilityCostText.text = "Cost: " + ability.GetCost() + "hp";
        abilityDescriptionText.text = ability.GetDescription();
        abilityArea.transform.GetComponent<AbilityArea>().incomeAbilitySound.Play();
    }

    public void StartAbilitySpinning()
    {
        abilityArea.transform.GetComponentInChildren<Animator>().Play("AbilitySpinning");
        abilityArea.transform.GetComponent<Animator>().Play("AbilitySpin");
        abilityArea.transform.GetComponent<AbilityArea>().scrollAbilityFXSound.Play();
    }

    public void StopAbilitySpinning()
    {
        abilityArea.transform.GetComponentInChildren<Animator>().Play("StopSpinning");
        abilityArea.transform.GetComponent<Animator>().Play("Stop");
        abilityArea.transform.GetComponent<AbilityArea>().scrollAbilityFXSound.Stop();
    }

    public void SetAbilityUsable()
    {
        abilityArea.transform.GetComponent<AbilityArea>().ability.color = new Color(1f, 1f, 1f, 1f);
        IsAbilityUsable = true;
    }
    public void SetAbilityUnUsable()
    {
        abilityArea.transform.GetComponent<AbilityArea>().ability.color = new Color(1f, 1f, 1f, 0.25f);
        IsAbilityUsable = false;
    }
    
    #endregion
    
    #region Animationen

    public void PhasenTextAnimation(string phase)
    {
        
        phasenanimation.transform.GetComponentInChildren<TMP_Text>(true).text = phase;
        phasenanimation.transform.GetComponent<Animator>().Play("Phasentext");
    }
    
    public void DisplayPlayerHealthIncreaseAnimation(uint HPvalue)
    {
        HealthUpdateAnimation.transform.GetComponent<TMP_Text>().text = "+ " + HPvalue + " HP";
        HealthUpdateAnimation.transform.GetComponent<Animator>().Play("GameHPIncrease");
    }
    
    public void DisplayPlayerHealthDecreaseAnimation(uint HPvalue)
    {
        HealthUpdateAnimation.transform.GetComponent<TMP_Text>().text = "- " + HPvalue + " HP";
        HealthUpdateAnimation.transform.GetComponent<Animator>().Play("GameHPDecrease");
    }
    
    
    

    #endregion

    #region backgrounds

    public void HideAllBackgrounds()
    {
        background_dayphase.SetActive(false);
        background_nightphase.SetActive(false);
        background_sunrisephase.SetActive(false);
    }

    public void DisplayBackgroundDayphase()
    {
        HideAllBackgrounds();
        background_dayphase.SetActive(true);
    }

    public void DisplayBackgroundNightphase()
    {
        HideAllBackgrounds();
        background_nightphase.SetActive(true);
    }

    public void DisplayBackgroundSunrisephase()
    {
        HideAllBackgrounds();
        background_sunrisephase.SetActive(true);
    }
    
    #endregion
    
    #region Paper_Scrolls

    public void HideAllScrolls()
    {
        ClearChat();
        
        paper_scrolls.GetComponent<Animator>().Play("ScrollOut");
        playerPositionsContainer.GetComponent<Animator>().Play("PlayerIn");
        chat.GetComponent<Animator>().Play("ChatIn");
        scroll_info.GetComponent<ScrollInfo>().PlayScrollAudio();
    }
    

    public void DisplayScrollInfo(string text)
    {
        scroll_info.GetComponent<ScrollInfo>().text.text = text;
        scroll_info.SetActive(true);
        chat.GetComponent<Animator>().Play("ChatOut");
        playerPositionsContainer.GetComponent<Animator>().Play("PlayerOut");
        paper_scrolls.GetComponent<Animator>().Play("ScrollIn");
        scroll_info.GetComponent<ScrollInfo>().PlayScrollAudio();
       
    }
    

    #endregion
    
    #region StatusInfo

    public void HideAllStatusInfo()
    {
       none.SetActive(false);
       toxified.SetActive(false); 
       cursed.SetActive(false);
       blinded.SetActive(false);
       confused.SetActive(false);
       protect.SetActive(false);
       
    }

    public void DisplayNoneStatusInfo()
    {
        HideAllStatusInfo();
        none.SetActive(true);
    }
    public void DisplayToxifiedStatusInfo()
    {
        HideAllStatusInfo();
        none.GetComponentInParent<Animator>().Play("Toxified");
        none.GetComponentInParent<StatusInfo>().statusInfoIncomeSound.Play();
        toxified.SetActive(true);
    }
    public void DisplayCursedStatusInfo()
    {
        HideAllStatusInfo();
        none.GetComponentInParent<Animator>().Play("cursed");
        none.GetComponentInParent<StatusInfo>().statusInfoIncomeSound.Play();
        cursed.SetActive(true);
    }
    public void DisplayBlindedStatusInfo()
    {
        HideAllStatusInfo();
        none.GetComponentInParent<Animator>().Play("Blinded");
        none.GetComponentInParent<StatusInfo>().statusInfoIncomeSound.Play();
        blinded.SetActive(true);
    }
    public void DisplayConfusedStatusInfo()
    {
        HideAllStatusInfo();
        none.GetComponentInParent<Animator>().Play("confused");
        none.GetComponentInParent<StatusInfo>().statusInfoIncomeSound.Play();
        confused.SetActive(true);
    }
    
    public void DisplayProtectedStatusInfo()
    {
        HideAllStatusInfo();
        none.GetComponentInParent<Animator>().Play("protected");
        none.GetComponentInParent<StatusInfo>().statusInfoIncomeSound.Play();
        protect.SetActive(true);
    }
    
    #endregion
    
    #region PlayerPositions

    public void AssignPlayerToPosition(NetworkPlayerGame networkplayer)
    {
        if (playerPositions.Count == 0)
        {
            foreach (Transform child in playerPositionsContainer.transform)
            {
                playerPositions.Add(child.gameObject);
            }
        }

        playerPositions[usedPositionsCount].transform.GetComponent<PlayerSlotScript>().id = networkplayer.netId;
        playerPositions[usedPositionsCount].transform.GetComponent<PlayerSlotScript>().playername.text =
            networkplayer.GetPlayerName();
        playerPositions[usedPositionsCount].transform.GetComponent<PlayerSlotScript>().active = true;
        playerPositions[usedPositionsCount].transform.GetComponent<PlayerSlotScript>().playericon.sprite = networkplayer.GetSprite();
        playerPositionsContainer.SetActive(true);
        usedPositionsCount++;
    }
    
    public void DisplayPlayerPositions()
    {
        foreach (var player in playerPositions)
        {
            if (player.transform.GetComponent<PlayerSlotScript>().active)
            {
                player.SetActive(true);
            }
        }
    }

    public void HidePlayerPositions()
    {
        foreach (var player in playerPositions)
        {
            player.gameObject.SetActive(false);
        }
    }
    public void SetPlayerSprite(NetworkPlayerGame networkplayer)
    {
        foreach (var player in playerPositions)
        {
            if (player.GetComponent<PlayerSlotScript>().id == networkplayer.netId)
            {
                player.transform.GetComponent<PlayerSlotScript>().playericon.sprite = networkplayer.GetSprite();
            }
        }
    }
    #endregion
    
    #region PlayerStatus
    
    public void HideAllPlayerStatus()
    {
        foreach (var player in playerPositions)
        {
            player.transform.GetComponent<PlayerSlotScript>().statusicon.gameObject.SetActive(false);
        }
    }

    public void DisplayAllPlayerStatus()
    {
        foreach (var player in playerPositions)
        {
            if (player.transform.GetComponent<PlayerSlotScript>().active)
                player.transform.GetComponent<PlayerSlotScript>().statusicon.gameObject.SetActive(true);
        }
    }

    public void HighlightMe(NetworkPlayerGame networkPlayer)
    {
        foreach (var player in playerPositions)
        {
            if (player.GetComponent<PlayerSlotScript>().id == networkPlayer.netId)
            {
                player.transform.GetComponent<PlayerSlotScript>().playername.color = new Color(1f,0.7968988f,0.3679245f,1 );
            }
        }
    }

    public void SetPlayerStatus(uint networkid, bool isKnight)
    {
        foreach (var player in playerPositions)
        {
            if (player.transform.GetComponent<PlayerSlotScript>().id == networkid)
                if (isKnight)
                {
                    player.transform.GetComponent<PlayerSlotScript>().statusicon.sprite =
                        statusicon_knight.transform.GetComponent<SpriteRenderer>().sprite;
                }
                else
                {
                    player.transform.GetComponent<PlayerSlotScript>().statusicon.sprite =
                        statusicon_traitor.transform.GetComponent<SpriteRenderer>().sprite;
                }
        }
    }
    
    public void DisplayPlayerStatus(uint networkid)
    {
        foreach (var player in playerPositions)
        {
            if (player.transform.GetComponent<PlayerSlotScript>().id == networkid)
            {
                player.transform.GetComponent<PlayerSlotScript>().statusicon.gameObject.SetActive(true);
                break;
            }
        }
    }

    public void SetPositionDead(NetworkPlayerGame networkplayer)
    {
        foreach (var player in playerPositions)
        {
            
            Color playername_dead = new Color(1f, 1f, 1f, 0.5f);
            Color playericon_dead = new Color(1f, 1f, 1f, 0.5f);
            Color playerbutton_dead = new Color(0f, 0f, 0f, 1.0f);
            
            if (player.GetComponent<PlayerSlotScript>().id == networkplayer.netId)
            {
                player.transform.GetComponent<PlayerSlotScript>().statusicon.sprite = null;
                player.transform.GetComponent<PlayerSlotScript>().playername.color = playername_dead;
                player.transform.GetComponent<PlayerSlotScript>().playericon.color = playericon_dead;
                player.transform.GetComponent<PlayerSlotScript>().button.image.color = playerbutton_dead;
                player.transform.GetComponent<PlayerSlotScript>().badge_death.SetActive(true);
                break;
            }
        }
    }

    public void SetPlayerTeamHeaderIconDead()
    {
        SetHeaderDeadIcon();
        SetHeaderTeamText("YOU DIED");
    }
    
    public void SetPositionAlive(NetworkPlayerGame networkplayer)
    {
        foreach (var player in playerPositions)
        {
            Color playername_alive = new Color(1f, 1f, 1f, 0.5f);
            Color playericon_alive = new Color(1f, 1f, 1f, 0.5f);
            Color playerbutton_alive = new Color(1f, 1f, 1f, 1.0f);
            
            if (player.GetComponent<PlayerSlotScript>().id == networkplayer.netId)
            {
                if (networkplayer.GetTeam() == Team.KNIGHT)
                {
                    player.transform.GetComponent<PlayerSlotScript>().statusicon.sprite = statusicon_knight.transform.GetComponent<SpriteRenderer>().sprite;
                    SetHeaderKnightIcon();
                    SetHeaderTeamText("KNIGHT");
                }
                else
                {
                    player.transform.GetComponent<PlayerSlotScript>().statusicon.sprite = statusicon_traitor.transform.GetComponent<SpriteRenderer>().sprite;
                    SetHeaderKnightIcon();
                    SetHeaderTeamText("TRAITOR");
                }
                player.transform.GetComponent<PlayerSlotScript>().playername.color = playername_alive;
                player.transform.GetComponent<PlayerSlotScript>().playericon.color = playericon_alive;
                player.transform.GetComponent<PlayerSlotScript>().button.image.color = playerbutton_alive;
                player.transform.GetComponent<PlayerSlotScript>().badge_death.SetActive(false);
                break;
            }
        }
    }
    
    public void HidePlayerStatus(uint networkid)
    {
        foreach (var player in playerPositions)
        {
            if (player.transform.GetComponent<PlayerSlotScript>().id == networkid)
            {
                player.transform.GetComponent<PlayerSlotScript>().statusicon.gameObject.SetActive(false);
                break;
            }
        }
    }
    
    #endregion
    
    #region PlayerHealth
    
    public void updatePlayerHealthbar(float playerHealth)
    {
        float playerHealthScale = playerHealth / 100;
        
        playerHealthBar.transform.localScale = new Vector3(playerHealthScale, 1, 1);
        playerHealthBarText.text = (uint)playerHealth + " HP";
    }
    
    #endregion

    #region Votes

      public void HideAllVotes()
    {
        foreach (var player in playerPositions)
        {
            player.transform.GetComponent<PlayerSlotScript>().votescontainer.SetActive(false);
            foreach (Transform vote in player.transform.GetComponent<PlayerSlotScript>().votescontainer.transform)
            {
                vote.gameObject.SetActive(false);
            }
        }
        ResetAllVotes();
    }

    public void DisplayAllVotes()
    {
        foreach (var player in playerPositions)
        {
            player.transform.GetComponent<PlayerSlotScript>().votescontainer.SetActive(true);
            foreach (Transform vote in player.transform.GetComponent<PlayerSlotScript>().votescontainer.transform)
            {
                vote.gameObject.SetActive(true);
            }
        }
        playerPositionsContainer.GetComponent<AudioSource>().Play();
    }

    public void AddVoteOptions(uint[] ids)
    {
        foreach (var id in ids)
        {
            foreach (var player in playerPositions)
            {
                if (player.transform.GetComponent<PlayerSlotScript>().id == id)
                {
                    player.transform.GetComponent<PlayerSlotScript>().button.interactable = true;
                }
            }
        }
    }

    public void HighliteVotee(uint votee)
    {
        foreach (var player in playerPositions)
        {
            if (player.transform.GetComponent<PlayerSlotScript>().id == votee)
            {
                player.transform.GetComponent<PlayerSlotScript>().highlite.SetActive(true);
            }
        }
    }

    public void RemoveVoteOptions()
    {
        foreach (var player in playerPositions)
        {
            player.transform.GetComponent<PlayerSlotScript>().button.interactable = false;
        }
    }

    public void AddVote(uint voter, uint votee)
    {
        GameObject voterPlayer = null;
        GameObject voteePlayer = null;
        
        foreach (var player in playerPositions)
        {
            if (player.transform.GetComponent<PlayerSlotScript>().id == voter)
            {
                voterPlayer = player;
            }

            if (player.transform.GetComponent<PlayerSlotScript>().id == votee)
            {
                voteePlayer = player;
            }
        }

        if (voterPlayer != null && voteePlayer != null)
        {
            if (voteePlayer.transform.GetComponent<PlayerSlotScript>().isInverted)
            {
                voteePlayer.transform.GetComponent<PlayerSlotScript>().vote[(voteePlayer.transform.GetComponent<PlayerSlotScript>().vote.Length-1)-voteePlayer.transform.GetComponent<PlayerSlotScript>().voteCounter++].sprite =
                    voterPlayer.transform.GetComponent<PlayerSlotScript>().playericon.sprite;
            }
            else
            {
                voteePlayer.transform.GetComponent<PlayerSlotScript>().vote[voteePlayer.transform.GetComponent<PlayerSlotScript>().voteCounter++].sprite =
                    voterPlayer.transform.GetComponent<PlayerSlotScript>().playericon.sprite;
            }
            
        }
    }

    public void ResetAllVotes()
    {
        foreach (var player in playerPositions)
        {
            foreach (var sprite in player.transform.GetComponent<PlayerSlotScript>().vote)
            {
                sprite.sprite = null;
            }

            player.transform.GetComponent<PlayerSlotScript>().voteCounter = 0;
            player.transform.GetComponent<PlayerSlotScript>().highlite.SetActive(false);
        }
    }

    #endregion

    #region Win & Death Screens

    public void DisplayDeath(NetworkPlayerGame player)
    {
        background_death.transform.GetComponent<Background_Death>().playername.text = player.GetPlayerName();
        background_death.transform.GetComponent<Background_Death>().playerIcon.sprite = player.GetSprite();
        background_death.SetActive(true);
        background_death.transform.GetComponent<Animator>().Play("DeathAnimation");
        var parent = background_death.transform.parent;
        parent.gameObject.SetActive(true);
        parent.GetComponentInChildren<Transform>().gameObject.SetActive(true);
        SetPositionDead(player);
        background_death.transform.GetComponent<Background_Death>().death.Play();
        inGameMusic.Pause();
    }
    public void HideDeath()
    {
        background_death.transform.GetComponent<Animator>().Play("Stop");
        background_death.transform.parent.gameObject.SetActive(false);
        inGameMusic.Play();
    }

    public void DisplayWinTraitor(NetworkPlayerGame[] players)
    {
        players_traitors_win.GetComponentInChildren<Transform>().gameObject.SetActive(false);
        for (int i = 0; i < players.Length; i++)
        {
            players_traitors_win.transform.GetChild(i).transform.GetComponent<PlayerSlotScript>().playername.text = players[i].GetPlayerName();
            players_traitors_win.transform.GetChild(i).transform.GetComponent<PlayerSlotScript>().playericon.sprite = players[i].GetSprite();
            players_traitors_win.transform.GetChild(i).transform.GetComponent<Transform>().gameObject.SetActive(true);
            
        }
        background_win_traitors.transform.parent.gameObject.SetActive(true);
        players_traitors_win.gameObject.SetActive(true);
        background_win_traitors.transform.GetComponent<Animator>().Play("TraitorsWin");
        background_win_traitors.transform.GetComponent<WinScript>().win.Play();
        inGameMusic.Stop();
    }
   
    public void DisplayWinKnight(NetworkPlayerGame[] players)
    {
        players_knights_win.GetComponentInChildren<Transform>().gameObject.SetActive(false);
        for (int i = 0; i < players.Length; i++)
        {
            players_knights_win.transform.GetChild(i).transform.GetComponent<PlayerSlotScript>().playername.text = players[i].GetPlayerName();
            players_knights_win.transform.GetChild(i).transform.GetComponent<PlayerSlotScript>().playericon.sprite = players[i].GetSprite();
            players_knights_win.transform.GetChild(i).transform.GetComponent<Transform>().gameObject.SetActive(true);
            
        }
        background_win_knights.transform.parent.gameObject.SetActive(true);
        players_knights_win.gameObject.SetActive(true);
        background_win_knights.transform.GetComponent<Animator>().Play("TraitorsWin");
        background_win_knights.transform.GetComponent<WinScript>().win.Play();
        inGameMusic.Stop();
    }
   
    #endregion

    #region Chat

    public void DisplayChat()
    {
        chat.SetActive(true);
    }

    public void HideChat()
    {
        chat.SetActive(false);
        chat.GetComponent<ChatBehavior>().clearMessages();
    }

    public void ClearChat()
    {
        chat.GetComponent<ChatBehavior>().clearMessages();
    }
    #endregion

    #region Tutorial

    public void DisplayTutorial(string info)
    {
        tutorial.transform.GetComponent<Tutorial>().DisplayTutorial(info);
    }

    public void HideTutorial()
    {
        tutorial.transform.GetComponent<Tutorial>().HideTutorial();
    }
    
    public void DisplayHighlight(Highlights.Highlight highlight)
    {
        highlights.GetComponent<Highlights>().DisplayHighlight(highlight);
    }
    public void HideHighlight(Highlights.Highlight highlight)
    {
        highlights.GetComponent<Highlights>().HideHighlight(highlight);
    }
    #endregion

    #region Header

    public void DisplayAllHeader()
    {
        DisplayHeaderArrows();
        DisplayHeaderTeamIcon();
        DisplayHeaderTeamText();
    }

    public void HideAllHeader()
    {
        HideHeaderArrows();
        HideHeaderTeamIcon();
        HideHeaderTeamText();
    }

    public void DisplayHeaderArrows()
    {
        header_arrows.SetActive(true);
    }

    public void HideHeaderArrows()
    {
        header_arrows.SetActive(false);
    }

    public void DisplayHeaderTeamIcon()
    {
        header_player_sprite.SetActive(true);
    }

    public void HideHeaderTeamIcon()
    {
        header_player_sprite.SetActive(false);
    }

    public void SetHeaderTraitorIcon()
    {
        header_player_sprite.GetComponent<Image>().sprite = statusicon_traitor.transform.GetComponent<SpriteRenderer>().sprite;
    }
    public void SetHeaderKnightIcon()
    {
        header_player_sprite.GetComponent<Image>().sprite = statusicon_knight.transform.GetComponent<SpriteRenderer>().sprite;
    }

    public void SetHeaderDeadIcon()
    {
        header_player_sprite.GetComponent<Image>().sprite = statusicon_death.transform.GetComponent<SpriteRenderer>().sprite;
    }
    public void DisplayHeaderTeamText()
    {
        header_player_team_text.gameObject.SetActive(true);
    }

    public void HideHeaderTeamText()
    {
        header_player_team_text.gameObject.SetActive(false);
    }

    public void SetHeaderTeamText(string text)
    {
        header_player_team_text.text = text;
    }

    #endregion

    #region Footer

    public void DisplayFooterCountdown()
    {
        footer_countdown_text.gameObject.SetActive(true);
    }

    public void HideFooterCountdown()
    {
        footer_countdown_text.gameObject.SetActive(false);
    }

    public void SetFooterCountdownTime(int time)
    {
        footer_countdown_text.text = "Time left: " + time;
    }

    #endregion

    #region Minigame

    public void DisplayMinigameUI()
    {
       InitializeMiniGameUI();
       minigame_ui.SetActive(true);

    }

    public void HideMinigameUI()
    {
        InitializeMiniGameUI();
        minigame_ui.SetActive(false);
    }

    public void InitializeMiniGameUI()
    {
        if (!minigame_ui)
        {
            minigame_ui = FindObjectOfType<PhaseMiniGame>().gameObject;
        }
    }
    #endregion


    #region
    public void DisplayReadySection(uint maxReady)
    {
        readyButton.gameObject.SetActive(true);
        readyCounterText.text = "0/" + maxReady + " ready";
        readyCounterText.gameObject.SetActive(true);
        readyButton.interactable = true;
    }

    public void HideReadySection()
    {
        readyButton.gameObject.SetActive(false);
        readyCounterText.gameObject.SetActive(false);
    }

    public void EnableReadyButton()
    {
        readyButton.interactable = true;
    }

    public void UpdateReadyCounter(uint currentReady, uint maxReady)
    {
        readyCounterText.text = currentReady + "/" + maxReady + " ready";
    }
    #endregion
}