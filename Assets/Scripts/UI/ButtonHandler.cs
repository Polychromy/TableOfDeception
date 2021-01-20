using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class ButtonHandler : MonoBehaviour
{
    private ExtendedNetworkManager manager;

    private ExtendedNetworkManager Manager
    {
        get
        {
            if (manager != null) { return manager; }
            return manager = FindObjectOfType<ExtendedNetworkManager>();
        }
    }

    public void exitGame()
    {
        Application.Quit();
    }

    public void BackToMainMenu()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            Manager.StopHost();
        }
        else if (NetworkClient.isConnected)
        {
            Manager.StopClient();
        }
        SceneManager.LoadScene("MainMenuScene");
    }
}
