using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuIPAddressInput : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField ipInputField = null;
    [SerializeField] private Button continueButton = null;

    public static string IPAddress { get; private set; }

    private const string PlayerPrefsIPKey = "IPAddress";

    private void Start() => SetUpInputField();

    private void SetUpInputField()
    {
        if (!PlayerPrefs.HasKey(PlayerPrefsIPKey))
        {
            SetIPAddress("");
            return;
        }

        string ipAddress = PlayerPrefs.GetString(PlayerPrefsIPKey);

        ipInputField.text = ipAddress;

        SetIPAddress(ipAddress);
    }

    public void SetIPAddress()
    {
        Debug.Log("Setting " + ipInputField.text);
        continueButton.interactable = !string.IsNullOrEmpty(ipInputField.text);
        SaveIPAddress();
    }

    public void SetIPAddress(string ipAddress)
    {
        continueButton.interactable = !string.IsNullOrEmpty(ipAddress);
        SaveIPAddress();
    }

    public void SaveIPAddress()
    {
        IPAddress = ipInputField.text;

        PlayerPrefs.SetString(PlayerPrefsIPKey, IPAddress);
    }
}
