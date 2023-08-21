using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthSystem : MonoBehaviour{
    public int maxHealth;
    
    [HideInInspector] public int currentHealth;

    void Start(){
        
    }

    void Update(){
        
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
