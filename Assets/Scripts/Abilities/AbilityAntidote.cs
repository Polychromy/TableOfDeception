using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AbilityAntidote : Ability
{
    System.Random rand = new System.Random();

    [TargetRpc]
    public override void Response(NetworkConnection target, AbilityResponse response)
    {
        if(response.success)
        {
            UIManager.DisplayScrollInfo("You have cured " + 
                GameManager.GetPlayer(response.targetNetID).GetPlayerName() + ".");
        }
        else
        {
            UIManager.DisplayScrollInfo("No other player was poisoned when you used the antidote... " +
                "Oops, you have poisoned yourself!");
        }
    }

    [Server]
    public override void ServerUse(NetworkPlayerGame user)
    {
        base.ServerUse(user);

        List<NetworkPlayerGame> poisonedPlayers = GameManager.GetPoisonedPlayers();

        int randomKnightIndex;

        // if only one knight is playing, choose him
        if (poisonedPlayers.Count == 1)
        {
            randomKnightIndex = 0;
        }

        AbilityResponse response;
        response.targetNetID = 0;

        if (poisonedPlayers.Count == 0)
        {
            // if no knights are playing (error)
            user.SetPoisoning(true);
            response.success = false;
        }
        else
        {
            // if more than one knight is playing get random knight and remove his poisining
            randomKnightIndex = rand.Next(poisonedPlayers.Count - 1);
            poisonedPlayers[randomKnightIndex].SetPoisoning(false);
            response.targetNetID = poisonedPlayers[randomKnightIndex].netId;
            response.success = true;
        }

        Response(user.connectionToClient, response);
    }
}
