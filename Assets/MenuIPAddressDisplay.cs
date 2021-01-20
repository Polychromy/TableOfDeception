using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Net.Http;

public class MenuIPAddressDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private TMP_Text text;
    private float timeout = 10f;
    private bool ipSet = false;

    private void Start()
    {
        SetIP();
    }

    private async void SetIP()
    {
        var httpClient = new HttpClient();
        var ip = await httpClient.GetStringAsync("https://api.ipify.org");
        text.text = ip;
        ipSet = true;
        Debug.Log("DONE!");
    }

    private void Update()
    {
        if(!ipSet)
        {
            timeout -= Time.deltaTime;
            if (timeout <= 0)
            {
                Debug.Log("RETRYING!");
                timeout = 10;
                SetIP();
            }
        }
        
    }
}
