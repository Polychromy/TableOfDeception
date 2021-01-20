using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [Header("Scenes")]
    [SerializeField] private string hostGameScene = string.Empty;
    [SerializeField] private string joinGameScene = string.Empty;

    public void OnClickHostGame() 
    {
        SceneManager.LoadScene(hostGameScene);
    }

    public void OnClickJoinGame()
    {
        SceneManager.LoadScene(joinGameScene);
    }
}
