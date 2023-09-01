using System.Collections;
using UnityEngine;
using TMPro;

public class WeaponBehavior : MonoBehaviour{
    [SerializeField] private Camera cameraObject;
    [SerializeField] private GameObject weaponCameraObject;
    [SerializeField] private Transform weaponHolderTransform;
    [SerializeField] private float pickupRange;
    [SerializeField] private int headShotMultiplier = 2;
    [SerializeField] private float dropUpwardForce;
    [SerializeField] private float dropForwardForce;
    [SerializeField] private float bloodSplatterTime = 1.0f;
    [SerializeField] private float sniperScopeZoom = 15f;
    [SerializeField] private GameObject bloodSplatterParticleSystem;
    [SerializeField] private GameObject scopeOverlay;
    [SerializeField] private PlayerHealthSystem playerHealthSystem;

    [SerializeField] private TextMeshProUGUI ammoText;
    
    private AudioSource reloadAudio;
    private AudioSource fireAudio;

    private const string IS_FIRING = "IsFiring";
    private const string IS_RELOADING = "IsReloading";

    private bool playerIsHoldingWeapon;
    private bool isReloading;
    private bool isShooting;
    private bool muzzleflashIsLit;
    public bool isScoped;
    private GameObject currentWeapon;

    private float currentTimeLeftToShootAgain;
    private float timeToShootAgain;
    private float currentTimeLeftToReloadAgain;
    private float timeToReloadAgain;
    private float originalFOV;

    private void Start(){
        playerIsHoldingWeapon = false;
        isReloading = false;
        isShooting = false;
        currentWeapon = null;
        isScoped = false;
        muzzleflashIsLit = false;
    }

    void Update(){
        if(playerHealthSystem.IsPlayerDying()){
            if(playerIsHoldingWeapon) DropWeapon(currentWeapon);
        }

        //weapon pickup
        Ray ray = cameraObject.ScreenPointToRay(Input.mousePosition);
        bool cameraRaycastIsHittingSomething = Physics.Raycast(ray, out RaycastHit hit);

        if (cameraRaycastIsHittingSomething && hit.transform.gameObject.tag.Equals("Weapon") && Input.GetKeyDown(KeyCode.E) && hit.distance <= pickupRange){
            PickupWeapon(hit.transform.gameObject);
        }

        if (playerIsHoldingWeapon){
            WeaponConfig currentWeaponConfig = currentWeapon.GetComponent<WeaponConfig>();

            if ((currentWeaponConfig.isAutomatic ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1")) && 
                !isReloading && !isShooting && currentWeaponConfig.currentBulletCount > 0){
                isShooting = true;
                currentWeaponConfig.currentBulletCount--;
                if (currentWeaponConfig.currentBulletCount <= 0) currentWeaponConfig.currentBulletCount = 0;
                muzzleflashIsLit = true;
                GameObject weaponMuzzleFlash = currentWeapon.transform.GetChild(0).Find("MuzzleflashPlane").gameObject;
                weaponMuzzleFlash.SetActive(true);
                fireAudio.Play();

                //check zombie hits
                if(hit.transform != null){
                    string objectTag = hit.transform.gameObject.tag;
                    if(objectTag.Equals("Zombie") || objectTag.Equals("ZombieHead")){
                        ZombieHealthSystem zhs = null;
                        if(objectTag.Equals("ZombieHead")){
                            zhs = hit.transform.parent.gameObject.GetComponent<ZombieHealthSystem>();
                            zhs.Damage(currentWeaponConfig.damage * headShotMultiplier, true);
                        }else{
                            zhs = hit.transform.gameObject.GetComponent<ZombieHealthSystem>();
                            zhs.Damage(currentWeaponConfig.damage, false);
                        }

                        GameObject bloodSplatter = Instantiate(bloodSplatterParticleSystem, hit.point, Quaternion.LookRotation(hit.normal));
                        Destroy(bloodSplatter, bloodSplatterTime);
                    }
                }
            }

            if(Input.GetButtonDown("Fire2") && !isReloading && !isShooting && currentWeapon.name.Equals("Sniper")){
                isScoped = !isScoped;

                if(isScoped) StartCoroutine(OnScoped());
                else OnUnscoped();
            }

            //reload
            if ((Input.GetKeyDown(KeyCode.R) && currentWeaponConfig.currentBulletCount < currentWeaponConfig.maxBulletCount && !isShooting && !isReloading) || 
                (currentWeaponConfig.currentBulletCount == 0 && !isReloading)){
                if(isScoped){
                    isScoped = false;
                    OnUnscoped();
                }
                isReloading = true;
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
            weaponAnimator.SetBool(IS_RELOADING, isReloading);
            weaponAnimator.SetBool(IS_FIRING, isShooting);
        }
    }

    void PickupWeapon(GameObject newWeapon){
        // Debug.Log("Picked up " + newWeapon.name);
        if (playerIsHoldingWeapon) DropWeapon(currentWeapon); 

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

    void DropWeapon(GameObject weaponToDrop){
        // Debug.Log("Dropped " + weaponToDrop.name);
        if (weaponToDrop == null) return;
        if (playerIsHoldingWeapon){
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

    public bool IsSniperScoped(){
        return isScoped;
    }

    IEnumerator OnScoped(){
        yield return new WaitForSeconds(0.15f);
        scopeOverlay.SetActive(true);

        weaponCameraObject.SetActive(false);
        originalFOV = cameraObject.fieldOfView;
        cameraObject.fieldOfView = sniperScopeZoom;
    }

    void OnUnscoped(){
        scopeOverlay.SetActive(false);
        weaponCameraObject.SetActive(true);

        cameraObject.fieldOfView = originalFOV;
    }
}
