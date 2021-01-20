using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandlerReadyGame : MonoBehaviour
{
    private ExtendedNetworkManager networkManager = null;
    GameUIManager uiManager = null;

    public void Ready()
    {
        if (networkManager == null)
        {
            networkManager = FindObjectOfType<ExtendedNetworkManager>();
        }

        if (uiManager == null)
        {
            uiManager = FindObjectOfType<GameUIManager>();
        }

        gameObject.GetComponent<Button>().interactable = false;
        networkManager.PlayerGameLocal.CmdReady();
        uiManager.SetAbilityUnUsable();
    }
}
