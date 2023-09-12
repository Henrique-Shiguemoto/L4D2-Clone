using UnityEngine;

public class HealthpackBehavior : MonoBehaviour {
    [SerializeField] private Camera cameraObject;
    [SerializeField] private float pickupRange;
    [SerializeField] private float dropUpwardForce;
    [SerializeField] private float dropForwardForce;
    [SerializeField] private float timeForHeal;
    [SerializeField, Range(0, 1)] private float healPercentage = 0.8f;
    [SerializeField] private GameObject healingUI;

    private Inventory playerInventory;
    private PlayerHealthSystem playerHealthSystem;

    private float timeLeftForHeal;
    public bool isHealing = false;

    void Awake(){
        playerInventory = GameObject.Find("MainPlayer").GetComponent<Inventory>();
        playerHealthSystem = GameObject.Find("MainPlayer").GetComponent<PlayerHealthSystem>();
    }

    void Start(){
        timeLeftForHeal = timeForHeal;
    }

    void Update(){
        HandlePickup();
        if(playerInventory.IsHoldingHealthpack()) HandleHeal();
    }

    void HandlePickup(){
        Ray ray = cameraObject.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform.gameObject.tag.Equals("Healthpack") && Input.GetKeyDown(KeyCode.E) && hit.distance <= pickupRange){
            PickupHealthpack(hit.transform.gameObject);
        }
    }

    void PickupHealthpack(GameObject newHealthpack){
        if(playerInventory.HasHealthpack()) DropHealthpack(playerInventory.healthpack);

        // Physics stuff
        {
            newHealthpack.GetComponent<Rigidbody>().isKinematic = true;
            newHealthpack.GetComponent<BoxCollider>().isTrigger = true;
            newHealthpack.transform.SetParent(transform);
            newHealthpack.transform.localPosition = new Vector3(-0.1f, -0.3f, -0.57f);
            newHealthpack.transform.localRotation = Quaternion.Euler(new Vector3(44, 15, 1));
            newHealthpack.transform.localScale = Vector3.one;
        }

        playerInventory.healthpack = newHealthpack;
        playerInventory.ChangeHeldObjectToDefault();
    }

    void DropHealthpack(GameObject healthpackToBeDropped){
        if (healthpackToBeDropped == null) return;
        if (playerInventory.HasHealthpack()){
            healthpackToBeDropped.transform.SetParent(null);
            healthpackToBeDropped.GetComponent<Rigidbody>().isKinematic = false;
            healthpackToBeDropped.GetComponent<BoxCollider>().isTrigger = false;

            //Add forces to weapon when dropped (up and forward)
            healthpackToBeDropped.GetComponent<Rigidbody>().AddForce(dropUpwardForce * cameraObject.transform.up, ForceMode.Impulse);
            healthpackToBeDropped.GetComponent<Rigidbody>().AddForce(dropForwardForce * cameraObject.transform.forward, ForceMode.Impulse);
        }
    }

    void HandleHeal(){
        if(playerHealthSystem.currentHealth < playerHealthSystem.maxHealth - 1 && Input.GetButton("Fire1")){
            // player has to hold button for a couple of seconds to actually heal
            WorkTowardsHealing();
        }else{
            timeLeftForHeal = timeForHeal;
            isHealing = false;
            healingUI.SetActive(false);
        }
    }

    void WorkTowardsHealing(){
        healingUI.SetActive(true);
        isHealing = true;
        timeLeftForHeal -= Time.deltaTime;
        if(timeLeftForHeal <= 0){
            timeLeftForHeal = timeForHeal;
            Heal();
        }
    }

    void Heal(){
        playerHealthSystem.Heal((int)((playerHealthSystem.maxHealth - playerHealthSystem.currentHealth) * healPercentage));
        Destroy(playerInventory.healthpack);
        playerInventory.healthpack = null;
        playerInventory.ChangeHeldObjectToDefault();
        healingUI.SetActive(false);
    }

    public float HealingProgress(){
        return (1.0f - (timeLeftForHeal / timeForHeal));
    }
}