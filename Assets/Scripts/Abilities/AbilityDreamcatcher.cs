using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AbilityDreamcatcher : Ability
{
    System.Random rand = new System.Random();

    [TargetRpc]
    public override void Response(NetworkConnection target, AbilityResponse response)
    {
        UIManager.DisplayScrollInfo("You used your dreamcatcher! " +
            "Now that traitors will not be able to use the chat at night.");
    }

    [Server]
    public override void ServerUse(NetworkPlayerGame user)
    {
        base.ServerUse(user);

        AbilityResponse response;

        GameManager.ServerToggleChat(false);

        response.targetNetID = 0;
        response.success = true;
        Response(user.connectionToClient, response);
    }
}
