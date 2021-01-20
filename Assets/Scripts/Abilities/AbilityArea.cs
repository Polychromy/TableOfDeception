using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityArea : MonoBehaviour

{
    public SpriteRenderer ability;
    public GameObject abilityPanel;
    public AudioSource useAbilitySound;
    public AudioSource scrollAbilityFXSound;
    public AudioSource incomeAbilitySound;
    public AudioSource hover;
    
   
    

    public void OnMouseOver()
    {
        abilityPanel.transform.gameObject.SetActive(true);
        ability.transform.localPosition = new Vector3(0f, 20f, 0f);
        ability.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        
        
       
         
    }
    
    public void OnMouseExit()
    {
        ability.transform.localPosition = new Vector3(0f, 0f, 0f);
        ability.transform.localScale = new Vector3(1f, 1f, 1f);
        abilityPanel.transform.gameObject.SetActive(false);
       
                
    }

    public void OnMouseDown()
    {
        GetComponent<ButtonHandlerUseAbility>().UseAbility();
        useAbilitySound.Play();
    }

    public void UsableAbility()
    {
        ability.transform.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }
    
    public void UnUsableAbility()
    {
        ability.transform.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1, 0.25f);
    }
}
