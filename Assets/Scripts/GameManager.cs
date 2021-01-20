using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class GameManager : NetworkBehaviour
{

    #region member variables

    [SyncVar(hook = nameof(OnReadyCounterChanged))] uint readyCounter = 0;
    uint readyNeededCount = 0;
    bool hasPlayerDiedAtNight = false;
    [SyncVar]
    bool isChatEnabled = true;

    [SyncVar] public bool UseTimer;

    #region managers
    private ExtendedNetworkManager room;
    private GameUIManager uiManager;
    private PlayerManager playerManager;
    private PhaseManager phaseManager;
    private AbilityManager abilityManager;
    private VoteManager voteManager;
    #endregion

    #endregion



    #region Setters

    #endregion


    #region Getters

    public NetworkPlayerGame GetPlayer(uint netID)
    {
        foreach(NetworkPlayerGame player in Room.GamePlayers)
        {
            if(player.netId == netID)
            {
                return player;
            }
        }
        return null;
    }

    public ExtendedNetworkManager Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as ExtendedNetworkManager;
        }
    }

    private PlayerManager PlayerManager
    {
        get
        {
            if (playerManager != null) { return playerManager; }
            return playerManager = FindObjectOfType<PlayerManager>();
        }
    }

    private GameUIManager UIManager
    {
        get
        {
            if (uiManager != null) { return uiManager; }
            return uiManager = FindObjectOfType<GameUIManager>();
        }
    }

    private PhaseManager PhaseManager
    {
        get
        {
            if (phaseManager != null) { return phaseManager; }
            return phaseManager = FindObjectOfType<PhaseManager>();
        }
    }

    private AbilityManager AbilityManager
    {
        get
        {
            if (abilityManager != null) { return abilityManager; }
            return abilityManager = FindObjectOfType<AbilityManager>();
        }
    }

    private VoteManager VoteManager
    {
        get
        {
            if (voteManager != null) { return voteManager; }
            return voteManager = FindObjectOfType<VoteManager>();
        }
    }

    #endregion

    private void Awake()
    {
        uiManager = FindObjectOfType<GameUIManager>();
    }

    [Server]
    public void PlayerReadyConnected(NetworkConnection conn)
    {
        foreach (NetworkPlayerGame player in Room.GamePlayers)
        {
            if (!player.connectionToClient.isReady) { return; }
        }

        StartGame();
    }

    [Server]
    public void StartGame()
    {
        UseTimer = Room.UseTimer;
        AbilityManager.ServerSpawnAbilities();
        PhaseManager.ServerSpawnPhases();

        PlayerManager.AssignPlayerTeams();

        foreach (NetworkPlayerGame player in Room.GamePlayers)
        {
            PlayerManager.AssignPlayerSprite(player);
            
            player.ClientAssignToPosition();
        }
        
        PhaseManager.ServerStartNextPhase();
    }

    [Server]
    public void ServerToggleChat(bool isEnabled)
    {
        isChatEnabled = isEnabled; 
    }

    public bool IsChatEnabled()
    {
        return isChatEnabled;
    }

    [Server]
    private void CleanUpServer()
    {
        ExtendedNetworkManager.OnServerStopped -= CleanUpServer;
        ExtendedNetworkManager.OnPlayerReadyConnected -= PlayerReadyConnected;
    }

    [ServerCallback]
    private void OnDestroy()
    {
        CleanUpServer();
    }

    #region Start & Stop Callbacks

    /// <summary>
    /// This is invoked for NetworkBehaviour objects when they become active on the server.
    /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
    /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
    /// </summary>
    public override void OnStartServer() 
    {
        ExtendedNetworkManager.OnPlayerReadyConnected += PlayerReadyConnected;
        ExtendedNetworkManager.OnServerStopped += CleanUpServer;
    }

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
        
    }

    /// <summary>
    /// This is invoked on behaviours that have authority, based on context and <see cref="NetworkIdentity.hasAuthority">NetworkIdentity.hasAuthority</see>.
    /// <para>This is called after <see cref="OnStartServer">OnStartServer</see> and before <see cref="OnStartClient">OnStartClient.</see></para>
    /// <para>When <see cref="NetworkIdentity.AssignClientAuthority"/> is called on the server, this will be called on the client that owns the object. When an object is spawned with <see cref="NetworkServer.Spawn">NetworkServer.Spawn</see> with a NetworkConnection parameter included, this will be called on the client that owns the object.</para>
    /// </summary>
    public override void OnStartAuthority() { }

    /// <summary>
    /// This is invoked on behaviours when authority is removed.
    /// <para>When NetworkIdentity.RemoveClientAuthority is called on the server, this will be called on the client that owns the object.</para>
    /// </summary>
    public override void OnStopAuthority() { }

    #endregion

    #region API

    public List<NetworkPlayerGame> GetPoisonedPlayers()
    {
        return PlayerManager.GetPoisonedPlayers();
    }

    public List<NetworkPlayerGame> GetKnightPlayers()
    {
        return PlayerManager.GetKnightPlayers();
    }

    public List<NetworkPlayerGame> GetTraitorPlayers()
    {
        return PlayerManager.GetTraitorPlayers();
    }

    public Sprite GetSprite(int spriteID)
    {
        return PlayerManager.GetSprite(spriteID);
    }

    public void ServerAssignPlayerAbility(NetworkPlayerGame player)
    {
        AbilityManager.ServerAssignPlayerAbility(player);
    }

    public Ability GetAbility(int abilityID)
    {
        return AbilityManager.GetAbility(abilityID);
    }

    public void ServerStartTimer(int time)
    {
        PhaseManager.ServerStartTimer(time);
    }

    public void ServerAddVote(uint voter, uint votee)
    {
        VoteManager.ServerAddVote(voter, votee);
    }

    public void ServerAddVoteOption(uint voteOption)
    {
        VoteManager.ServerAddVoteOption(voteOption);
    }

    public void ServerStartVoting()
    {
        VoteManager.ServerStartVoting();
    }

    public void ServerStartVotingTraitor()
    {
        VoteManager.ServerStartVotingTraitor();
    }

    public void ServerEndVoting()
    {
        VoteManager.ServerEndVoting();
    }

    public void ServerDisplayVoteResults()
    {
        VoteManager.ServerDisplayVoteResults();
    }

    public void ServerDisplayVoteResultsTraitor()
    {
        VoteManager.ServerDisplayVoteResultsTraitor();
    }

    public uint GetLastVotedPlayerNetID()
    {
        return VoteManager.GetLastVotedPlayerNetID();
    }

    [Server]
    public void ServerKillPlayer(uint targetPlayerNetID)
    {
        PhaseManager.ServerHaltTimer();
        GetPlayer(targetPlayerNetID).Kill();
        RpcDisplayDeathScreen(targetPlayerNetID);
        StartCoroutine(DeathCoroutine());
    }

    [ClientRpc]
    public void RpcDisplayDeathScreen(uint targetPlayerNetID)
    {
        NetworkPlayerGame player = GetPlayer(targetPlayerNetID);
        UIManager.DisplayDeath(player);
        UIManager.HidePlayerStatus(targetPlayerNetID);
        if(Room.PlayerGameLocal.netId == targetPlayerNetID)
        {
            UIManager.SetPlayerTeamHeaderIconDead();
        }
    }

    private IEnumerator DeathCoroutine()
    {
        int timeLeftDeathSeconds = 5;
        while (timeLeftDeathSeconds > 0)
        {
            yield return new WaitForSeconds(1);
            timeLeftDeathSeconds -= 1;
        }

        RpcHideDeathScreen();

        if(IsKnightWinConditionMet())
        {
            RpcDisplayWinScreen(Team.KNIGHT);
        }
        else if(IsTraitorWinConditionMet())
        {
            RpcDisplayWinScreen(Team.TRAITOR);
        }
        else
        {
            PhaseManager.ServerResumeTimer();
        }
    }

    public List<NetworkPlayerGame> GetAlivePlayer()
    {
        return PlayerManager.GetAlivePlayers();
    }

    public List<NetworkPlayerGame> GetAliveKnights()
    {
        return PlayerManager.GetAliveKnights();
    }

    public List<NetworkPlayerGame> GetAliveTraitors()
    {
        return PlayerManager.GetAliveTraitors();
    }

    private bool IsKnightWinConditionMet()
    {
        foreach(NetworkPlayerGame traitor in PlayerManager.GetTraitorPlayers())
        {
            if(traitor.IsAlive())
            {
                return false;
            }
        }
        return true;
    }

    private bool IsTraitorWinConditionMet()
    {
        List<NetworkPlayerGame> knights = new List<NetworkPlayerGame>();
        foreach (NetworkPlayerGame knight in PlayerManager.GetKnightPlayers())
        {
            if (knight.IsAlive())
            {
                knights.Add(knight);
            }
        }

        List<NetworkPlayerGame> traitors = new List<NetworkPlayerGame>();
        foreach (NetworkPlayerGame traitor in PlayerManager.GetTraitorPlayers())
        {
            if (traitor.IsAlive())
            {
                traitors.Add(traitor);
            }
        }

        return traitors.Count >= knights.Count;
    }

    [ClientRpc]
    public void RpcHideDeathScreen()
    {
        UIManager.HideDeath();
    }

    [ClientRpc]
    public void RpcDisplayWinScreen(Team team)
    {
        if(team == Team.KNIGHT)
        {
            UIManager.DisplayWinKnight(PlayerManager.GetKnightPlayers().ToArray());
        }
        else
        {
            UIManager.DisplayWinTraitor(PlayerManager.GetTraitorPlayers().ToArray());
        }
    }

    public bool IsLastVoteResultValid()
    {
        return VoteManager.IsLastVoteResultValid();
    }

    [Server]
    public void ServerEnableReadyFeature()
    {
        List<NetworkPlayerGame> players = new List<NetworkPlayerGame>();
        foreach(NetworkPlayerGame player in Room.GamePlayers)
        {
            if (player.IsAlive()) { players.Add(player); }
        }
        readyNeededCount = (uint)players.Count;
        readyCounter = 0;
        RpcDisplayReadySection(readyNeededCount);
    }

    [Server]
    public void ServerEnableReadyFeatureTraitorsOnly()
    {
        List<NetworkPlayerGame> players = new List<NetworkPlayerGame>();
        foreach (NetworkPlayerGame player in GetTraitorPlayers())
        {
            if (player.IsAlive()) { players.Add(player); }
        }
        readyNeededCount = (uint)players.Count;
        readyCounter = 0;
        RpcDisplayReadySectionTraitorsOnly(readyNeededCount);
    }

    [ClientRpc]
    public void RpcDisplayReadySection(uint maxReady)
    {
        if(!Room.PlayerGameLocal.IsAlive()) { return; }
        readyNeededCount = maxReady;
        UIManager.DisplayReadySection(maxReady);
    }

    [ClientRpc]
    public void RpcDisplayReadySectionTraitorsOnly(uint maxReady)
    {
        if(Room.PlayerGameLocal.GetTeam() == Team.KNIGHT || !Room.PlayerGameLocal.IsAlive()) { return; }
        readyNeededCount = maxReady;
        UIManager.DisplayReadySection(maxReady);
    }

    [Server]
    public void ServerDisableReadyFeature()
    {
        readyCounter = 0;
        RpcDisableReadySection();
    }

    [ClientRpc]
    public void RpcDisableReadySection()
    {
        UIManager.HideReadySection();
    }

    [Server]
    public void ServerIncreaseReadyCounter()
    {
        readyCounter++;
        if(readyCounter >= readyNeededCount)
        {
            ServerDisableReadyFeature();
            readyCounter = 0;
            
            if(UseTimer)
            {
                if (PhaseManager.GetTimeLeft() > 3)
                {
                    PhaseManager.ServerFinishCurrentAction();
                }
            }
            else
            {
                PhaseManager.ServerReadyWithoutTimer();
            }
            
        }
    }

    [Server]
    public void ServerEnableAbilityUse()
    {
        AbilityManager.ServerEnableAbilityUse();
    }

    [Server]
    public void ServerDisableAbilityUse()
    {
        AbilityManager.ServerDisableAbilityUse();
    }

    public bool IsAbilityUseAllowed()
    {
        return AbilityManager.IsAbilityUseAllowed();
    }

    public void OnReadyCounterChanged(uint oldValue, uint newValue)
    {
        UIManager.UpdateReadyCounter(newValue, (uint) readyNeededCount);
    }

    public void ServerRestartPhases()
    {
        PhaseManager.ServerRestartPhases();
    }

    [Server]
    public void ServerInformDeathAtNight(bool hasDied)
    {
        hasPlayerDiedAtNight = hasDied;
    }

    public bool HasPlayerDiedLastNight()
    {
        return hasPlayerDiedAtNight;
    }

    #endregion
}
