using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MenuPlayerNameInput : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField nameInputField = null;
    [SerializeField] private TMP_Text warningText = null;
    [SerializeField] private Button continueButton = null;

    public static string DisplayName { get; private set; }

    private const string PlayerPrefsNameKey = "PlayerName";

    private void Start() => SetUpInputField();

    private void SetUpInputField()
    {
        if (!PlayerPrefs.HasKey(PlayerPrefsNameKey)) 
        {
            SetPlayerName("");
            return; 
        }

        string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);

        nameInputField.text = defaultName;

        SetPlayerName(defaultName);
    }

    public void SetPlayerName()
    {
        nameInputField.characterValidation = TMP_InputField.CharacterValidation.Alphanumeric;
        if(nameInputField.text.Length < 2)
        {
            warningText.text = "Your name should be at least 2 characters long and should be alphanumerical.";
            warningText.gameObject.SetActive(true);
            continueButton.interactable = false;
        }
        else if(nameInputField.text.Length > 10)
        {
            warningText.text = "Your name should be no longer than 10 characters long and should be alphanumerical.";
            warningText.gameObject.SetActive(true);
            continueButton.interactable = false;
        }
        else
        {
            continueButton.interactable = true;
            warningText.gameObject.SetActive(false);
        }
        
        nameInputField.text = TruncateName(nameInputField.text);
        SavePlayerName();
    }

    public void SetPlayerName(string name)
    {
        continueButton.interactable = !string.IsNullOrEmpty(name);
        SavePlayerName();
    }

    public void SavePlayerName()
    {
        DisplayName = TruncateName(nameInputField.text);

        PlayerPrefs.SetString(PlayerPrefsNameKey, DisplayName);
    }

    private string TruncateName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        return name.Substring(0, Math.Min(name.Length, 10));
    }
}
