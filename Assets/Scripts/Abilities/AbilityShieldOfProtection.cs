using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AbilityShieldOfProtection : Ability
{

    [TargetRpc]
    public override void Response(NetworkConnection target, AbilityResponse response)
    {
    }

    [Server]
    public override void ServerUse(NetworkPlayerGame user)
    {
        // very simple am I right? :)
        base.ServerUse(user);
        user.SetProtection(true);
    }
}
