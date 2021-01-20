using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyUIManager : MonoBehaviour
{

    [Header("UI Player slots")]
    [SerializeField] private Button startButton = null;
    [SerializeField] private List<TMP_Text> playerNameTexts = null;
    [SerializeField] private List<TMP_Text> playerReadyTexts = null;

    [Header("UI Game configuration")]
    [SerializeField] private TMP_Text useTimerButtonText = null;
    [SerializeField] private TMP_Text traitorCountText;
    [SerializeField] private TMP_Text playerCountText;
    [SerializeField] private Button traitorIncreaseButton;
    [SerializeField] private Button traitorDecreaseButton;
    [SerializeField] private Button useTimerButton;

    private ExtendedNetworkManager lobby = null;
    private LobbySettingsSynchronizer lobbySettingsSynchronizer;

    #region getters
    private LobbySettingsSynchronizer LobbySettingsSynchronizer
    {
        get
        {
            if (lobbySettingsSynchronizer != null) { return lobbySettingsSynchronizer; }
            return lobbySettingsSynchronizer = FindObjectOfType<LobbySettingsSynchronizer>();
        }
    }
    #endregion

    #region unity functions
    private void Awake()
    {
        lobby = FindObjectOfType<ExtendedNetworkManager>();
    }
    #endregion

    #region setters
    public void SetTraitorCount(uint traitorCount)
    {
        // there can be at most on traitor less than knights
        if(traitorCount >= ((lobby.MaxPlayers / 2) - 1))
        {
            traitorIncreaseButton.interactable = false;
        }
        else
        {
            traitorIncreaseButton.interactable = true;
        }

        if (traitorCount <= 1)
        {
            traitorDecreaseButton.interactable = false;
        }
        else
        {
            traitorDecreaseButton.interactable = true;
        }

        traitorCountText.text = "Traitor Count: " + traitorCount; 
    }

    public void SetMinPlayerCount(uint minPlayerCount)
    {
        playerCountText.text = "Minimum players needed: " + minPlayerCount;
        startButton.interactable = minPlayerCount <= lobby.LobbyPlayers.Count && lobby.IsReadyToStart();
    }

    public void SetUseTimerText(bool isTimerEnabled)
    {
        if(isTimerEnabled)
        {
            useTimerButtonText.text = "Timer on";
        }
        else
        {
            useTimerButtonText.text = "Timer off";
        }
    }

    #endregion

    #region events
    public void OnReadyButtonClicked()
    {
        lobby.PlayerLobbyLocal.CmdToggleReadyStatus();
    }

    public void OnStartButtonClicked()
    {
        lobby.PlayerLobbyLocal.CmdStartGame();
    }

    public void UpdateUI()
    {
        int count;
        for(count = 0; count < lobby.LobbyPlayers.Count; count++)
        {
            playerNameTexts[count].text = lobby.LobbyPlayers[count].DisplayName;

            if(lobby.LobbyPlayers[count].IsReady)
            {
                playerReadyTexts[count].text = "Ready!";
                playerReadyTexts[count].color = Color.green;
            }
            else
            {
                playerReadyTexts[count].text = "Not ready.";
                playerReadyTexts[count].color = Color.red;
            }
        }
    }

    public void OnReadyToStart(bool isReady)
    {
        startButton.interactable = isReady;
    }
    #endregion

    public void SetupHost()
    {
        startButton.gameObject.SetActive(true);
        traitorIncreaseButton.gameObject.SetActive(true);
        traitorDecreaseButton.gameObject.SetActive(true);
        useTimerButton.gameObject.SetActive(true);
        useTimerButton.interactable = true;
        startButton.interactable = false;
    }
}
