using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class IngameMenu : MonoBehaviour
{
    public static bool IngameMenuActive = false;
    public GameObject IngameMenuUI;

    private ExtendedNetworkManager manager;

    private ExtendedNetworkManager Manager
    {
        get
        {
            if (manager != null) { return manager; }
            return manager = FindObjectOfType<ExtendedNetworkManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IngameMenuActive)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        IngameMenuUI.SetActive(false);
        IngameMenuActive = false;
    }

    void Pause()
    {
        IngameMenuUI.SetActive(true);
        IngameMenuActive = true;
    }

    public void BackToMainMenu()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            Manager.StopHost();
        }
        // stop client if client-only
        else if (NetworkClient.isConnected)
        {
            Manager.StopClient();
        }
        SceneManager.LoadScene("MainMenuScene");
    }

    public void QuitGame()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            Manager.StopHost();
        }
        // stop client if client-only
        else if (NetworkClient.isConnected)
        {
            Manager.StopClient();
        }
        Application.Quit();
    }
}
