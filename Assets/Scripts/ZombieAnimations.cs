using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAnimations : MonoBehaviour{    
    [SerializeField] private ZombieNavigation zombieNavigationScript;
    [SerializeField] private ZombieHealthSystem zombieHealthSystem;

    private Animator zombieAnimator;

    private const string IS_RUNNING = "IsRunning";
    private const string IS_WALKING = "IsWalking";
    private const string IS_ATTACKING = "IsAttacking";
    private const string IS_DYING = "IsDying";
    private const string ZOMBIE_NEEDS_TO_IDLE = "ZombieNeedsToIdle";

    void Awake(){
        zombieAnimator = GetComponent<Animator>();
    }

    void Update(){
        // this is to cancel the animation movement, I will take care of movement myself
        transform.localPosition = new Vector3(0.0f, transform.localPosition.y, 0.0f);

        zombieAnimator.SetBool(IS_RUNNING, zombieNavigationScript.isRunning);
        zombieAnimator.SetBool(IS_WALKING, zombieNavigationScript.isWalking);
        zombieAnimator.SetBool(IS_ATTACKING, zombieNavigationScript.isAttacking);
        if(zombieHealthSystem.IsZombieDying() && !zombieHealthSystem.IsZombieAlreadyDead()) zombieAnimator.SetTrigger(IS_DYING);
        zombieAnimator.SetBool(ZOMBIE_NEEDS_TO_IDLE, zombieHealthSystem.ZombieNeedsToIdleBack());
    }

    void TriggerDamageToPlayerAnimationEvent(){
        zombieHealthSystem.DealDamageToPlayer();
    }
}
