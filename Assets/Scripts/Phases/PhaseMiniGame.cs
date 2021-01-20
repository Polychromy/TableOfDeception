using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;

public class PhaseMiniGame : Phase
{
    uint roundCount = 1;

    protected MiniGameManager minigameManager;
    protected MiniGameUIManager miniGameUiManager;

    protected MiniGameManager MiniGameManager
    {
        get
        {
            if (minigameManager != null) { return minigameManager; }
            return minigameManager = FindObjectOfType<MiniGameManager>();
        }
    }

    protected MiniGameUIManager MiniGameUIManager
    {
        get
        {
            if (miniGameUiManager != null) { return miniGameUiManager; }
            return miniGameUiManager = FindObjectOfType<MiniGameUIManager>();
        }
    }

    [ClientRpc]
    public override void ClientDeInitializeUI()
    {
        UIManager.HideMinigameUI();
    }

    [ClientRpc]
    public override void ClientInitializeUI()
    {
        UIManager.DisplayMinigameUI();
    }

    [ClientRpc]
    public override void ClientExecute()
    {
        if (isFirstExecution)
        {
            phaseActions.Add(ActionPrepareFight);
            phaseActions.Add(ActionFight);
            phaseActions.Add(ActionPostFight);
            phaseActions.Add(ActionFightResult);
            isFirstExecution = false;
        }

        base.ClientExecute();
    }

    public void ActionPrepareFight()
    {
        roundCount = 1;
        MiniGameUIManager.HideFooterCountdownTimer();
        MiniGameUIManager.PlayMiniGameStartSound();
        
        if (isServer)
        {
            MiniGameManager.SetPlayerGroups();
            GameManager.ServerStartTimer(2);
        }
    }

    public void ActionFight()
    {
        MiniGameUIManager.DisplayRoundAnimation(roundCount++);
        MiniGameUIManager.DisplayFooterCountdownTimer();
        if (isServer)
        {
            GameManager.ServerStartTimer(10);
        }
    }

    public void ActionPostFight()
    {
        MiniGameUIManager.HideFooterCountdownTimer();
        if (isServer)
        {
            MiniGameManager.ServerDoFight();
            GameManager.ServerStartTimer(1);
        }
        // Reset Button Selection
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ActionFightResult()
    {
        MiniGameUIManager.HideFooterCountdownTimer();
        if (isServer)
        {
            if (!MiniGameManager.IsPlayerDead())
            {
                GameManager.ServerStartTimer(1);
            }
            else
            {
                GameManager.ServerStartTimer(5);
                MiniGameManager.ServerEndMinigame();
            }
        }

        if (!MiniGameManager.IsPlayerDead())
        {
            phaseActions.Add(ActionFight);
            phaseActions.Add(ActionPostFight);
            phaseActions.Add(ActionFightResult);
        }
        else
        {
            phaseActions.Add(ActionKillPlayer);
        }

    }

    public void ActionKillPlayer()
    {
        MiniGameUIManager.HideFooterCountdownTimer();
        if (isServer)
        {
            MiniGameManager.ServerKillPlayer();
            GameManager.ServerStartTimer(1);
        }
        phaseActions.Clear();
        isFirstExecution = true;

    }
}
