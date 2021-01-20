using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PhaseDay : Phase
{
    bool isFirstDay = true;

    [ClientRpc]
    public override void ClientDeInitializeUI()
    {
        UIManager.HideChat();
        UIManager.HideAllBackgrounds();
        UIManager.HideHeaderArrows();
        UIManager.HideAllScrolls();
        UIManager.HideHeaderTeamText();
        UIManager.HidePlayerPositions();
    }

    [ClientRpc]
    public override void ClientInitializeUI()
    {
        UIManager.DisplayChat();
        UIManager.DisplayBackgroundDayphase();
        UIManager.DisplayHeaderArrows();
        UIManager.DisplayHeaderTeamText();
        UIManager.DisplayPlayerPositions();
    }

    [ClientRpc]
    public override void ClientExecute()
    {
        if(isFirstExecution)
        {
            UIManager.PhasenTextAnimation("day");
            phaseActions.Add(ActionVote);
            phaseActions.Add(ActionVoteResult);
            phaseActions.Add(ActionKill);
            isFirstExecution = false;
        }
        
        base.ClientExecute();
    }

    public void ActionVote()
    {
        if(isFirstDay)
        {
            if(isServer)
            {
                GameManager.ServerStartTimer(7);
            }
            UIManager.DisplayFooterCountdown();
            UIManager.DisplayScrollInfo("Since it is the first day, nothing happens.");
            if(Room.PlayerGameLocal.IsTutorialEnabled())
            {
                UIManager.DisplayTutorial("Usually there is a voting at day, but the first day phase is always skipped.");
            }
        }
        else
        {
            if (isServer)
            {
                GameManager.ServerDisableAbilityUse();
                foreach (NetworkPlayerGame player in Room.GamePlayers)
                {
                    if (player.IsAlive())
                    {
                        GameManager.ServerAddVoteOption(player.netId);
                    }
                }

                GameManager.ServerStartVoting();

                if (Room.UseTimer)
                {
                    GameManager.ServerStartTimer(120);
                }
                GameManager.ServerEnableReadyFeature();
            }
            if (GameManager.UseTimer)
            {
                UIManager.DisplayFooterCountdown();
            }
            if (Room.PlayerGameLocal.IsTutorialEnabled())
            {
                UIManager.DisplayTutorial("Use the chat  for discussion.\n\nWho might be a traitor?\n\n" +
                "Cast your Vote!\n\nThe player with the most votes will be executed.");
                UIManager.DisplayHighlight(Highlights.Highlight.ChatHighlight);
                UIManager.DisplayHighlight(Highlights.Highlight.Votes);
            }
        }
    }

    public void ActionVoteResult()
    {
        if (isFirstDay)
        {
            UIManager.HideFooterCountdown();
            if (isServer)
            {
                GameManager.ServerStartTimer(1);
            }
        }
        else
        {
            UIManager.DisplayFooterCountdown();
            if (isServer)
            {
                GameManager.ServerDisplayVoteResults();
                GameManager.ServerStartTimer(6);
            }
        }
        UIManager.HideHighlight(Highlights.Highlight.ChatHighlight);
        UIManager.HideHighlight(Highlights.Highlight.Votes);
        UIManager.HideTutorial();
    }

    public void ActionKill()
    {
        if(isFirstDay)
        {
            UIManager.HideFooterCountdown();
            if(isServer)
            {
                GameManager.ServerStartTimer(1);
            }
            isFirstDay = false;
        }
        else
        {
            if (isServer)
            {
                GameManager.ServerEndVoting();

                if (GameManager.IsLastVoteResultValid())
                {
                    GameManager.ServerStartTimer(2);
                    GameManager.ServerKillPlayer(GameManager.GetLastVotedPlayerNetID());
                }
                else
                {
                    GameManager.ServerStartTimer(1);
                }
            }
            UIManager.HideFooterCountdown();
        }
    }
}
