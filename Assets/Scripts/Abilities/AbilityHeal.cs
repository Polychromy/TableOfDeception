using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AbilityHeal : Ability
{

    [SerializeField] uint healAmount;
    [SerializeField] uint damageAmount;
    [SerializeField] uint failChancePercentage;
    System.Random rand = new System.Random();

    [TargetRpc]
    public override void Response(NetworkConnection target, AbilityResponse response)
    {
        if (response.success)
        {
            UIManager.DisplayScrollInfo("You have healed yourself.");
        }
        else
        {
            UIManager.DisplayScrollInfo("You had an allergic reaction to the potion! You lost life.");
        }
    }

    [Server]
    public override void ServerUse(NetworkPlayerGame user)
    {
        base.ServerUse(user);

        AbilityResponse response;
        response.targetNetID = user.netId;

        // roll the "dice" and if in fail percentage area the use of this abiltiy fails (user loses life)
        int roll = rand.Next(1, 100);
        if(roll > failChancePercentage)
        {
            user.Heal(healAmount);
            response.success = true;
        }
        else
        {
            user.TakeDamage(damageAmount);
            response.success = false;
        }

        Response(user.connectionToClient, response);
    }
}
