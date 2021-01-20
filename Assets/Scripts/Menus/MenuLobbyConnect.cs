using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuLobbyConnect : MonoBehaviour
{
    [SerializeField] private ExtendedNetworkManager networkManager = null;

    [Header("UI")]
    [SerializeField] private TMP_InputField ipAddressInputField = null;
    [SerializeField] private Button joinButton = null;

    private void OnEnable()
    {
        ExtendedNetworkManager.OnClientConnected += HandleClientConnected;
        ExtendedNetworkManager.OnClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        ExtendedNetworkManager.OnClientConnected -= HandleClientConnected;
        ExtendedNetworkManager.OnClientDisconnected -= HandleClientDisconnected;
    }

    public void JoinLobby()
    {
        if(networkManager == null)
        {
            networkManager = FindObjectOfType<ExtendedNetworkManager>();
        }
        string ipAddress = ipAddressInputField.text;

        networkManager.networkAddress = ipAddress;
        networkManager.StartClient();

        joinButton.interactable = false;
    }

    private void HandleClientConnected()
    {
        joinButton.interactable = true;

        gameObject.SetActive(false);
    }

    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;
    }
}
