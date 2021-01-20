using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonHandlerLobbyTraitorCount : MonoBehaviour
{
    private ExtendedNetworkManager lobby;

    private ExtendedNetworkManager Lobby
    {
        get
        {
            if (lobby != null) { return lobby; }
            return lobby = FindObjectOfType<ExtendedNetworkManager>();
        }
    }

    public void IncreaseTraitorCount()
    {
        if(!Lobby.PlayerLobbyLocal.IsHost) { return; }
        Lobby.IncreaseTraitorCount();
    }

    public void DecreaseTraitorCount()
    {
        if (!Lobby.PlayerLobbyLocal.IsHost) { return; }
        Lobby.DecreaseTraitorCount();
    }

    public void OnUseTimerButtonClicked()
    {
        if (!Lobby.PlayerLobbyLocal.IsHost) { return; }
        Lobby.ToggleUseTimer();
    }
}
