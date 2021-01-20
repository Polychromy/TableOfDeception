using UnityEngine;
using Mirror;
using System.Collections.Generic;


/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class MiniGameManager : NetworkBehaviour
{
    protected ExtendedNetworkManager room;
    protected GameManager gameManager;
    protected GameUIManager gameUIManager;
    protected MiniGameUIManager miniGameUiManager;

    [SerializeField] private GameObject knightPrefab;
    [SerializeField] private GameObject traitorPrefab;
    private MiniGameKnight knightInstance;
    private MiniGameTraitor traitorInstance;
    [SyncVar] public string selectedAttack;
    [SyncVar] public string selectedDefense;
    private uint miniGameLoserNetID;

    public Team CurrentTeam { get; set; }
    public NetworkPlayerGame currentPlayer;

    public int damageDealtToKnightThisRound;
    public int damageDealtToTraitorThisRound;

    // instantiate random 
    System.Random random = new System.Random();

    public ExtendedNetworkManager Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as ExtendedNetworkManager;
        }
    }

    protected GameManager GameManager
    {
        get
        {
            if (gameManager != null) { return gameManager; }
            return gameManager = FindObjectOfType<GameManager>();
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

    public MiniGameKnight KnightInstance
    {
        get
        {
            if (knightInstance != null) { return knightInstance; }
            return knightInstance = FindObjectOfType<MiniGameKnight>();
        }
    }

    public MiniGameTraitor TraitorInstance
    {
        get
        {
            if (traitorInstance != null) { return traitorInstance; }
            return traitorInstance = FindObjectOfType<MiniGameTraitor>();
        }
    }

    protected MiniGameUIManager MiniGameUIManager
    {
        get
        {
            if (miniGameUiManager != null) { return miniGameUiManager; }
            return miniGameUiManager = FindObjectOfType<MiniGameUIManager>();
        }
    }

    [Server]
    public void ServerSpawnMiniGamePlayers()
    {
        GameObject currentKnightInstance = Instantiate(knightPrefab);
        NetworkServer.Spawn(currentKnightInstance);
        knightInstance = currentKnightInstance.GetComponent<MiniGameKnight>();

        GameObject currentTraitorInstance = Instantiate(traitorPrefab);
        NetworkServer.Spawn(currentTraitorInstance);
        traitorInstance = currentTraitorInstance.GetComponent <MiniGameTraitor>();
    }
    
    public void HideUIOnLoad()
    {
        UIManager.HideMinigameUI();
    }

    [Server]
    public void SetPlayerGroups()
    {
        List<NetworkPlayerGame> traitors = new List<NetworkPlayerGame>();
        //get a randomised Traitor from players and set him as current traitor.player
        
        foreach (NetworkPlayerGame traitor in GameManager.GetTraitorPlayers())
        {
            if(traitor.IsAlive())
            {
                traitors.Add(traitor);
            }
        }

        int index = random.Next(traitors.Count);

        Debug.LogWarning("Traitor is " + traitors[index].GetPlayerName());
        Debug.LogWarning("Knight is " + GameManager.GetPlayer(GameManager.GetLastVotedPlayerNetID()).GetPlayerName());

        TraitorInstance.InitializeTraitor(traitors[index]);
        KnightInstance.InitializeKnight(GameManager.GetPlayer(GameManager.GetLastVotedPlayerNetID()));
    }

    [Server]
    public void ServerDoFight()
    {
        (int damage, string unit) damageToUnit = GetDamage();

        switch (damageToUnit)
        {
            case var damageDone when damageDone.unit == "traitor":
                TraitorInstance.ServerDealDamage(damageToUnit.damage);
                RpcUpdateViewerTextbox(damageToUnit.damage, 0);
                break;
            case var damageDone when damageDone.unit == "both":
                TraitorInstance.ServerDealDamage(damageToUnit.damage);
                KnightInstance.ServerDealDamage(damageToUnit.damage);
                RpcUpdateViewerTextbox(damageToUnit.damage, damageToUnit.damage);
                break;
            default:
                KnightInstance.ServerDealDamage(damageToUnit.damage);
                RpcUpdateViewerTextbox(0, damageToUnit.damage);
                break;
        }
        
    }

    [ClientRpc]
    public void RpcUpdateViewerTextbox(int traitorDamage, int knightDamage)
    {
        MiniGameUIManager.ChangeFightViewerDescription(selectedAttack, selectedDefense, traitorDamage, knightDamage);
    }

    public bool IsPlayerDead()
    {
        return KnightInstance.healthPoints <= 0 || TraitorInstance.healthPoints <= 0;
    }

    public (int, string) GetDamage()
    {
        MiniGameAttack traitorAttack = TraitorInstance.Attacks.Find(attack => attack.attackName == selectedAttack);

        // if no attack choosen randomise attack
        if (traitorAttack == null)
        {
            int index = random.Next(TraitorInstance.Attacks.Count);
            traitorAttack = TraitorInstance.Attacks[index];
        }

        (int damage, string unit) damageToUnit = traitorAttack.GetDamage(selectedDefense);

        return damageToUnit;
    }

    public void SetMiniGameLoserNetID(uint playerNetID)
    {
        miniGameLoserNetID = playerNetID;
    }

    [Server]
    public void ServerKillPlayer()
    {
        RpcCleanUpMiniGame();
        knightInstance.Player.SetHealthPoints((uint) knightInstance.healthPoints);
        if(traitorInstance.healthPoints > 35)
        {
            traitorInstance.Player.SetHealthPoints((uint)traitorInstance.healthPoints);
        }
        else
        {
            traitorInstance.Player.SetHealthPoints(35);
        }
        
        ServerCleanUpMiniGame();
    }

    [Server]
    public void ServerCleanUpMiniGame()
    {
        knightInstance.healthPoints = 100;
        traitorInstance.healthPoints = 100;
        selectedAttack = "";
        selectedDefense = "";
    }

    [ClientRpc]
    public void RpcCleanUpMiniGame()
    {
        UIManager.HideMinigameUI();
        MiniGameUIManager.HideAnimation();
        MiniGameUIManager.ChangeFightViewerDescriptionReset();
    }

    [Server]
    public void ServerEndMinigame()
    {
        if(KnightInstance.healthPoints <= 0)
        {
            RpcDisplayTraitorWinner();
        }
        else
        {
            RpcDisplayKnightWinner();
            KnightInstance.TargetPlayerShowKnight(KnightInstance.Player.connectionToClient, TraitorInstance.Player.GetPlayerName());
        }
    }

    [ClientRpc]
    public void RpcDisplayKnightWinner()
    {
        MiniGameUIManager.DisplayFightWinAnimation("Knight");
        MiniGameUIManager.ChangeFightViewerDescriptionKnightWin();
    }

    [ClientRpc]
    public void RpcDisplayTraitorWinner()
    {
        MiniGameUIManager.DisplayFightWinAnimation("Traitor");
        MiniGameUIManager.ChangeFightViewerDescriptionTraitorWin();
    }

}
