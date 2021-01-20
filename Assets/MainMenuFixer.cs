using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuFixer : MonoBehaviour
{
    void Awake()
    {
        NetworkPlayerGame[] gamePlayers = FindObjectsOfType<NetworkPlayerGame>();
        NetworkPlayerLobby[] lobbyPlayers = FindObjectsOfType<NetworkPlayerLobby>();
        ExtendedNetworkManager[] networkManagers = FindObjectsOfType<ExtendedNetworkManager>();

        foreach (NetworkPlayerLobby player in lobbyPlayers)
        {
            Destroy(player.gameObject);
        }

        foreach (NetworkPlayerGame player in gamePlayers)
        {
            Destroy(player.gameObject);
        }

        foreach (ExtendedNetworkManager manager in networkManagers)
        {
            Destroy(manager.gameObject);
        }
    }
}
