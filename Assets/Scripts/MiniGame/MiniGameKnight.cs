using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MiniGameKnight : NetworkBehaviour
{
    public NetworkPlayerGame Player { get; set; }

    [SyncVar(hook=nameof(OnHealthPointsChanged))]
    public int healthPoints = 100;
    public uint PlayerNetID = 0;

    public List<MiniGameDefense> Defenses = new List<MiniGameDefense>();

    private ExtendedNetworkManager room;
    private GameManager gameManager;
    private MiniGameUIManager miniGameUIManager;
    private MiniGameManager miniGameManager;
    private GameUIManager gameUIManager;

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

    public MiniGameManager MiniGameManager
    {
        get
        {
            if (miniGameManager != null) { return miniGameManager; }
            return miniGameManager = FindObjectOfType<MiniGameManager>();
        }
    }

    public GameUIManager UIManager
    {
        get
        {
            if (gameUIManager != null) { return gameUIManager; }
            return gameUIManager = FindObjectOfType<GameUIManager>();
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

    [Server]
    public void InitializeKnight(NetworkPlayerGame player)
    {
        Player = player;
        healthPoints = (int) player.GetHealthPoints();
        PlayerNetID = player.netId;
        RpcSetupKnight(PlayerNetID);
    }

    [ClientRpc]
    public void RpcSetupKnight(uint netID)
    {
        Player = GameManager.GetPlayer(netID);
        PlayerNetID = netID;

        if (Room.PlayerGameLocal.netId == netID)
        {
            MiniGameManager.CurrentTeam = Room.PlayerGameLocal.GetTeam();
            InitializeAbilities();
            MiniGameUIManager.SetAbilitiesKnight();
            MiniGameUIManager.DisplayAbilities();
            MiniGameUIManager.SetKnightSelf();
        }
        else
        {
            if (MiniGameManager.TraitorInstance.PlayerNetID != Room.PlayerGameLocal.netId)
            {
                MiniGameUIManager.DisplayViewerView();
                MiniGameUIManager.SetKnightOther();
            }
            else
            {
                MiniGameUIManager.SetKnightName(Player.GetPlayerName());
            }
            
        }
    }

    [Server]
    public void ServerDealDamage(int damage)
    {
        RpcDealDamage(damage);
        if ((healthPoints - damage) < 0)
        {
            GameManager.ServerInformDeathAtNight(true);
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
        MiniGameUIManager.DisplayDamageDealtToKnightAnimation(damage);
    }

    void InitializeAbilities()
    {
        Defenses.Clear();
        MiniGameDefense headDefense = new MiniGameDefense("Crouch", "Reflects 36 'Lance Attack' damage", "Splits 14 'Backstab' damage", "No effect: 'Wild Slash' -32- & 'Raging Strike' -32- ");
        MiniGameDefense bodyDefenseSide = new MiniGameDefense("Dodge", "Reflects 32 'Wild Slash' damage", "Splits 16 'Raging Strike' damage", "No effect: 'Lance Attack' -36- & 'Backstab' -28-");
        MiniGameDefense bodyDefenseTorso = new MiniGameDefense("Cover", "Reflects 32 'Raging Strike' damage", "Splits 16 'Wild Slash' damage", "No effect: 'Lance Attack' -36- & 'Backstab' -28-");
        MiniGameDefense armDefense = new MiniGameDefense("Retreat", "Reflects 28 'Backstab' damage", "Splits 18 'Lance Attack' damage", "No effect: 'Wild Slash' -32- & 'Raging Strike' -32-");

        Defenses.Add(headDefense);
        Defenses.Add(bodyDefenseSide);
        Defenses.Add(bodyDefenseTorso);
        Defenses.Add(armDefense);
    }

    [Client]
    public void ClientSendDefense(uint defenseID)
    {
        MiniGameManager.selectedDefense = Defenses[(int)defenseID].defenseName;
        Player.CmdSendKnightDefense(Defenses[(int)defenseID].defenseName);
    }

    [TargetRpc]
    public void TargetPlayerShowKnight(NetworkConnection target, string name)
    {
        MiniGameUIManager.SetTraitorName(name);
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
