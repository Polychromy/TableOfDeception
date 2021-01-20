using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PhaseNight : Phase
{
    [ClientRpc]
    public override void ClientDeInitializeUI()
    {
        UIManager.HideChat();
        UIManager.HideTutorial();
        UIManager.HideAllBackgrounds();
        UIManager.HideHeaderArrows();
        UIManager.HideAllScrolls();
        UIManager.HideHeaderTeamText();
        UIManager.HidePlayerPositions();
    }

    [ClientRpc]
    public override void ClientInitializeUI()
    {
        if(Room.PlayerGameLocal.GetTeam() == Team.TRAITOR)
        {
            if(GameManager.IsChatEnabled())
            {
                UIManager.DisplayChat();
            }
            else
            {
                UIManager.DisplayScrollInfo("Your chat is disabled, because someone used a dreamcatcher! \n\n The chat should be back next night.");
            }   
        }
        else
        {
            UIManager.DisplayScrollInfo("It is night. \n\n You sleep and hope to wake up unharmed in the morning.");
        }
        if (Room.PlayerGameLocal.IsTutorialEnabled())
        {
            UIManager.DisplayTutorial("The traitors now choose a knight they want to attack.\n\nThe chosen knight will fight in the Minigame against a random traitor.\n\nIf a knight loose, he dies in the game.\n\nIf a traitor loose, his identity will be revealed to the chosen knight.");
        }
        UIManager.PhasenTextAnimation("night");
        UIManager.DisplayBackgroundNightphase();
        UIManager.DisplayHeaderArrows();
        UIManager.DisplayHeaderTeamText();
        UIManager.DisplayPlayerPositions();
    }

    [ClientRpc]
    public override void ClientExecute()
    {
        if (isFirstExecution)
        {
            phaseActions.Add(ActionVote);
            phaseActions.Add(ActionVoteResult);
            phaseActions.Add(ActionStartFightOrNextPhase);
            isFirstExecution = false;
        }
        
        base.ClientExecute();
    }

    public void ActionVote()
    {
        if (isServer)
        {
            foreach (NetworkPlayerGame player in Room.GamePlayers)
            {
                if (player.IsAlive() && player.GetTeam() == Team.KNIGHT)
                {
                    GameManager.ServerAddVoteOption(player.netId);
                }
            }

            GameManager.ServerStartVotingTraitor();
            if (Room.UseTimer)
            {
                GameManager.ServerStartTimer(30);
            }
            
            GameManager.ServerEnableReadyFeatureTraitorsOnly();
        }
        if (GameManager.UseTimer)
        {
            UIManager.DisplayFooterCountdown();
        }
        
    }

    public void ActionVoteResult()
    {
        if (isServer)
        {
            GameManager.ServerDisplayVoteResultsTraitor();
            GameManager.ServerStartTimer(5);
            if (GameManager.UseTimer)
            {
                GameManager.ServerEnableReadyFeatureTraitorsOnly();
            }
        }
        UIManager.DisplayFooterCountdown();
    }

    public void ActionStartFightOrNextPhase()
    {
        UIManager.HideFooterCountdown();
        if (isServer)
        {
            GameManager.ServerEndVoting();
            if(GameManager.IsLastVoteResultValid())
            {
                GameManager.ServerStartTimer(1);
            }
            else
            {
                GameManager.ServerInformDeathAtNight(false);
                GameManager.ServerRestartPhases();
            }
        }
    }
}
