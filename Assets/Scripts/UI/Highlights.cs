using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Highlights : MonoBehaviour
{
    public GameObject lifebar;
    public GameObject chatHighlight;
    public GameObject abilityArea;
    public GameObject statusIcon;
    public GameObject player;
    public GameObject votes;
    public GameObject team;
    public GameObject timer;
    
    
    private GameObject _currentHighlight;

    public enum Highlight
    {
        Lifebar,
        ChatHighlight,
        AbilityArea,
        StatusIcon,
        Player,
        Votes,
        Team,
        Timer
    }

    public void DisplayHighlight(Highlight currentHighlight)
    {
        Debug.Log("CALLED");
        switch (currentHighlight)
        {
            case Highlight.Lifebar:
                _currentHighlight = lifebar;
                break;
            case Highlight.ChatHighlight:
                _currentHighlight = chatHighlight ;
                break;
            case Highlight.AbilityArea:
                _currentHighlight = abilityArea;
                break;
            case Highlight.StatusIcon:
                _currentHighlight = statusIcon;
                break;
            case Highlight.Player:
                _currentHighlight = player;
                break;
            case Highlight.Votes:
                _currentHighlight = votes;
                break;
            case Highlight.Team:
                _currentHighlight = team;
                break;
            case Highlight.Timer:
                _currentHighlight = timer;
                break;
            
            
        }
        _currentHighlight.SetActive(true);
    }
    public void HideHighlight(Highlight currentHighlight)
    {
        switch (currentHighlight)
        {
            case Highlight.Lifebar:
                _currentHighlight = lifebar;
                break;
            case Highlight.ChatHighlight:
                _currentHighlight = chatHighlight ;
                break;
            case Highlight.AbilityArea:
                _currentHighlight = abilityArea;
                break;
            case Highlight.StatusIcon:
                _currentHighlight = statusIcon;
                break;
            case Highlight.Player:
                _currentHighlight = player;
                break;
            case Highlight.Votes:
                _currentHighlight = votes;
                break;
            case Highlight.Team:
                _currentHighlight = team;
                break;
            case Highlight.Timer:
                _currentHighlight = timer;
                break;
        }
        _currentHighlight.SetActive(false);
    }
}
