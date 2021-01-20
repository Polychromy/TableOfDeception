using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

public abstract class Phase : NetworkBehaviour
{
    protected List<Action> phaseActions = new List<Action>();
    [SerializeField] protected int currentPhaseActionIndex = int.MinValue;

    protected ExtendedNetworkManager room;
    protected GameManager gameManager;
    protected GameUIManager gameUIManager;

    [Header("General")]
    [SerializeField] private string phaseName;

    protected bool isFirstExecution = true;

    #region getters
    protected ExtendedNetworkManager Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as ExtendedNetworkManager;
        }
    }

    protected GameUIManager UIManager
    {
        get
        {
            if (gameUIManager != null) { return gameUIManager; }
            return gameUIManager = FindObjectOfType<GameUIManager>();
        }
    }

    protected GameManager GameManager
    {
        get
        {
            if (gameManager != null) { return gameManager; }
            return gameManager = FindObjectOfType<GameManager>();
        }
    }

    
    public virtual bool HasNextAction()
    {
        return (currentPhaseActionIndex + 1) < phaseActions.Count;
    }

    #endregion

    [ClientRpc]
    public virtual void ClientInitializeUI() { throw new NotImplementedException(); }

    [ClientRpc]
    public virtual void ClientDeInitializeUI() { throw new NotImplementedException(); }

    [ClientRpc]
    public virtual void ClientExecute()
    {
        ExecuteNextPhaseAction();
    }

    [ClientRpc]
    public void ClientExecuteNextPhaseAction()
    {
        ExecuteNextPhaseAction();
    }

    private void ExecuteNextPhaseAction()
    {
        if (currentPhaseActionIndex == int.MinValue) { currentPhaseActionIndex = 0; }
        else
        {
            currentPhaseActionIndex = (currentPhaseActionIndex + 1) % phaseActions.Count;
        }

        phaseActions[currentPhaseActionIndex]();
    }

    [ClientRpc]
    public void ResetPhase()
    {
        currentPhaseActionIndex = int.MinValue;
    }
}
