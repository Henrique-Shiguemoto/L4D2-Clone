using UnityEngine;

public class MuzzleFlash : MonoBehaviour {
    private WeaponBehavior weaponBehavior;
    private Inventory playerInventory;
    [SerializeField] private GameObject muzzleFlash;

    void Awake(){
        weaponBehavior = GameObject.Find("Weapon Holder").GetComponent<WeaponBehavior>();
        playerInventory = GameObject.Find("MainPlayer").GetComponent<Inventory>();
    }

    void Update(){
        if(transform.parent.gameObject == playerInventory.GetCurrentHeldObject()){
            muzzleFlash.SetActive(weaponBehavior.muzzleFlashShouldLit);
        }
    }
}
