using UnityEngine;

public class MenuLobbyHost : MonoBehaviour
{
    [SerializeField] private ExtendedNetworkManager networkManager = null;

    public void HostLobby()
    {
        if(networkManager == null) { networkManager = FindObjectOfType<ExtendedNetworkManager>(); }
        networkManager.StartHost();
    }
}
