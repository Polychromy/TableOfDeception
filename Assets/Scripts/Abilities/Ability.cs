using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public struct AbilityResponse
{
    public uint targetNetID;
    public bool success;
}

public abstract class Ability : NetworkBehaviour
{
    private ExtendedNetworkManager room;
    private GameManager gameManager;
    private GameUIManager gameUIManager;

    [Header("General")]
    [SerializeField] [SyncVar] uint abilityID;
    [SerializeField] private Sprite abilitySprite;
    [SerializeField] private string abilityName;
    [SerializeField] private string description;
    [SerializeField] private uint healthCost;
    
    [Header("Functional")]
    [SerializeField] private bool isPassive;
    [SerializeField] private bool isKnightAbility;
    [SerializeField] private bool isTraitorAbility;
    [SerializeField] private uint lootChance;


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

    public string GetAbilityName()
    {
        return abilityName;
    }

    public string GetDescription()
    {
        return description;
    }

    public bool IsPassive()
    {
        return isPassive;
    }

    public Sprite GetSprite()
    {
        return abilitySprite;
    }

    public uint GetID()
    {
        return abilityID;
    }
    
    public bool IsTraitorAbility()
    {
        return isTraitorAbility;
    }

    public bool IsKnightAbility()
    {
        return isKnightAbility;
    }

    public uint GetCost()
    {
        return healthCost;
    }

    public uint GetLootChance()
    {
        return lootChance;
    }

    #endregion

    #region setters

    [Server]
    public void SetAbilityID(uint id)
    {
        abilityID = id;
    }

    #endregion

    // called on the server to perform actions upon use of an ability
    public virtual void ServerUse(NetworkPlayerGame user)
    {
        user.TakeDamage(healthCost);
    }

    // called on the user for UI response on the users client
    [TargetRpc]
    public virtual void Response(NetworkConnection target, AbilityResponse response) 
    { 
        throw new NotImplementedException(); 
    }
}
