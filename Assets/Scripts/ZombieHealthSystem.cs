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
            EnableController(true);
            EnableHeadCollider(true);
            isDying = false;
            SetRespawnFlag(false);
            EnableHeadVisuals(true);
        }
    }

    public void DealDamageToPlayer(){
        playerHealthSystem.Damage(zombieDamage);
    }

    public void Damage(int amount, bool headShot){
        currentHealth -= amount;
        if(currentHealth <= 0){
            currentHealth = 0;
            isDying = true;
            EnableController(false);
            EnableHeadCollider(false);
        }
        if(headShot) {
            // deactivating head visual game object
            EnableHeadVisuals(false);
        }
    }

    public bool IsZombieDying(){
        return isDying;
    }

    public void EnableController(bool enable){
        zombieController.enabled = enable;
    }

    public void EnableHeadCollider(bool enable){
        transform.GetChild(1).gameObject.SetActive(enable);
    }

    public void EnableHeadVisuals(bool enable){
        transform.GetChild(0).GetChild(0).gameObject.SetActive(enable);
    }    

    public void SetRespawnFlag(bool enable){
        zombieJustRespawned = enable;
    }

    public bool ZombieHasRespawned(){
        return zombieJustRespawned;
    }
}
