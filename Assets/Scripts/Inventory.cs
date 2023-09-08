using UnityEngine;

public class Inventory : MonoBehaviour {
    [HideInInspector] public GameObject primaryWeapon   = null;             // first spot
    [HideInInspector] public GameObject secondaryWeapon = null;             // second spot
    [HideInInspector] public GameObject throwable       = null;             // third spot
    [HideInInspector] public GameObject healthpack      = null;             // fourth spot
    [HideInInspector] public GameObject pills           = null;             // fifth spot
    [HideInInspector] public WeaponConfig primaryWeaponConfig   = null;
    [HideInInspector] public WeaponConfig secondaryWeaponConfig = null;
    [HideInInspector] public int currentHeldItemIndex   = 1;                // The player starts with a pistol, so the current held item is in the second spot of the inventory

    void Update(){
        HideItemsExceptForHeldItem();
        if(Input.GetKeyDown(KeyCode.Alpha1) && primaryWeapon)   currentHeldItemIndex = 0;
        if(Input.GetKeyDown(KeyCode.Alpha2) && secondaryWeapon) currentHeldItemIndex = 1;
        if(Input.GetKeyDown(KeyCode.Alpha3) && throwable)       currentHeldItemIndex = 2;
        if(Input.GetKeyDown(KeyCode.Alpha4) && healthpack)      currentHeldItemIndex = 3;
        if(Input.GetKeyDown(KeyCode.Alpha5) && pills)           currentHeldItemIndex = 4;
    }

    public bool HasPrimaryWeapon()          { return primaryWeapon != null; }
    public bool HasSecondaryWeapon()        { return secondaryWeapon != null; }
    public bool HasThrowable()              { return throwable != null; }
    public bool HasHealthpack()             { return healthpack != null; }
    public bool HasPills()                  { return pills != null; }
    public bool IsHoldingPrimaryWeapon()    { return currentHeldItemIndex == 0 && primaryWeapon; }
    public bool IsHoldingSecondaryWeapon()  { return currentHeldItemIndex == 1 && secondaryWeapon; }
    public bool IsHoldingThrowable()        { return currentHeldItemIndex == 2 && throwable; }
    public bool IsHoldingHealthpack()       { return currentHeldItemIndex == 3 && healthpack; }
    public bool IsHoldingPills()            { return currentHeldItemIndex == 4 && pills; }
    public bool IsHoldingWeapon()           { return IsHoldingPrimaryWeapon() || IsHoldingSecondaryWeapon(); }

    public GameObject GetCurrentHeldObject(){
        if(currentHeldItemIndex == 0) return primaryWeapon;
        if(currentHeldItemIndex == 1) return secondaryWeapon;
        if(currentHeldItemIndex == 2) return throwable;
        if(currentHeldItemIndex == 3) return healthpack;
        if(currentHeldItemIndex == 4) return pills;
        return null;
    }

    public WeaponConfig GetCurrentWeaponConfig(){
        if(currentHeldItemIndex == 0) return primaryWeaponConfig;
        if(currentHeldItemIndex == 1) return secondaryWeaponConfig;
        return null;
    }

    public void HideItemsExceptForHeldItem(){
        if(currentHeldItemIndex == 0){
            if(primaryWeapon)       primaryWeapon.SetActive(true);
            if(secondaryWeapon)     secondaryWeapon.SetActive(false);
            if(throwable)           throwable.SetActive(false);
            if(healthpack)          healthpack.SetActive(false);
            if(pills)               pills.SetActive(false);
        }else if(currentHeldItemIndex == 1) {
            if(primaryWeapon)       primaryWeapon.SetActive(false);
            if(secondaryWeapon)     secondaryWeapon.SetActive(true);
            if(throwable)           throwable.SetActive(false);
            if(healthpack)          healthpack.SetActive(false);
            if(pills)               pills.SetActive(false);
        }else if(currentHeldItemIndex == 2) {
            if(primaryWeapon)       primaryWeapon.SetActive(false);
            if(secondaryWeapon)     secondaryWeapon.SetActive(false);
            if(throwable)           throwable.SetActive(true);
            if(healthpack)          healthpack.SetActive(false);
            if(pills)               pills.SetActive(false);
        }else if(currentHeldItemIndex == 3) {
            if(primaryWeapon)       primaryWeapon.SetActive(false);
            if(secondaryWeapon)     secondaryWeapon.SetActive(false);
            if(throwable)           throwable.SetActive(false);
            if(healthpack)          healthpack.SetActive(true);
            if(pills)               pills.SetActive(false);
        }else if(currentHeldItemIndex == 4) {
            if(primaryWeapon)       primaryWeapon.SetActive(false);
            if(secondaryWeapon)     secondaryWeapon.SetActive(false);
            if(throwable)           throwable.SetActive(false);
            if(healthpack)          healthpack.SetActive(false);
            if(pills)               pills.SetActive(true);
        }
    }

    public void ChangeHeldObjectToDefault(){
        if(HasPrimaryWeapon()) {
            currentHeldItemIndex = 0;
        }else if(!HasPrimaryWeapon() && HasSecondaryWeapon()) {
            currentHeldItemIndex = 1;
        }else if(!HasPrimaryWeapon() && !HasSecondaryWeapon() && HasThrowable()){
            currentHeldItemIndex = 2;
        }else if(!HasPrimaryWeapon() && !HasSecondaryWeapon() && !HasThrowable() && HasHealthpack()){
            currentHeldItemIndex = 3;
        }else if(!HasPrimaryWeapon() && !HasSecondaryWeapon() && !HasThrowable() && !HasHealthpack() && HasPills()){
            currentHeldItemIndex = 4;
        }
    }
}
