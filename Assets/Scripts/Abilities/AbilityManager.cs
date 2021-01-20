using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AbilityManager : NetworkBehaviour
{
    private ExtendedNetworkManager room;
    private GameManager gameManager;
    private GameUIManager gameUIManager;

    [Header("Ability Prefabs")]
    [SerializeField] private GameObject[] abilityPrefabs;
    private List<Ability> traitorAbilities = new List<Ability>();
    private List<Ability> knightAbilities = new List<Ability>();

    /* ability loot chance variables */
    private uint[] cumuluatedTraitorAbilityChances;
    private uint[] cumuluatedKnightAbilityChances;
    private uint traitorAbilityWeight = 0;
    private uint knightAbilityWeight = 0;
    private bool isInitialized = false;
    private System.Random rand = new System.Random();

    private bool isAbilityUseAllowed = false;

    #region getters
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

    public bool IsAbilityUseAllowed()
    {
        return isAbilityUseAllowed;
    }

    public Ability GetAbility(int abilityID)
    {
        if (!isInitialized) { ClientInitializeAbilities(); }

        foreach (Ability ability in traitorAbilities)
        {
            if (abilityID == ability.GetID())
            {
                return ability;
            }
        }

        foreach (Ability ability in knightAbilities)
        {
            if (abilityID == ability.GetID())
            {
                return ability;
            }
        }

        return null;
    }

    #endregion

    #region ability use permission functions

    [Server]
    public void ServerEnableAbilityUse()
    {
        isAbilityUseAllowed = true;
        RpcEnableAbilityUse();
    }

    [Server]
    public void ServerDisableAbilityUse()
    {
        isAbilityUseAllowed = false;
        RpcDisableAbilityUse();
    }

    [ClientRpc]
    public void RpcEnableAbilityUse()
    {
        if (Room.PlayerGameLocal.HasAbility() && Room.PlayerGameLocal.IsAlive())
        {
            GameUIManager.SetAbilityUsable();
            Room.PlayerGameLocal.CanUseAbility = true;
        }
    }

    [ClientRpc]
    public void RpcDisableAbilityUse()
    {
        GameUIManager.SetAbilityUnUsable();
        Room.PlayerGameLocal.CanUseAbility = false;
    }

    #endregion

    #region ability manager initialization

    [Server]
    public void ServerSpawnAbilities()
    {
        // spawn all abilites for client and server

        int i = 0;
        Ability currentAbility;
        foreach (GameObject abilityPrefab in abilityPrefabs)
        {
            GameObject abilityInstance = Instantiate(abilityPrefab);
            NetworkServer.Spawn(abilityInstance);
            currentAbility = abilityInstance.GetComponent<Ability>();
            currentAbility.SetAbilityID((uint)i++);

            if (currentAbility.IsTraitorAbility())
            {
                // cumulate the ability loot chance weight for traitor abilites
                traitorAbilityWeight += currentAbility.GetLootChance();
                traitorAbilities.Add(currentAbility);
            }

            if (currentAbility.IsKnightAbility())
            {
                // cumulate the ability loot chance weight for knight abilites
                knightAbilityWeight += currentAbility.GetLootChance();
                knightAbilities.Add(currentAbility);
            }
        }
        ServerInitializeAbilitiesWeights();
    }

    [Server]
    private void ServerInitializeAbilitiesWeights()
    {
        // this function initializes two loot table chances for the knight and the traitor
        cumuluatedKnightAbilityChances = new uint[knightAbilities.Count];
        cumuluatedTraitorAbilityChances = new uint[traitorAbilities.Count];

        uint currentChance = 0;

        for (int i = 0; i < knightAbilities.Count; i++)
        {
            currentChance += knightAbilities[i].GetLootChance();
            cumuluatedKnightAbilityChances[i] = currentChance;
        }

        currentChance = 0;
        for (int i = 0; i < traitorAbilities.Count; i++)
        {
            currentChance += knightAbilities[i].GetLootChance();
            cumuluatedTraitorAbilityChances[i] = currentChance;
        }
    }

    [Server]
    public void ServerAssignPlayerAbility(NetworkPlayerGame player)
    {
        // randomly assigns a player an item depending on their team with 
        // taking loot chance into account
        if (player.GetTeam() == Team.TRAITOR)
        {
            int rolledChance = rand.Next((int)traitorAbilityWeight - 1);
            player.AssignAbility(GetAbilityIDByChance(Team.TRAITOR, rolledChance));
        }
        else
        {
            int rolledChance = rand.Next((int)knightAbilityWeight - 1);
            player.AssignAbility(GetAbilityIDByChance(Team.KNIGHT, rolledChance));
        }
    }

    private int GetAbilityIDByChance(Team team, int chance)
    {
        // returns an ability based on the "rolled" value
        if (team == Team.TRAITOR)
        {
            for (int i = 0; i < cumuluatedTraitorAbilityChances.Length; i++)
            {
                if (chance < cumuluatedTraitorAbilityChances[i])
                {
                    return (int)traitorAbilities[i].GetID();
                }
            }
        }
        else
        {
            for (int i = 0; i < cumuluatedKnightAbilityChances.Length; i++)
            {
                if (chance < cumuluatedKnightAbilityChances[i])
                {
                    return (int)knightAbilities[i].GetID();
                }
            }
        }

        return -1;
    }

    [Client]
    private void ClientInitializeAbilities()
    {
        // intialized abilities on the client
        Ability[] abilities = FindObjectsOfType<Ability>();
        for (int i = 0; i < abilities.Length; i++)
        {

            if (abilities[i].IsTraitorAbility())
            {
                traitorAbilities.Add(abilities[i]);
            }

            if (abilities[i].IsKnightAbility())
            {
                knightAbilities.Add(abilities[i]);
            }
        }

        isInitialized = true;
    }

    #endregion
}
