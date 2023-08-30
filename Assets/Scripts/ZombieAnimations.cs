using UnityEngine;

public class ZombieAnimations : MonoBehaviour{    
    [SerializeField] private ZombieNavigation zombieNavigationScript;
    [SerializeField] private ZombieHealthSystem zombieHealthSystem;
    private ThrowableBehavior throwableBehaviorScript;

    private Animator zombieAnimator;

    private const string IS_RUNNING = "IsRunning";
    private const string IS_WALKING = "IsWalking";
    private const string IS_ATTACKING = "IsAttacking";
    private const string IS_DYING = "IsDying";

    void Awake(){
        zombieAnimator = GetComponent<Animator>();
        throwableBehaviorScript = GameObject.Find("Weapon Holder").GetComponent<ThrowableBehavior>();
    }

    void LateUpdate(){
        // this is to cancel the animation movement, I will take care of movement myself
        transform.localPosition = new Vector3(0.0f, transform.localPosition.y, 0.0f);

        zombieAnimator.SetBool(IS_RUNNING, zombieNavigationScript.isRunning);
        zombieAnimator.SetBool(IS_WALKING, zombieNavigationScript.isWalking);
        zombieAnimator.SetBool(IS_ATTACKING, zombieNavigationScript.isAttacking);
        zombieAnimator.SetBool(IS_DYING, zombieHealthSystem.isDying);
    }

    void TriggerDamageAnimationEvent(){
        if(!throwableBehaviorScript.throwableHasBeenThrown) zombieHealthSystem.DealDamageToPlayer();
    }

    void TriggerDeathEvent(){
        zombieHealthSystem.deathEventTriggered = true;
    }
}
