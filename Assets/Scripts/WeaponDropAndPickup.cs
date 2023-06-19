using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDropAndPickup : MonoBehaviour
{
    [SerializeField] private Camera cameraObject;
    [SerializeField] private Transform weaponHolderTransform;
    [SerializeField] private float pickupRange;
    [SerializeField] private float dropUpwardForce;
    [SerializeField] private float dropForwardForce;

    private bool playerIsHoldingWeapon;
    private GameObject currentWeapon;

    private void Start()
    {
        playerIsHoldingWeapon = false;
        currentWeapon = null;
    }

    void Update()
    {
        Ray ray = cameraObject.ScreenPointToRay(Input.mousePosition);
        bool cameraRaycastIsHittingSomething = Physics.Raycast(ray, out RaycastHit hit);
        
        if (cameraRaycastIsHittingSomething && 
            hit.transform.gameObject.tag.Equals("Weapon") &&
            Input.GetKeyDown(KeyCode.E) &&
            hit.distance <= pickupRange)
        {
            PickupWeapon(hit.transform.gameObject);
        }
    }

    void PickupWeapon(GameObject newWeapon)
    {
        if (playerIsHoldingWeapon)
        {
            DropWeapon(currentWeapon);
        }
        newWeapon.transform.SetParent(weaponHolderTransform);
        newWeapon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
        newWeapon.transform.localScale = Vector3.one;

        newWeapon.GetComponent<Rigidbody>().isKinematic = true;
        newWeapon.GetComponent<BoxCollider>().isTrigger = true;
        playerIsHoldingWeapon = true;

        currentWeapon = newWeapon;

        Debug.Log("Picked up " + newWeapon.name);
    }

    void DropWeapon(GameObject weaponToDrop)
    {
        if (weaponToDrop == null)
        {
            return;
        }
        if (playerIsHoldingWeapon)
        {
            //Rigidbody weaponRigidbody = weapon.GetComponent<Rigidbody>();
            BoxCollider weaponBoxCollider = weaponToDrop.GetComponent<BoxCollider>();

            weaponToDrop.transform.SetParent(null);
            weaponToDrop.GetComponent<Rigidbody>().isKinematic = false;
            weaponBoxCollider.isTrigger = false;

            //Add forces to weapon when dropped (up and forward)
            weaponToDrop.GetComponent<Rigidbody>().AddForce(dropUpwardForce * cameraObject.transform.up, ForceMode.Impulse);
            weaponToDrop.GetComponent<Rigidbody>().AddForce(dropForwardForce * cameraObject.transform.forward, ForceMode.Impulse);

            playerIsHoldingWeapon = false;
            Debug.Log("Dropped " + weaponToDrop.name);
        }
    }
}
