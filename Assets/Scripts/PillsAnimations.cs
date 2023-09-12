using UnityEngine;

public class PillsAnimations : MonoBehaviour {
    private const string IS_TAKING_PILLS = "IsTakingPills";
    private Animator pillsAnimator;
    private PillsBehavior pillsBehavior;
    private PlayerHealthSystem playerHealthSystem;
    private Inventory playerInventory;

    void Awake() {
        pillsAnimator = GetComponent<Animator>();
        pillsBehavior = GameObject.Find("Weapon Holder").GetComponent<PillsBehavior>();
        playerInventory = GameObject.Find("MainPlayer").GetComponent<Inventory>();
        playerHealthSystem = GameObject.Find("MainPlayer").GetComponent<PlayerHealthSystem>();
    }

    void Update() {
        pillsAnimator.SetBool(IS_TAKING_PILLS, pillsBehavior.isTakingPills);
    }

    void TriggerHealEvent(){
        playerHealthSystem.Heal(pillsBehavior.healAmount);
        Destroy(playerInventory.pills);
        playerInventory.pills = null;
        playerInventory.ChangeHeldObjectToDefault();
        pillsBehavior.isTakingPills = false;
    }
}