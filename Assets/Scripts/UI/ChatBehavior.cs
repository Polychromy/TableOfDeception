using UnityEngine;
using System;
using Mirror;
using TMPro;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class ChatBehavior : MonoBehaviour
{

    [SerializeField] private TMP_Text textChat = null;
    [SerializeField] private TMP_InputField inputChat = null;
    [SerializeField] private AudioSource messageinputsound;


    private static event Action<string> OnMessage;

    #region Start & Stop Callbacks

    public void Start() {
        OnMessage += HandleNewMessage;
    }

    public void OnDestroy()
    {
        OnMessage -= HandleNewMessage;
    }

    private void HandleNewMessage(string message) {
        textChat.text += message;
        messageinputsound.Play();
    }

    public void InvokeNewMessage(string message)
    {
        OnMessage.Invoke($"\n{message}");
    }

    public void Send()
    {
        string message = inputChat.text; 
        if(!Input.GetKeyDown(KeyCode.Return)) { return; }
        if(string.IsNullOrWhiteSpace(message)) { return; }

        ExtendedNetworkManager netMan = NetworkManager.singleton as ExtendedNetworkManager;
        netMan.PlayerGameLocal.CmdSendMessage(message);

        inputChat.text = string.Empty;
    }

    public void clearMessages()
    {
        textChat.text ="";
    }

    #endregion
}
