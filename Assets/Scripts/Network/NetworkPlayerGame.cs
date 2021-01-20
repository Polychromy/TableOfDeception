using UnityEngine;
using Mirror;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Collections;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public enum Team
{
    NONE,
    TRAITOR,
    KNIGHT
}

[RequireComponent(typeof(ExtendedNetworkManager))]
public class NetworkPlayerGame : NetworkBehaviour
{
    [Header("Player Information")]
    [SyncVar]
    [SerializeField] private string displayName;
    [SerializeField] private Team team = Team.NONE;
    [SerializeField] private int spriteID = -1;
    [SerializeField] private int abilityID = -1;
    public bool CanUseAbility { get; set; }
    [SyncVar]
    [SerializeField] private bool hasAbility = false;
    [SyncVar]
    [SerializeField] private bool isAlive = true;
    [SyncVar(hook =nameof(OnHealthPointsChanged))]
    [SerializeField] private uint healthPoints = 100;
    [SyncVar(hook = nameof(OnPoisoningStatusChanged))]
    [SerializeField] private bool isPoisoned = false;
    [SyncVar(hook = nameof(OnCursedInkStatusChanged))]
    [SerializeField] private bool isCursedInked = false;
    [SyncVar(hook = nameof(OnDistortedVisionStatusChanged))]
    [SerializeField] private bool isDistorted = false;
    [SyncVar(hook = nameof(OnShieldProtectionStatusChanged))]
    [SerializeField] private bool isProtected = false;

    private ExtendedNetworkManager room;
    private GameManager gameManager;
    private GameUIManager gameUIManager;
    private MiniGameManager miniGameManager;

    #region getters
    public ExtendedNetworkManager Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as ExtendedNetworkManager;
        }
    }

    public GameUIManager GameUIManager
    {
        get
        {
            if(gameUIManager != null) { return gameUIManager; }
            return gameUIManager = FindObjectOfType<GameUIManager>();
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

    public MiniGameManager MiniGameManager
    {
        get
        {
            if (miniGameManager != null) { return miniGameManager; }
            return miniGameManager = FindObjectOfType<MiniGameManager>();
        }
    }

    public string GetPlayerName()
    {
        return displayName;
    }

    public Team GetTeam()
    {
        return team;
    }

    public Sprite GetSprite()
    {
        return GameManager.GetSprite(spriteID);
    }

    public uint GetHealthPoints()
    {
        return healthPoints;
    }

    public bool IsAlive()
    {
        return isAlive;
    }

    public bool IsPoisoned()
    {
        return isPoisoned;
    }

    public bool IsCursedInked()
    {
        return isCursedInked;
    }

    public bool IsDistorted()
    {
        return isDistorted;
    }

    public bool IsProtected()
    {
        return isProtected;
    }

    public bool HasAbility()
    {
        return hasAbility;
    }

    [Client]
    public bool IsTutorialEnabled()
    {
        if(PlayerPrefs.HasKey("isTutorialEnabled"))
        {
            return PlayerPrefs.GetInt("isTutorialEnabled") == 1;
        }
        PlayerPrefs.SetInt("isTutorialEnabled", 1);
        return true;
    }



    #endregion


    #region setters

    [Client]
    public void ToggleTutorial(bool isEnabled)
    {
        if (isEnabled)
        {
            PlayerPrefs.SetInt("isTutorialEnabled", 1);
        }
        else
        {
            PlayerPrefs.SetInt("isTutorialEnabled", 0);
        }
    }

    [Server]
    public void SetDisplayName(string name)
    {
        displayName = name;
    }

    [Server]
    public void SetHealthPoints(uint healthPoints)
    {
        if (healthPoints < 0 || healthPoints > 100) { Debug.LogError("Health points have to be in range [0:100]"); }
        this.healthPoints = healthPoints;
        if (healthPoints <= 0)
        {
            GameManager.ServerKillPlayer(netId);
        }
    }

    [Server]
    public void Heal(uint healAmount)
    {
        if (healthPoints + healAmount > 100)
        {
            healthPoints = 100;
        }
        else
        {
            healthPoints += healAmount;
        }
    }

    [Server]
    public void TakeDamage(uint damageAmount)
    {
        healthPoints -= damageAmount;
        if(healthPoints <= 0)
        {
            GameManager.ServerKillPlayer(netId);
        }
    }

    [Server]
    public void TakePoisonDamage()
    {
        if(((int) healthPoints - 25) <= 0) { SetHealthPoints(0); }
        else
        {
            healthPoints -= 25;
        }
    }

    [Server]
    public void SetPoisoning(bool isPoisoned)
    {
        this.isPoisoned = isPoisoned;
    }

    [Server]
    public void SetCursedInked(bool isCursed)
    {
        this.isCursedInked = isCursed;
    }

    [Server]
    public void SetDistortedVision(bool isDistorted)
    {
        this.isDistorted = isDistorted;
    }

    [Server]
    public void SetProtection(bool isProtected)
    {
        this.isProtected = isProtected;
    }



    [ClientRpc]
    public void ClientSetSprite(int spriteID)
    {
        this.spriteID = spriteID;
        UpdateSpriteUI();
    }

    [Server]
    public void ServerSetTeam(Team team)
    {
        this.team = team;
        ClientSetTeam(team);
    }

    [ClientRpc]
    public void ClientSetTeam(Team team)
    {
        this.team = team;
        if (isLocalPlayer && hasAuthority)
        {
            UpdateHeaderUI();
        }
    }

    [ClientRpc]
    public void ClientAssignToPosition()
    {
        GameUIManager.AssignPlayerToPosition(this);
    }

    [ClientRpc]
    public void AssignAbility(int abilityID)
    {
        this.abilityID = abilityID;
        hasAbility = true;
        if (isLocalPlayer && hasAuthority)
        {
            UpdateAbilityUI();
        }
    }
    #endregion

    [Client]
    private void UpdateHeaderUI()
    {
        if(!isLocalPlayer) { return; }
        if(team == Team.TRAITOR)
        {
            GameUIManager.SetHeaderTeamText("Traitor");
            GameUIManager.SetHeaderTraitorIcon();
        }
        else if(team == Team.KNIGHT)
        {
            GameUIManager.SetHeaderTeamText("Knight");
            GameUIManager.SetHeaderKnightIcon();
        }
        else
        {
            GameUIManager.SetHeaderTeamText("NONE");
        }
    }
    
    [Client]
    private void UpdateSpriteUI()
    {
        GameUIManager.SetPlayerSprite(this);
    }

    [Server]
    public void Kill()
    {
        isAlive = false;
        SetPoisoning(false);
        healthPoints = 0;
    }

    [Server]
    public void Revive()
    {
        if(team == Team.KNIGHT)
        {
            SetHealthPoints(50);
        }
        else
        {
            SetHealthPoints(100);
        }
        isAlive = true;
        TargetRevive();
    }

    [ClientRpc]
    public void TargetRevive()
    {
        if(Room.PlayerGameLocal == this)
        {
            GameUIManager.DisplayScrollInfo("You were revivd by one of your team mates!");
        }
        GameUIManager.SetPositionAlive(this);
    }

    [Client]
    private void UpdateAbilityUI()
    {
        StartCoroutine(AbilitySpinningCoroutine());
    }

    IEnumerator AbilitySpinningCoroutine()
    {
        GameUIManager.StartAbilitySpinning();

        yield return new WaitForSeconds(2);

        GameUIManager.StopAbilitySpinning();
        GameUIManager.SetAbility(GameManager.GetAbility(abilityID));
        GameUIManager.SetAbilityUsable();
        if (Room.PlayerGameLocal.IsTutorialEnabled())
        {
            GameUIManager.DisplayTutorial("At sunrise players heal 15hp and if poisoned, knights lose 25hp after being healed. " +
            "Also everyone gets a new ability if a knight died at night. So think carefully, do you want to use your ability or gamble and keep it for the next round?");
            GameUIManager.DisplayHighlight(Highlights.Highlight.AbilityArea);
            GameUIManager.DisplayHighlight(Highlights.Highlight.Lifebar);
        }
        
    }

    [ClientRpc]
    private void RpcHandleMessage(string message)
    {
        ChatBehavior activeChat = FindObjectOfType<ChatBehavior>();
        if(activeChat != null)
        {
            activeChat.InvokeNewMessage(message);
        }
    }

    #region event hooks

    public void OnHealthPointsChanged(uint oldValue, uint newValue)
    {
        if(isLocalPlayer && hasAuthority)
        {
            GameUIManager.updatePlayerHealthbar(healthPoints);
            if(oldValue < newValue)
            {
                GameUIManager.DisplayPlayerHealthIncreaseAnimation((newValue - oldValue));
            }
            else if(oldValue > newValue)
            {
                GameUIManager.DisplayPlayerHealthDecreaseAnimation((oldValue - newValue));
            }
        }
    }

    public void OnPoisoningStatusChanged(bool oldValue, bool newValue)
    {
        if(!hasAuthority) { return; }
        if(!isAlive) { return; }
        if(newValue)
        {
            GameUIManager.DisplayScrollInfo("You have been poisoned! " +
                "You will lose 25 hp at each sunrise unless you get cured!");
            GameUIManager.DisplayToxifiedStatusInfo();
        }
        else
        {
            GameUIManager.DisplayScrollInfo("You poisining was cured by another player.");
            GameUIManager.HideAllStatusInfo();
        }
    }

    public void OnCursedInkStatusChanged(bool oldValue, bool newValue)
    {
        if (!hasAuthority) { return; }
        if (newValue)
        {
            GameUIManager.DisplayScrollInfo("Your ink has been cursed! " +
                "You cannot use the chat until sunrise.");
            GameUIManager.DisplayCursedStatusInfo();
        }
        else
        {
            GameUIManager.HideAllStatusInfo();
        }
    }

    public void OnDistortedVisionStatusChanged(bool oldValue, bool newValue)
    {
        if (!hasAuthority) { return; }
        if (newValue)
        {
            GameUIManager.DisplayScrollInfo("Your vision has been distorted! " +
                "You will vote randomly until sunrise.");
            GameUIManager.DisplayBlindedStatusInfo();
        }
        else
        {
            GameUIManager.HideAllStatusInfo();
        }
    }

    public void OnShieldProtectionStatusChanged(bool oldValue, bool newValue)
    {
        if (!hasAuthority) { return; }
        if (newValue)
        {
            GameUIManager.DisplayScrollInfo("You are protected by the shield" +
                " of protection! You cannot be attacked at night.");
            GameUIManager.DisplayProtectedStatusInfo();
        }
        else
        {
            GameUIManager.HideAllStatusInfo();
        }
    }

    #endregion

    #region commands
    [Command]
    public void CmdSendTraitorAttack(string attackName)
    {
        MiniGameManager.selectedAttack = attackName;
    }

    [Command]
    public void CmdSendKnightDefense(string defenseName)
    {
        MiniGameManager.selectedDefense = defenseName;
    }

    [Command]
    public void CmdVote(uint voteeNetID)
    {
        GameManager.ServerAddVote(netId, voteeNetID);
    }

    [Command]
    public void CmdReady()
    {
        GameManager.ServerIncreaseReadyCounter();
    }

    [Command]
    public void CmdUseAbility()
    {
        if (!hasAbility || !isAlive) { return; }

        hasAbility = false;
        Ability ability = GameManager.GetAbility(abilityID);
        Debug.Log(netId + " used his ability: " + ability.GetAbilityName());
        ability.ServerUse(this);
    }

    [Command]
    public void CmdSendMessage(string message)
    {
        if (isCursedInked || !isAlive) { return; }
        RpcHandleMessage($"{displayName}: {message}");
    }
    #endregion

    #region Start & Stop Callbacks

    /// <summary>
    /// Called on every NetworkBehaviour when it is activated on a client.
    /// <para>Objects on the host have this function called, as there is a local client on the host. The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.</para>
    /// </summary>
    public override void OnStartClient() 
    {
        DontDestroyOnLoad(gameObject);

        Room.GamePlayers.Add(this);
    }

    /// <summary>
    /// This is invoked on clients when the server has caused this object to be destroyed.
    /// <para>This can be used as a hook to invoke effects or do client specific cleanup.</para>
    /// </summary>
    public override void OnStopClient() 
    {
        Room.GamePlayers.Remove(this);
    }

    /// <summary>
    /// This is invoked on behaviours that have authority, based on context and <see cref="NetworkIdentity.hasAuthority">NetworkIdentity.hasAuthority</see>.
    /// <para>This is called after <see cref="OnStartServer">OnStartServer</see> and before <see cref="OnStartClient">OnStartClient.</see></para>
    /// <para>When <see cref="NetworkIdentity.AssignClientAuthority"/> is called on the server, this will be called on the client that owns the object. When an object is spawned with <see cref="NetworkServer.Spawn">NetworkServer.Spawn</see> with a NetworkConnection parameter included, this will be called on the client that owns the object.</para>
    /// </summary>
    public override void OnStartAuthority() 
    {
        Room.PlayerGameLocal = this;
    }

    #endregion
}
