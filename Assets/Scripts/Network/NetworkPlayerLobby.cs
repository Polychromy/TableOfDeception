using UnityEngine;
using Mirror;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class NetworkPlayerLobby : NetworkBehaviour
{

    [Header("UI")]
    [SerializeField] private LobbyUIManager lobbyUIManager = null;

    [Header("Player Lobby Information")]
    [SyncVar(hook = nameof(OnDisplayNameChanged))]
    public string DisplayName = "Connecting...";
    [SyncVar(hook = nameof(OnReadyStatusChanged))]
    public bool IsReady = false;

    private bool isHost;
    public bool IsHost
    {
        set
        {
            isHost = true;
            if (lobbyUIManager == null) { SetupLobbyUIManager(); }
            lobbyUIManager.SetupHost();
        }
        get
        {
            return isHost;
        }
    }

    private ExtendedNetworkManager lobby;
    private ExtendedNetworkManager Lobby
    {
        get
        {
            if(lobby != null) { return lobby; }
            return lobby = NetworkManager.singleton as ExtendedNetworkManager;
        }
    }

    public void OnDisplayNameChanged(string oldValue, string newValue)
    {
        if (lobbyUIManager == null) { SetupLobbyUIManager(); }
        lobbyUIManager.UpdateUI();
    }
        
    public void OnReadyStatusChanged(bool oldValue, bool newValue)
    {
        lobbyUIManager.UpdateUI();
    }

    public void OnReadyToStart(bool isReady)
    {
        if(!isHost) { return; }

        lobbyUIManager.OnReadyToStart(isReady);
    }

    private void SetupLobbyUIManager()
    {
        lobbyUIManager = FindObjectOfType<LobbyUIManager>();

        if (lobbyUIManager == null)
        {
            Debug.Log("Could not find lobby UI manager.");
        }
    }

    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        foreach(NetworkPlayerLobby player in Lobby.LobbyPlayers)
        {
            if(displayName == player.DisplayName)
            {
                displayName = displayName + Lobby.DuplicateNameCounter++;
            }
        }

        DisplayName = displayName;
    }

    [Command]
    public void CmdToggleReadyStatus()
    {
        IsReady = !IsReady;

        Lobby.NotifyPlayersOfReadyState();
    }

    [Command]
    public void CmdStartGame()
    {
        if(Lobby.LobbyPlayers[0].connectionToClient != connectionToClient) { return; }

        Lobby.StartGame();
    }

    #region Start & Stop Callbacks

    /// <summary>
    /// This is invoked for NetworkBehaviour objects when they become active on the server.
    /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
    /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
    /// </summary>
    public override void OnStartServer() { }

    /// <summary>
    /// Invoked on the server when the object is unspawned
    /// <para>Useful for saving object data in persistant storage</para>
    /// </summary>
    public override void OnStopServer() { }

    /// <summary>
    /// Called on every NetworkBehaviour when it is activated on a client.
    /// <para>Objects on the host have this function called, as there is a local client on the host. The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.</para>
    /// </summary>
    public override void OnStartClient() 
    {
        Lobby.LobbyPlayers.Add(this);

        if (lobbyUIManager == null) { SetupLobbyUIManager(); }
        lobbyUIManager.UpdateUI();
    }

    /// <summary>
    /// This is invoked on clients when the server has caused this object to be destroyed.
    /// <para>This can be used as a hook to invoke effects or do client specific cleanup.</para>
    /// </summary>
    public override void OnStopClient() { }

    /// <summary>
    /// Called when the local player object has been set up.
    /// <para>This happens after OnStartClient(), as it is triggered by an ownership message from the server. This is an appropriate place to activate components or functionality that should only be active for the local player, such as cameras and input.</para>
    /// </summary>
    public override void OnStartLocalPlayer() 
    {
        lobby.PlayerLobbyLocal = this;
    }

    /// <summary>
    /// This is invoked on behaviours that have authority, based on context and <see cref="NetworkIdentity.hasAuthority">NetworkIdentity.hasAuthority</see>.
    /// <para>This is called after <see cref="OnStartServer">OnStartServer</see> and before <see cref="OnStartClient">OnStartClient.</see></para>
    /// <para>When <see cref="NetworkIdentity.AssignClientAuthority"/> is called on the server, this will be called on the client that owns the object. When an object is spawned with <see cref="NetworkServer.Spawn">NetworkServer.Spawn</see> with a NetworkConnection parameter included, this will be called on the client that owns the object.</para>
    /// </summary>
    public override void OnStartAuthority() 
    {
        CmdSetDisplayName(PlayerPrefs.GetString("PlayerName"));
    }

    /// <summary>
    /// This is invoked on behaviours when authority is removed.
    /// <para>When NetworkIdentity.RemoveClientAuthority is called on the server, this will be called on the client that owns the object.</para>
    /// </summary>
    public override void OnStopAuthority() { }

    #endregion
}
