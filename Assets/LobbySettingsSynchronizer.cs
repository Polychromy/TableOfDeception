using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LobbySettingsSynchronizer : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnMinPlayersChanged))] public uint MinPlayers = 2;
    [SyncVar(hook = nameof(OnTraitorPlayersChanged))] public uint TraitorPlayerCount = 1;
    [SyncVar(hook = nameof(OnUseTimerChanged))] public bool UseTimer = false;
    private LobbyUIManager lobbyUIManager;

    private LobbyUIManager LobbyUIManager
    {
        get
        {
            if (lobbyUIManager != null) { return lobbyUIManager; }
            return lobbyUIManager = FindObjectOfType<LobbyUIManager>();
        }
    }
    
    [TargetRpc]
    public void TargetSetup(NetworkConnection target, uint traitorCount, uint minPlayers, bool useTimer)
    {
        LobbyUIManager.SetMinPlayerCount(minPlayers);
        LobbyUIManager.SetTraitorCount(traitorCount);
        LobbyUIManager.SetUseTimerText(useTimer);
    }

    public void OnMinPlayersChanged(uint oldValue, uint newValue)
    {
        LobbyUIManager.SetMinPlayerCount(newValue);
    }

    public void OnTraitorPlayersChanged(uint oldValue, uint newValue)
    {
        LobbyUIManager.SetTraitorCount(newValue);
    }

    public void OnUseTimerChanged(bool oldValue, bool newValue)
    {
        LobbyUIManager.SetUseTimerText(newValue);
    }
}
