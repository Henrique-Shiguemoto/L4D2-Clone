using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHealthSystem : MonoBehaviour{
    public int maxHealth;
    public int zombieDamage;
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
