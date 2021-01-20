using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class VoteManager : NetworkBehaviour
{
    private class Vote
    {
        public NetworkPlayerGame Voter { get; }
        public NetworkPlayerGame Votee { get; }

        public Vote(NetworkPlayerGame voter, NetworkPlayerGame votee)
        {
            this.Voter = voter;
            this.Votee = votee;
        }
    }

    private ExtendedNetworkManager room;
    private GameManager gameManager;
    private GameUIManager gameUIManager;

    [SyncVar] private bool isLastVoteResultValid = false;
    [SyncVar] private uint lastVotedNetID = 0;
    [SyncVar] private bool isVotingActive;
    private List<uint> voteOptionIDs = new List<uint>();
    private bool isTraitorVoting = false;

    private uint votesGiven = 0;
    private Vote[] votes = new Vote[8];
    System.Random rand = new System.Random();

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

    [Server]
    public void ServerAddVoteOption(uint voteOption)
    {
        if (voteOptionIDs.Contains(voteOption)) { return; }
        voteOptionIDs.Add(voteOption);
    }

    [Server]
    public void ServerAddVote(uint voter, uint votee)
    {
        if (!isVotingActive) { return; }
        Debug.Log("Received vote number " + (votesGiven + 1) +  " from " + voter + " for " + votee);
        Vote vote = new Vote(GameManager.GetPlayer(voter), GameManager.GetPlayer(votee));
        votes[votesGiven++] = vote;
        RpcAddVote(voter, votee);
    }

    [ClientRpc]
    private void RpcAddVote(uint voter, uint votee)
    {
        GameUIManager.AddVote(voter, votee);
    }

    [Server]
    public void ServerStartVoting()
    {
        votesGiven = 0;
        isLastVoteResultValid = false;
        lastVotedNetID = 0;
        isVotingActive = true;
        uint[] tempVoteOptionIDs = voteOptionIDs.ToArray();
        RpcStartVoting(tempVoteOptionIDs);
    }

    [Server]
    public void ServerStartVotingTraitor()
    {
        votesGiven = 0;
        isLastVoteResultValid = false;
        isTraitorVoting = true;
        lastVotedNetID = 0;
        isVotingActive = true;
        uint[] tempVoteOptionIDs = voteOptionIDs.ToArray();
        RpcStartVotingTraitor(tempVoteOptionIDs);
    }

    [ClientRpc]
    private void RpcStartVotingTraitor(uint[] voteOptionIDArr)
    {
        if (Room.PlayerGameLocal.GetTeam() != Team.TRAITOR) { return; }
        if (!Room.PlayerGameLocal.IsAlive()) { return; }
        if(Room.PlayerGameLocal.IsDistorted())
        {
            int randomPlayerIndex = rand.Next(voteOptionIDArr.Length - 1);
            Room.PlayerGameLocal.CmdVote(voteOptionIDArr[randomPlayerIndex]);
            GameUIManager.DisplayScrollInfo("Oh no! You have voted randomly " +
                "because your vision was distorted...");
            GameUIManager.ResetAllVotes();
            return;
        }

        List<uint> tempVoteOptionIDs = new List<uint>();

        // first try to fill the list of options without the own player
        for (int i = 0; i < voteOptionIDArr.Length; i++)
        {
            if (Room.PlayerGameLocal.netId != voteOptionIDArr[i])
            {

                tempVoteOptionIDs.Add(voteOptionIDArr[i]);
            }
        }

        GameUIManager.PhasenTextAnimation("vote now");
        GameUIManager.ResetAllVotes();
        GameUIManager.AddVoteOptions(tempVoteOptionIDs.ToArray());
    }

    [ClientRpc]
    private void RpcStartVoting(uint[] voteOptionIDArr)
    {
        if(!Room.PlayerGameLocal.IsAlive()) { return; }
        if (Room.PlayerGameLocal.IsDistorted())
        {
            int randomPlayerIndex = rand.Next(voteOptionIDArr.Length - 1);
            Room.PlayerGameLocal.CmdVote(voteOptionIDArr[randomPlayerIndex]);
            GameUIManager.DisplayScrollInfo("Oh no! You have voted randomly " +
                "because your vision was distorted...");
            GameUIManager.ResetAllVotes();
            return;
        }

        List<uint> tempVoteOptionIDs = new List<uint>();

        // first try to fill the list of options without the own player
        for (int i = 0; i < voteOptionIDArr.Length; i++)
        {
            if (Room.PlayerGameLocal.netId != voteOptionIDArr[i])
            {

                tempVoteOptionIDs.Add(voteOptionIDArr[i]);
            }
        }
        GameUIManager.PhasenTextAnimation("vote now");
        GameUIManager.ResetAllVotes();
        GameUIManager.AddVoteOptions(tempVoteOptionIDs.ToArray());
    }

    [Server]
    public void ServerEndVoting()
    {
        voteOptionIDs.Clear();
        isVotingActive = false;
        RpcEndVoting();
        ServerCalculateVoteResult();
    }

    [Server]
    public void ServerCalculateVoteResult()
    {
        Dictionary<uint, uint> votees = new Dictionary<uint, uint>();

        if (votesGiven == 0 || (GameManager.GetAlivePlayer().Count - votesGiven) >= votesGiven && !isTraitorVoting) 
        { 
            isLastVoteResultValid = false;
            return;
        }

        for(int i = 0; i < votesGiven; i++)
        {
            if (votees.ContainsKey(votes[i].Votee.netId))
            {
                votees[votes[i].Votee.netId]++;
            }
            else
            {
                votees.Add(votes[i].Votee.netId, 1);
            }
        }

        uint highestVotee = uint.MaxValue;
        uint highestVotes = 0;
        bool twoVotesSame = false;

        foreach (KeyValuePair<uint, uint> votee in votees)
        {
            // first
            if(highestVotee == uint.MaxValue) 
            { 
                highestVotee = votee.Key;
                highestVotes = votee.Value;
                continue;
            }

            // if higher
            if (votee.Value > highestVotes)
            {
                twoVotesSame = false;
                highestVotee = votee.Key;
                highestVotes = votee.Value;
                continue;
            }

            //if equal
            if(votee.Value == highestVotes)
            {
                twoVotesSame = true;
                continue;
            }
        }

        if(twoVotesSame)
        {
            isLastVoteResultValid = false;
        }
        else
        {
            //
            if(isTraitorVoting)
            {
                if(GameManager.GetPlayer(highestVotee).IsProtected())
                {
                    isLastVoteResultValid = false;
                    RpcInformShield();
                }
                else
                {
                    lastVotedNetID = highestVotee;
                    isLastVoteResultValid = true;
                }
                isTraitorVoting = false;
            }
            else
            {
                lastVotedNetID = highestVotee;
                isLastVoteResultValid = true;
            }
        }
    }

    [ClientRpc]
    private void RpcInformShield()
    {
        GameUIManager.DisplayScrollInfo("The traitors voted a player this night" +
            " who had the shield of protection! He will be protected from an attack.");
    }

    [ClientRpc]
    private void RpcDisableVoting()
    {
        GameUIManager.RemoveVoteOptions();
    }

    [Server]
    public void ServerDisplayVoteResults()
    {
        RpcDisableVoting();
        RpcDisplayVoteResults();
    }

    [ClientRpc]
    private void RpcDisplayVoteResults()
    {
        GameUIManager.DisplayAllVotes();
    }

    [Server]
    public void ServerDisplayVoteResultsTraitor()
    {
        RpcDisableVoting();
        RpcDisplayVoteResultsTraitor();
    }

    [ClientRpc]
    private void RpcDisplayVoteResultsTraitor()
    {
        if (Room.PlayerGameLocal.GetTeam() != Team.TRAITOR) { return; }
        GameUIManager.DisplayAllVotes();
    }

    [ClientRpc]
    private void RpcEndVoting()
    {
        GameUIManager.ResetAllVotes();
        GameUIManager.HideAllVotes();
    }

    public bool IsLastVoteResultValid()
    {
        return isLastVoteResultValid;
    }

    public uint GetLastVotedPlayerNetID()
    {
        return lastVotedNetID;
    }
}
