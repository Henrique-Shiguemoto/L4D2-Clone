using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAnimations : MonoBehaviour{    
    [SerializeField] private Animator zombieAnimator;
    [SerializeField] private ZombieMovement zombieMovementScript;

    [SerializeField] private ZombieHealthSystem zombieHealthSystem;

    private const string IS_RUNNING = "IsRunning";
    private const string IS_WALKING = "IsWalking";
    private const string IS_ATTACKING = "IsAttacking";
    private const string IS_DYING = "IsDying";

    void Update(){
        zombieAnimator.SetBool(IS_RUNNING, zombieMovementScript.IsZombieRunning());
        zombieAnimator.SetBool(IS_WALKING, zombieMovementScript.IsZombieWalking());
        zombieAnimator.SetBool(IS_ATTACKING, zombieMovementScript.IsZombieAttacking());
        if(zombieHealthSystem.IsZombieDying() && !zombieHealthSystem.IsZombieAlreadyDead()){
            zombieAnimator.SetTrigger(IS_DYING);
        }
    }

    void TriggerDamageToPlayerAnimationEvent(){
        zombieHealthSystem.DealDamageToPlayer();
    }
}
