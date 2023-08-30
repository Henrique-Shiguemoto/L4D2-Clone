using UnityEngine;

public class ZombieHealthSystem : MonoBehaviour {
    public int maxHealth;
    [HideInInspector] public int currentHealth;
    public int zombieDamage;
    [SerializeField] private PlayerHealthSystem playerHealthSystem;

    [SerializeField] private Animator zombieAnimator;

    private CharacterController zombieController;

    public bool isDying = false;
    public bool deathEventTriggered = false;
    public bool zombieJustRespawned = false;

    void Awake(){
        playerHealthSystem = GameObject.Find("MainPlayer").GetComponent<PlayerHealthSystem>();
        zombieController = GetComponent<CharacterController>();
    }

    void Start(){
        currentHealth = maxHealth;
    }

    void LateUpdate(){
        if(ZombieHasRespawned()){
            deathEventTriggered = false;
            currentHealth = maxHealth;
            EnableControllerOnDeath(true);
            isDying = false;
            SetRespawnFlag(false);
        }
    }

    public void DealDamageToPlayer(){
        playerHealthSystem.Damage(zombieDamage);
    }

    public void Damage(int amount){
        currentHealth -= amount;
        if(currentHealth <= 0){
            currentHealth = 0;
            isDying = true;
            EnableControllerOnDeath(false);
        }
    }

    public bool IsZombieDying(){
        return isDying;
    }

    public void EnableControllerOnDeath(bool enable){
        zombieController.enabled = enable;
    }

    public void SetRespawnFlag(bool enable){
        zombieJustRespawned = enable;
    }

    public bool ZombieHasRespawned(){
        return zombieJustRespawned;
    }
}
