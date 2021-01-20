using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AbilityReanimate : Ability
{
    System.Random rand = new System.Random();

    [TargetRpc]
    public override void Response(NetworkConnection target, AbilityResponse response)
    {
        if(response.success)
        {
            string playerName = GameManager.GetPlayer(response.targetNetID).GetPlayerName();
            UIManager.DisplayScrollInfo("You have reanimated your fellow teammate " + playerName);
        }
        else
        {
            UIManager.DisplayScrollInfo("You have not revived anyone because no one of your team is dead... Use this information wisely!");
        }
    }

    [Server]
    public override void ServerUse(NetworkPlayerGame user)
    {
        base.ServerUse(user);

        AbilityResponse response;

        if (user.GetTeam() == Team.TRAITOR)
        {
            // get all alive traitors
            List<NetworkPlayerGame> traitors = new List<NetworkPlayerGame>();
            foreach (NetworkPlayerGame traitor in GameManager.GetTraitorPlayers())
            {
                if (!traitor.IsAlive())
                {
                    traitors.Add(traitor);
                }
            }

            // get random traitor and revive him
            int randomTraitorIndex = 0;
            if (traitors.Count > 1)
            {
                randomTraitorIndex = rand.Next(traitors.Count - 1);
            }
            else if (traitors.Count == 0)
            {
                response.targetNetID = 0;
                response.success = false;
                Response(user.connectionToClient, response);
                return;
            }

            response.success = true;
            response.targetNetID = traitors[randomTraitorIndex].netId;
            traitors[randomTraitorIndex].Revive();
        }
        else
        {
            // get all alive knights
            List<NetworkPlayerGame> knights = new List<NetworkPlayerGame>();
            foreach(NetworkPlayerGame knight in GameManager.GetKnightPlayers())
            {
                if(!knight.IsAlive())
                {
                    knights.Add(knight);
                }
            }

            // get random knight and revive him
            int randomKnightIndex = 0;
            if (knights.Count > 1)
            {
                randomKnightIndex = rand.Next(knights.Count - 1);
            }
            else if(knights.Count == 0)
            {
                response.targetNetID = 0;
                response.success = false;
                Response(user.connectionToClient, response);
                return;
            }

            response.success = true;
            response.targetNetID = knights[randomKnightIndex].netId;
            knights[randomKnightIndex].Revive();
        }

        Response(user.connectionToClient, response);
    }
}
