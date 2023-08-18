using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponBehavior : MonoBehaviour
{
    [SerializeField] private Camera cameraObject;
    [SerializeField] private Transform weaponHolderTransform;
    [SerializeField] private float pickupRange;
    [SerializeField] private float dropUpwardForce;
    [SerializeField] private float dropForwardForce;

    [SerializeField] private TextMeshProUGUI ammoText;
    
    private AudioSource reloadAudio;
    private AudioSource fireAudio;

    private const string IS_FIRING = "IsFiring";
    private const string IS_RELOADING = "IsReloading";

    private bool playerIsHoldingWeapon;
    private bool isReloading;
    private bool isShooting;
    private bool muzzleflashIsLit;
    private GameObject currentWeapon;

    private float currentTimeLeftToShootAgain;
    private float timeToShootAgain;
    private float currentTimeLeftToReloadAgain;
    private float timeToReloadAgain;

    private void Start()
    {
        playerIsHoldingWeapon = false;
        isReloading = false;
        isShooting = false;
        currentWeapon = null;
        muzzleflashIsLit = false;
    }

    void Update()
    {
        //weapon pickup
        Ray ray = cameraObject.ScreenPointToRay(Input.mousePosition);
        bool cameraRaycastIsHittingSomething = Physics.Raycast(ray, out RaycastHit hit);

        if (cameraRaycastIsHittingSomething && 
            hit.transform.gameObject.tag.Equals("Weapon") &&
            Input.GetKeyDown(KeyCode.E) &&
            hit.distance <= pickupRange)
        {
            PickupWeapon(hit.transform.gameObject);
        }

        //shooting and reloading logic (no visuals)
        if (playerIsHoldingWeapon)
        {
            WeaponConfig currentWeaponConfig = currentWeapon.GetComponent<WeaponConfig>();

            //shoot (idk how to refactor this)
            if(currentWeaponConfig.isAutomatic){
                if (Input.GetButton("Fire1") && !isReloading && !isShooting && currentWeaponConfig.currentBulletCount > 0){
                    isShooting = true;
                    currentWeaponConfig.currentBulletCount--;
                    if (currentWeaponConfig.currentBulletCount <= 0){
                        currentWeaponConfig.currentBulletCount = 0;
                    }
                    muzzleflashIsLit = true;
                    GameObject weaponMuzzleFlash = currentWeapon.transform.GetChild(0).Find("MuzzleflashPlane").gameObject;
                    weaponMuzzleFlash.SetActive(true);
                    fireAudio.Play();
                }
            }else{
                if (Input.GetButtonDown("Fire1") && !isReloading && !isShooting && currentWeaponConfig.currentBulletCount > 0){
                    isShooting = true;
                    currentWeaponConfig.currentBulletCount--;
                    if (currentWeaponConfig.currentBulletCount <= 0){
                        currentWeaponConfig.currentBulletCount = 0;
                    }
                    muzzleflashIsLit = true;
                    GameObject weaponMuzzleFlash = currentWeapon.transform.GetChild(0).Find("MuzzleflashPlane").gameObject;
                    weaponMuzzleFlash.SetActive(true);
                    fireAudio.Play();
                }
            }

            //reload
            if (Input.GetKeyDown(KeyCode.R) && currentWeaponConfig.currentBulletCount < currentWeaponConfig.maxBulletCount && !isShooting && !isReloading)
            {
                isReloading = true;
                currentWeaponConfig.currentBulletCount = currentWeaponConfig.maxBulletCount;
                reloadAudio.Play();
            }

            //ammo text update
            ammoText.text = currentWeaponConfig.currentBulletCount + "/" + currentWeaponConfig.maxBulletCount;
        }

        //shooting cooldown
        if(isShooting && currentTimeLeftToShootAgain > 0){
            currentTimeLeftToShootAgain -= Time.deltaTime;
            if(currentTimeLeftToShootAgain <= 0){
                isShooting = false;
                currentTimeLeftToShootAgain = timeToShootAgain;
            }
            if(muzzleflashIsLit && currentTimeLeftToShootAgain <= timeToShootAgain * 0.75){
                muzzleflashIsLit = false;
                GameObject weaponMuzzleFlash = currentWeapon.transform.GetChild(0).Find("MuzzleflashPlane").gameObject;
                weaponMuzzleFlash.SetActive(false);
            }
        }

        //reloading cooldown
        if(isReloading && currentTimeLeftToReloadAgain > 0){
            currentTimeLeftToReloadAgain -= Time.deltaTime;
            if(currentTimeLeftToReloadAgain <= 0){
                isReloading = false;
                currentTimeLeftToReloadAgain = timeToReloadAgain;
            }
        }

        //animations
        if(playerIsHoldingWeapon){
            Animator weaponAnimator = currentWeapon.transform.GetChild(0).gameObject.GetComponent<Animator>();
            
            //isShooting and isReloading are exclusively true/false (they cannot be equal)
            // players cannot shoot while reloading and they cannot reload while shooting
            weaponAnimator.SetBool(IS_FIRING, isShooting);
            weaponAnimator.SetBool(IS_RELOADING, isReloading);
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
        
        AudioSource[] audios = currentWeapon.GetComponents<AudioSource>();
        fireAudio = audios[0];
        reloadAudio = audios[1];
        
        WeaponConfig currentWeaponConfig = currentWeapon.GetComponent<WeaponConfig>();
        currentTimeLeftToShootAgain = currentWeaponConfig.fireRate;
        timeToShootAgain = currentWeaponConfig.fireRate;
        currentTimeLeftToReloadAgain = currentWeaponConfig.reloadSpeed;
        timeToReloadAgain = currentWeaponConfig.reloadSpeed;
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
