using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AbilityDistortedVision : Ability
{
    System.Random rand = new System.Random();

    [TargetRpc]
    public override void Response(NetworkConnection target, AbilityResponse response)
    {
        UIManager.DisplayScrollInfo("You have distorted the vision of a player from " +
            "the opponents team! They will vote randomly until sunrise!");
    }

    [Server]
    public override void ServerUse(NetworkPlayerGame user)
    {
        base.ServerUse(user);

        AbilityResponse response;

        if (user.GetTeam() == Team.KNIGHT)
        {
            List<NetworkPlayerGame> traitors = GameManager.GetAliveTraitors();

            // get random traitor and use this ability on him
            int randomTraitorIndex = 0;
            if (traitors.Count > 1)
            {
                randomTraitorIndex = rand.Next(traitors.Count - 1);
            }

            response.targetNetID = traitors[randomTraitorIndex].netId;
            traitors[randomTraitorIndex].SetDistortedVision(true);
        }
        else
        {
            List<NetworkPlayerGame> knights = GameManager.GetAliveKnights();

            // get random knight and use this ability on him
            int randomKnightIndex = 0;
            if (knights.Count > 1)
            {
                randomKnightIndex = rand.Next(knights.Count - 1);
            }

            response.targetNetID = knights[randomKnightIndex].netId;
            knights[randomKnightIndex].SetDistortedVision(true);
        }


        response.success = true;
        Response(user.connectionToClient, response);
    }
}
