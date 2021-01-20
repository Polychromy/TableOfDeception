using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{

    private ExtendedNetworkManager room;
    private GameManager gameManager;

    [SerializeField] private Sprite[] playerSprites;
    private List<int> usedSpriteIDs = new List<int>();

    private System.Random rand = new System.Random();

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

    public List<NetworkPlayerGame> GetPoisonedPlayers()
    {
        List<NetworkPlayerGame> poisonedPlayers = new List<NetworkPlayerGame>();
        foreach (NetworkPlayerGame player in Room.GamePlayers)
        {
            if (player.IsPoisoned())
            {
                poisonedPlayers.Add(player);
            }
        }

        return poisonedPlayers;
    }

    public List<NetworkPlayerGame> GetKnightPlayers()
    {
        List<NetworkPlayerGame> knights = new List<NetworkPlayerGame>();
        foreach(NetworkPlayerGame player in Room.GamePlayers)
        {
            if(player.GetTeam() == Team.KNIGHT)
            {
                knights.Add(player);
            }
        }

        return knights;
    }

    public List<NetworkPlayerGame> GetTraitorPlayers()
    {
        List<NetworkPlayerGame> traitors = new List<NetworkPlayerGame>();
        foreach (NetworkPlayerGame player in Room.GamePlayers)
        {
            if (player.GetTeam() == Team.TRAITOR)
            {
                traitors.Add(player);
            }
        }

        return traitors;
    }

    public List<NetworkPlayerGame> GetAlivePlayers()
    {
        List<NetworkPlayerGame> alivePlayers = new List<NetworkPlayerGame>();
        foreach (NetworkPlayerGame player in Room.GamePlayers)
        {
            if (player.IsAlive())
            {
                alivePlayers.Add(player);
            }
        }

        return alivePlayers;
    }

    public List<NetworkPlayerGame> GetAliveTraitors()
    {
        List<NetworkPlayerGame> alivePlayers = new List<NetworkPlayerGame>();
        foreach (NetworkPlayerGame player in Room.GamePlayers)
        {
            if (player.IsAlive() && player.GetTeam() == Team.TRAITOR)
            {
                alivePlayers.Add(player);
            }
        }

        return alivePlayers;
    }

    public List<NetworkPlayerGame> GetAliveKnights()
    {
        List<NetworkPlayerGame> alivePlayers = new List<NetworkPlayerGame>();
        foreach (NetworkPlayerGame player in Room.GamePlayers)
        {
            if (player.IsAlive() && player.GetTeam() == Team.KNIGHT)
            {
                alivePlayers.Add(player);
            }
        }

        return alivePlayers;
    }

    public Sprite GetSprite(int spriteID)
    {
        if(spriteID >= playerSprites.Length) { return null; }
        return playerSprites[spriteID];
    }



    [Server]
    public void AssignPlayerTeams()
    {
        uint traitorcount = Room.TraitorPlayerCount;
        if(traitorcount <= 0) { traitorcount = 1; }

        List<NetworkPlayerGame> players = new List<NetworkPlayerGame>(Room.GamePlayers);

        // first assign traitors randomly
        for(int i = 0; i < traitorcount; i++)
        {
            int result = rand.Next(players.Count);
            players[result].ServerSetTeam(Team.TRAITOR);
            players.RemoveAt(result);
        }

        // assign knights
        foreach(NetworkPlayerGame player in players)
        {
            player.ServerSetTeam(Team.KNIGHT);
        }
    }

    [Server]
    public void AssignPlayerSprite(NetworkPlayerGame player)
    {
        // get a random sprite
        int randomSpriteID = rand.Next(playerSprites.Length - 1);
        while(usedSpriteIDs.Contains(randomSpriteID))
        {
            randomSpriteID = rand.Next(playerSprites.Length - 1);
        }

        usedSpriteIDs.Add(randomSpriteID);
        player.ClientSetSprite(randomSpriteID);
    }


}
