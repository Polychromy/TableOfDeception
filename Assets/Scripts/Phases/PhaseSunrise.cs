using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

public class PhaseSunrise : Phase
{
    private bool isFirstTime = true;
    System.Random rand = new System.Random();

    [ClientRpc]
    public override void ClientDeInitializeUI()
    {
        UIManager.HideAllBackgrounds();
        UIManager.HideHeaderArrows();
        UIManager.HideAllScrolls();
        UIManager.HideHeaderTeamText();
        UIManager.HidePlayerPositions();
        UIManager.HideFooterCountdown();
        UIManager.HideHighlight(Highlights.Highlight.AbilityArea);
        UIManager.HideHighlight(Highlights.Highlight.Lifebar);
        UIManager.HideHighlight(Highlights.Highlight.Player);
        UIManager.HideHighlight(Highlights.Highlight.Team);
        UIManager.HideHighlight(Highlights.Highlight.StatusIcon);
    }

    [ClientRpc]
    public override void ClientInitializeUI()
    {
        UIManager.DisplayChat();
        UIManager.PhasenTextAnimation("sunrise");
        UIManager.DisplayBackgroundSunrisephase();
        UIManager.DisplayHeaderArrows();
        UIManager.DisplayHeaderTeamText();
        UIManager.HidePlayerPositions();
        UIManager.DisplayPlayerPositions();
        UIManager.SetAbilityUnUsable();
    }

    [ClientRpc]
    public override void ClientExecute()
    {
        if (isFirstExecution)
        {
            UIManager.HideLoadingScreen();
            UIManager.HighlightMe(Room.PlayerGameLocal);
            UIManager.HideAllStatusInfo();
            phaseActions.Add(ActionInfoDeath);
            phaseActions.Add(ActionAbilityUse);
            isFirstExecution = false;
        }
        else
        {
            Room.PlayerGameLocal.ToggleTutorial(false);
        }

        base.ClientExecute();
    }

    public void ActionInfoDeath()
    {
        if(isFirstTime && Room.PlayerGameLocal.GetTeam() == Team.TRAITOR)
        {
            List<NetworkPlayerGame> traitors = GameManager.GetTraitorPlayers();
            foreach(NetworkPlayerGame traitor in traitors)
            {
                UIManager.SetPlayerStatus(traitor.netId, false);
            }

            UIManager.DisplayAllPlayerStatus();
        }

        if (isFirstTime)
        {
            UIManager.DisplayPlayerStatus(Room.PlayerGameLocal.netId);
        }

        if (isServer)
        {
            GameManager.ServerToggleChat(true);

            if(isFirstTime && GameManager.GetTraitorPlayers().Count > 1)
            {
                List<NetworkPlayerGame> traitors = GameManager.GetTraitorPlayers();
                List<NetworkPlayerGame> knights = GameManager.GetKnightPlayers();

                int randomTraitor;
                if (traitors.Count == 1)
                {
                    randomTraitor = 0;
                }
                else
                {
                    randomTraitor = rand.Next(traitors.Count);
                }

                int randomKnight = rand.Next(knights.Count);

                TargetPlayerGiveVision(knights[randomKnight].connectionToClient, traitors[randomTraitor].netId);

            }

            foreach (NetworkPlayerGame player in Room.GamePlayers)
            {
                if(player.IsAlive()) { player.Heal(15); }
                if(player.IsCursedInked()) { player.SetCursedInked(false); }
                if(player.IsDistorted()) { player.SetDistortedVision(false); }
                if(player.IsProtected()) { player.SetProtection(false); }
            }

            foreach (NetworkPlayerGame poisonedPlayer in GameManager.GetPoisonedPlayers())
            {
                poisonedPlayer.TakePoisonDamage();
            }

            if (GameManager.HasPlayerDiedLastNight() || isFirstTime)
            {
                foreach (NetworkPlayerGame player in Room.GamePlayers)
                {
                    if (player.IsAlive())
                    {
                        GameManager.ServerAssignPlayerAbility(player);
                    }
                }
            }

            isFirstTime = false;
            GameManager.ServerStartTimer(1);
        }
        UIManager.HideFooterCountdown();
        if(Room.PlayerGameLocal.IsTutorialEnabled())
        {
            UIManager.DisplayHighlight(Highlights.Highlight.Player);
            UIManager.DisplayHighlight(Highlights.Highlight.Team);
            UIManager.DisplayHighlight(Highlights.Highlight.StatusIcon);
        }
    }

    public void ActionAbilityUse()
    {
        if(isServer)
        {
            if(Room.UseTimer)
            {
                GameManager.ServerStartTimer(40);
            }
            GameManager.ServerEnableReadyFeature();
            GameManager.ServerEnableAbilityUse();
        }
           
        if(GameManager.UseTimer)
        {
            UIManager.DisplayFooterCountdown();
        }
    }

    [TargetRpc]
    public void TargetPlayerGiveVision(NetworkConnection target, uint visionedPlayerID)
    {
        string name = GameManager.GetPlayer(visionedPlayerID).GetPlayerName();
        UIManager.DisplayScrollInfo("You had a vision!\n\n You saw that " + name + " is a traitor!\n\nThis was the only vision anybody will ever have.\n\nTry to convince your teammates that you speak the truth!");
    }

}
