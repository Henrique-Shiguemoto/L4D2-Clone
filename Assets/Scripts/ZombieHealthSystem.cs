using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHealthSystem : MonoBehaviour {
    public int maxHealth;
    [HideInInspector] public int currentHealth;
    public int zombieDamage;
    private PlayerHealthSystem playerHealthSystem;

    [SerializeField] private Animator zombieAnimator;
    private const string IS_ATTACKING = "IsAttacking";

    private bool isDying = false;
    private bool isAlreadyDead = false;

    void Start(){
        currentHealth = maxHealth;
        playerHealthSystem = GameObject.Find("MainPlayer").GetComponent<PlayerHealthSystem>();
    }

    void Update(){
        if(isDying) isAlreadyDead = true;
    }

    public void DealDamageToPlayer(){
        playerHealthSystem.Damage(zombieDamage);
    }

    public void Damage(int amount){
        currentHealth -= amount;
        if(currentHealth <= 0){
            currentHealth = 0;
            isDying = true;
        }
    }

    public bool IsZombieDying(){
        return isDying;
    }

    public bool IsZombieAlreadyDead(){
        return isAlreadyDead;
    }
}
