using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MiniGameTraitor : NetworkBehaviour
{

    public NetworkPlayerGame Player { get; set; }

    [SyncVar(hook = nameof(OnHealthPointsChanged))]
    public int healthPoints = 100;
    public uint PlayerNetID = 0;

    public List<MiniGameAttack> Attacks = new List<MiniGameAttack>();

    private ExtendedNetworkManager room;
    private GameManager gameManager;
    private MiniGameUIManager miniGameUIManager;
    private MiniGameManager miniGameManager;

    public ExtendedNetworkManager Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as ExtendedNetworkManager;
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

    public MiniGameUIManager MiniGameUIManager
    {
        get
        {
            if (miniGameUIManager != null) { return miniGameUIManager; }
            return miniGameUIManager = FindObjectOfType<MiniGameUIManager>();
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

    [Server]
    public void InitializeTraitor(NetworkPlayerGame player)
    {
        InitializeAbilities();
        Player = player;
        healthPoints = (int) player.GetHealthPoints();
        PlayerNetID = player.netId;
        RpcSetupTraitor(PlayerNetID);
    }

    [ClientRpc]
    public void RpcSetupTraitor(uint netID)
    {
        Player = GameManager.GetPlayer(netID);
        PlayerNetID = netID;

        if (Room.PlayerGameLocal.netId == netID)
        {
            MiniGameManager.CurrentTeam = Room.PlayerGameLocal.GetTeam();
            InitializeAbilities();
            MiniGameUIManager.SetAbilitiesTraitor();
            MiniGameUIManager.DisplayAbilities();
            MiniGameUIManager.SetTraitorSelf();
            if(MiniGameManager.KnightInstance.Player != null)
            {
                MiniGameUIManager.SetKnightName(MiniGameManager.KnightInstance.Player.GetPlayerName());
            }
        }
        else
        {
            if (MiniGameManager.KnightInstance.PlayerNetID != Room.PlayerGameLocal.netId)
            {
                MiniGameUIManager.DisplayViewerView();
            }
            MiniGameUIManager.SetTraitorOther();
        }
    }

    [Server]
    public void ServerDealDamage(int damage)
    {
        RpcDealDamage(damage);
        if ((healthPoints - damage) < 0)
        {
            GameManager.ServerInformDeathAtNight(false);
            healthPoints = 0;
            MiniGameManager.SetMiniGameLoserNetID(Player.netId);
        }
        else
        {
            healthPoints -= damage;
        }
    }

    [ClientRpc]
    public void RpcDealDamage(int damage)
    {
        MiniGameUIManager.DisplayDamageDealtToTraitorAnimation(damage);
    }

    void InitializeAbilities()
    {
        Attacks.Clear();
        MiniGameAttack headAttack = new MiniGameAttack("Lance Attack", "Deals 36 damage", "Splits damage against 'Retreat' defense", "Returns damage against 'Crouch'", "Retreat", "Crouch", 36);
        MiniGameAttack bodyAttackSide = new MiniGameAttack("Wild Slash", "Deals 32 damage", "Splits damage against 'Cover' defense", "Returns damage against 'Dodge'", "Cover", "Dodge", 32);
        MiniGameAttack bodyAttackTorso = new MiniGameAttack("Raging Strike", "Deals 32 damage", "Splits damage against 'Dodge' defense", "Returns damage against 'Cover'", "Dodge", "Cover", 32);
        MiniGameAttack backstab = new MiniGameAttack("Backstab", "Deals 28 damage", "Splits damage against 'Crouch' defense", "Returns damage against 'Retreat'", "Crouch", "Retreat", 28);

        Attacks.Add(headAttack);
        Attacks.Add(bodyAttackSide);
        Attacks.Add(bodyAttackTorso);
        Attacks.Add(backstab);
    }

    [Client]
    public void ClientSendAttack(uint attackID)
    {
        MiniGameManager.selectedAttack = Attacks[(int) attackID].attackName;
        Player.CmdSendTraitorAttack(Attacks[(int)attackID].attackName);
    }

    public void OnHealthPointsChanged(int oldValue, int newValue)
    {
        MiniGameUIManager.UpdateHealthBars();
        if (newValue <= 0)
        {
            MiniGameUIManager.ChangeViewCauseOfWin();
        }
    }
}
