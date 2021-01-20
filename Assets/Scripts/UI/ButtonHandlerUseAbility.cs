using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHandlerUseAbility : MonoBehaviour
{
    NetworkPlayerGame localPlayer = null;
    GameUIManager uiManager = null;
    public void UseAbility()
    {
        if(localPlayer == null || uiManager == null)
        {
            uiManager = FindObjectOfType<GameUIManager>();
            localPlayer = FindObjectOfType<ExtendedNetworkManager>().PlayerGameLocal;
        }


        if(!localPlayer.IsAlive() || !localPlayer.CanUseAbility || !uiManager.IsAbilityUsable) { return; }

        uiManager.SetAbilityUnUsable();
        localPlayer.CmdUseAbility();
    }
}
