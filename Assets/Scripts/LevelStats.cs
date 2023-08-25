using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelStats : MonoBehaviour {
    [SerializeField] private PlayerHealthSystem playerHealthSystem;
    [SerializeField] private GameObject levelStatsUI;
    [SerializeField] private TextMeshProUGUI levelStatsText;

    void Update(){
        if(playerHealthSystem.IsPlayerDying()){
            StartCoroutine(OnPlayerDeath());
        }
    }

    IEnumerator OnPlayerDeath(){
        yield return new WaitForSeconds(1.0f);
        levelStatsUI.SetActive(true);
        levelStatsText.text = "Zombies Killed: " +                  "0\n" + 
                              "Headshots: " +                       "0\n" + 
                              "Headshot Accuracy: " +               "0\n" + 
                              "Health Healed: " +                   "0\n" + 
                              "Ammo Used: " +                       "0\n" + 
                              "Weapons Kills: " +                   "0\n" + 
                              "Grenades Used: " +                   "0\n" + 
                              "Grenades Kills: " +                  "0";
    }
}
