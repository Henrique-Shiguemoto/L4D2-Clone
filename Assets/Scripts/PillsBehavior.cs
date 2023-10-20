using UnityEngine;

public class PillsBehavior : MonoBehaviour {
    [SerializeField] private Camera cameraObject;
    [SerializeField] private float pickupRange;
    [SerializeField] private float dropUpwardForce;
    [SerializeField] private float dropForwardForce;
    public int healAmount = 50;
    
    private Inventory playerInventory;
    private PlayerHealthSystem playerHealthSystem;

    [HideInInspector] public bool isTakingPills = false;

    void Awake(){
        playerInventory = GameObject.Find("MainPlayer").GetComponent<Inventory>();
        playerHealthSystem = GameObject.Find("MainPlayer").GetComponent<PlayerHealthSystem>();
    }

    void Update(){
        HandlePickup();
        if(playerInventory.IsHoldingPills()) HandleHeal();
    }

    void HandlePickup(){
        Ray ray = cameraObject.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform.gameObject.tag.Equals("Pills") && Input.GetKeyDown(KeyCode.E) && hit.distance <= pickupRange){
            PickupPills(hit.transform.gameObject);
        }
    }

    void PickupPills(GameObject newPills){
        if(playerInventory.HasPills()) {
            playerInventory.pills.SetActive(true);
            DropPills(playerInventory.pills);
        }

        newPills.transform.GetChild(0).gameObject.GetComponent<Animator>().enabled = true;

        // Physics stuff
        {
            newPills.GetComponent<Rigidbody>().isKinematic = true;
            newPills.GetComponent<BoxCollider>().isTrigger = true;
            newPills.transform.SetParent(transform);
            newPills.transform.localPosition = new Vector3(0.0f, -0.05f, -0.1f);
            newPills.transform.localRotation = Quaternion.Euler(new Vector3(-10.0f, 0.0f, 12.5f));
            newPills.transform.localScale = Vector3.one;
        }

        playerInventory.pills = newPills;
        playerInventory.ChangeHeldObjectToDefault();
    }

    void DropPills(GameObject pillsToBeDropped){
        if (pillsToBeDropped == null) return;
        if (playerInventory.HasPills()){
            pillsToBeDropped.transform.SetParent(null);
            pillsToBeDropped.GetComponent<Rigidbody>().isKinematic = false;
            pillsToBeDropped.GetComponent<BoxCollider>().isTrigger = false;

            //Add forces to weapon when dropped (up and forward)
            pillsToBeDropped.GetComponent<Rigidbody>().AddForce(dropUpwardForce * cameraObject.transform.up, ForceMode.Impulse);
            pillsToBeDropped.GetComponent<Rigidbody>().AddForce(dropForwardForce * cameraObject.transform.forward, ForceMode.Impulse);
        }
    }

    void HandleHeal(){
        if(playerHealthSystem.currentHealth < playerHealthSystem.maxHealth - 1 && Input.GetButtonDown("Fire1")){
            isTakingPills = true;
        }
    }
}
