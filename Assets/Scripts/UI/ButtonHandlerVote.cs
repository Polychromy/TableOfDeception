using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHandlerVote : MonoBehaviour
{
    [SerializeField]
    private uint netID = 0;
    private bool isNetIDSet = false;
    private ExtendedNetworkManager networkManager = null;
    private GameUIManager gameUIManager;

    public GameUIManager GameUIManager
    {
        get
        {
            if (gameUIManager != null) { return gameUIManager; }
            return gameUIManager = FindObjectOfType<GameUIManager>();
        }
    }

    public void Vote()
    {
        if (!isNetIDSet)
        {
            netID = GetComponent<PlayerSlotScript>().id;
            isNetIDSet = true;
        }

        if (networkManager == null)
        {
            networkManager = FindObjectOfType<ExtendedNetworkManager>();
        }

        if (!networkManager.PlayerGameLocal.IsAlive()) { return; }

        GameUIManager.HighliteVotee(netID);
        GameUIManager.RemoveVoteOptions();
        networkManager.PlayerGameLocal.CmdVote(netID);
    }
}
