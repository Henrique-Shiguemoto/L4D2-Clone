using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehavior : MonoBehaviour
{
    [SerializeField] private Camera cameraObject;
    [SerializeField] private Transform weaponHolderTransform;
    [SerializeField] private float pickupRange;
    [SerializeField] private float dropUpwardForce;
    [SerializeField] private float dropForwardForce;

    [SerializeField] private float fireRate;
    [SerializeField] private float reloadSpeed;
    [SerializeField] private bool automatic;
    [SerializeField] private int maxBulletCount;

    private const string IS_FIRING = "IsFiring"; // just to make sure I don't type this incorrectly

    private int currentBulletAmount;

    private bool playerIsHoldingWeapon;
    private GameObject currentWeapon;

    private void Start()
    {
        currentBulletAmount = maxBulletCount;

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

        if (playerIsHoldingWeapon)
        {
            GameObject currentWeaponVisual = currentWeapon.transform.GetChild(0).gameObject;
            currentWeaponVisual.GetComponent<Animator>().SetBool(IS_FIRING, playerIsHoldingWeapon && Input.GetButtonDown("Fire1"));
        }
    }

    void PickupWeapon(GameObject newWeapon)
    {
        Debug.Log("Picked up " + newWeapon.name);
        if (playerIsHoldingWeapon)
        {
            DropWeapon(currentWeapon);
        }

        //GetChild(0) returns the weapon child with the visuals
        GameObject newWeaponVisual = newWeapon.transform.GetChild(0).gameObject;
        newWeaponVisual.GetComponent<Animator>().enabled = true;

        newWeapon.GetComponent<Rigidbody>().isKinematic = true;
        newWeapon.GetComponent<BoxCollider>().isTrigger = true;
        playerIsHoldingWeapon = true;

        newWeapon.transform.SetParent(weaponHolderTransform);
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.Euler(Vector3.zero);
        newWeapon.transform.localScale = Vector3.one;

        currentWeapon = newWeapon;
    }

    void DropWeapon(GameObject weaponToDrop)
    {
        Debug.Log("Dropped " + weaponToDrop.name);
        if (weaponToDrop == null)
        {
            return;
        }
        if (playerIsHoldingWeapon)
        {
            weaponToDrop.transform.SetParent(null);
            GameObject newWeaponVisual = weaponToDrop.transform.GetChild(0).gameObject;
            newWeaponVisual.GetComponent<Animator>().enabled = false;

            weaponToDrop.GetComponent<Rigidbody>().isKinematic = false;
            weaponToDrop.GetComponent<BoxCollider>().isTrigger = false;

            //Add forces to weapon when dropped (up and forward)
            weaponToDrop.GetComponent<Rigidbody>().AddForce(dropUpwardForce * cameraObject.transform.up, ForceMode.Impulse);
            weaponToDrop.GetComponent<Rigidbody>().AddForce(dropForwardForce * cameraObject.transform.forward, ForceMode.Impulse);

            playerIsHoldingWeapon = false;
        }
    }
}
