using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHandlerUseMiniGameAbility : MonoBehaviour
{
    [SerializeField] private uint abilityID = 0;
    NetworkPlayerGame localPlayer = null;
    GameUIManager uiManager = null;
    MiniGameManager miniGameManager = null;
    public void UseAbility()
    {
        if (localPlayer == null || uiManager == null)
        {
            uiManager = FindObjectOfType<GameUIManager>();
            miniGameManager = FindObjectOfType<MiniGameManager>();
            localPlayer = FindObjectOfType<ExtendedNetworkManager>().PlayerGameLocal;
        }

        if (miniGameManager.TraitorInstance.Player == localPlayer) 
        {
            miniGameManager.TraitorInstance.ClientSendAttack(abilityID); 
        }
        else
        {
            miniGameManager.KnightInstance.ClientSendDefense(abilityID);
        }

    }
}
