using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AbilityClairvoyance : Ability
{
    System.Random rand = new System.Random();

    [TargetRpc]
    public override void Response(NetworkConnection target, AbilityResponse response)
    {
        if(response.success)
        {
            string playerName = GameManager.GetPlayer(response.targetNetID).GetPlayerName();
            UIManager.DisplayScrollInfo(playerName + " is a knight.");
            UIManager.SetPlayerStatus(response.targetNetID, true);
            UIManager.DisplayPlayerStatus(response.targetNetID);
        }
        else
        {
            UIManager.DisplayScrollInfo("I guess you are in debug mode, because there is no knight..." +
                "otherwise, nice job fucking up this function Matthias! :/");
        }
    }

    [Server]
    public override void ServerUse(NetworkPlayerGame user)
    {
        base.ServerUse(user);

        List<NetworkPlayerGame> knights = GameManager.GetKnightPlayers();
        AbilityResponse response;

        // if no knight is playing (error)
        if(knights.Count == 0)
        {
            response.success = false;
        }
        else if (knights.Count == 1) 
        {
            // if one knight is playing (should never happen)
            response.targetNetID = knights[0].netId;
            response.success = true;
            Response(user.connectionToClient, response);
            return;
        }
        else
        {
            // get random knight and reveal his identity to the user
            int randomKnightIndex = rand.Next(knights.Count - 1);
            while (knights[randomKnightIndex].netId == Room.PlayerGameLocal.netId)
            {
                randomKnightIndex = rand.Next(knights.Count - 1);
            }

            response.targetNetID = knights[0].netId;
            response.success = true;
            Response(user.connectionToClient, response);
        }
    }
}
