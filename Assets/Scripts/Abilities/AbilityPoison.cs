using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AbilityPoison : Ability
{
    System.Random rand = new System.Random();

    [TargetRpc]
    public override void Response(NetworkConnection target, AbilityResponse response)
    {
        if (response.success)
        {
            UIManager.DisplayScrollInfo("You have poisoned " + 
                GameManager.GetPlayer(response.targetNetID).GetPlayerName() + "!");
        }
    }

    [Server]
    public override void ServerUse(NetworkPlayerGame user)
    {
        base.ServerUse(user);

        List<NetworkPlayerGame> knights = GameManager.GetAliveKnights();
        AbilityResponse response;
        int randomKnightIndex;

        // get random knight and poison him
        if (knights.Count == 1)
        {
            randomKnightIndex = 0;
        }
        else
        {
            randomKnightIndex = rand.Next(knights.Count - 1);
            knights[randomKnightIndex].SetPoisoning(true);
        }

        response.success = true;
        response.targetNetID = knights[randomKnightIndex].netId;
        Response(user.connectionToClient, response);
    }
}
