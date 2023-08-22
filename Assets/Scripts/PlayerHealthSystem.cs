using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealthSystem : MonoBehaviour{
    public int maxHealth;
    [SerializeField] private TextMeshProUGUI healthBarText;
    [HideInInspector] public int currentHealth;

    void Start(){
        currentHealth = maxHealth;
    }

    void Update(){
        healthBarText.text = currentHealth.ToString();
    }

    void Damage(int amount){
        currentHealth -= amount;
        if(currentHealth < 0) currentHealth = 0;
    }

    void Heal(int amount){
        currentHealth += amount;
        if(currentHealth > maxHealth) currentHealth = maxHealth;
    }
}
