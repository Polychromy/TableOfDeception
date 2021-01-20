using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PhaseManager : NetworkBehaviour
{
    private ExtendedNetworkManager room;
    private GameManager gameManager;
    private GameUIManager gameUIManager;
    private MiniGameUIManager miniGameUIManager;

    [SerializeField] private GameObject[] phasePrefabs;
    private Phase[] phaseInstances;

    [SerializeField] private int currentPhaseIndex = int.MinValue;
    private Phase currentPhase = null;

    [SyncVar(hook =nameof(OnTimeLeftChanged))] 
    private int timeLeftSeconds = 0;
    private bool isTimerHalted = false;

    public ExtendedNetworkManager Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as ExtendedNetworkManager;
        }
    }

    public GameManager GameManager
    {
        get
        {
            if (gameManager != null) { return gameManager; }
            return gameManager = FindObjectOfType<GameManager>();
        }
    }

    public GameUIManager GameUIManager
    {
        get
        {
            if (gameUIManager != null) { return gameUIManager; }
            return gameUIManager = FindObjectOfType<GameUIManager>();
        }
    }

    public MiniGameUIManager MiniGameUIManager
    {
        get
        {
            if (miniGameUIManager != null) { return miniGameUIManager; }
            return miniGameUIManager = FindObjectOfType<MiniGameUIManager>();
        }
    }

    [Server]
    public void ServerFinishCurrentAction()
    {
        timeLeftSeconds = 3;
    }

    public bool IsTimerRunning()
    {
        return !(timeLeftSeconds <= 0);
    }

    private IEnumerator TimerCoroutine()
    {
        while (timeLeftSeconds > 0)
        {
            while(isTimerHalted)
            {
                yield return null;
            }
            yield return new WaitForSeconds(1);
            timeLeftSeconds -= 1;
        }

        timeLeftSeconds = 0;
        if (currentPhase.HasNextAction())
        {
            currentPhase.ClientExecuteNextPhaseAction();
        }
        else
        {
            ServerStartNextPhase();
        }
    }

    [Server]
    public void ServerReadyWithoutTimer()
    {
        if (currentPhase.HasNextAction())
        {
            currentPhase.ClientExecuteNextPhaseAction();
        }
        else
        {
            ServerStartNextPhase();
        }
    }

    [Server]
    public void ServerHaltTimer()
    {
        isTimerHalted = true;
    }

    [Server]
    public void ServerResumeTimer()
    {
        isTimerHalted = false;
    }

    [Server]
    public void ServerStartTimer(int time)
    {
        if (time < 0) { return; }
        timeLeftSeconds = time;
        StartCoroutine(TimerCoroutine());
    }

    public int GetTimeLeft()
    {
        return timeLeftSeconds;
    }

    [Server]
    public void ServerSpawnPhases()
    {
        phaseInstances = new Phase[phasePrefabs.Length + 1];

        int i = 0;
        foreach(GameObject phasePrefab in phasePrefabs)
        {
            GameObject currentPhaseInstance = Instantiate(phasePrefab);
            NetworkServer.Spawn(currentPhaseInstance);
            phaseInstances[i++] = currentPhaseInstance.GetComponent<Phase>();
        }
    }

    [Server]
    public void ServerRestartPhases()
    {
        if (currentPhase != null)
        {
            currentPhase.ClientDeInitializeUI();
            currentPhase.ResetPhase();
        }

        currentPhaseIndex = 0;
      
        currentPhase = phaseInstances[currentPhaseIndex];

        currentPhase.ClientInitializeUI();
        currentPhase.ClientExecute();
    }

    [Server]
    public void ServerStartNextPhase()
    {
        if(currentPhase != null) 
        {
            currentPhase.ClientDeInitializeUI();
            currentPhase.ResetPhase();
        }
        if(currentPhaseIndex == int.MinValue) { currentPhaseIndex = 0; }
        else { currentPhaseIndex = (currentPhaseIndex + 1) % phasePrefabs.Length; }

        currentPhase = phaseInstances[currentPhaseIndex];

        currentPhase.ClientInitializeUI();
        currentPhase.ClientExecute();
    }

    public void OnTimeLeftChanged(int oldValue, int newValue)
    {
        GameUIManager.SetFooterCountdownTime(newValue);
        if(MiniGameUIManager != null)
        {
            MiniGameUIManager.SetFooterCountdownTime(newValue);
        }
    }
}
