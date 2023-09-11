using System.Collections;
using UnityEngine;
using TMPro;

public class WeaponBehavior : MonoBehaviour{
    [SerializeField] private Camera cameraObject;
    [SerializeField] private GameObject weaponCameraObject; // refactor sniper scope
    [SerializeField] private GameObject initialWeapon;
    [SerializeField] private float pickupRange;
    [SerializeField] private int headShotMultiplier = 2;
    [SerializeField] private float dropUpwardForce;
    [SerializeField] private float dropForwardForce;
    [SerializeField] private float bloodSplatterTime = 1.0f;
    [SerializeField] private float sniperScopeZoom = 15f;
    [SerializeField] private GameObject bloodSplatterParticleSystem;
    [SerializeField] private GameObject scopeOverlay;
    [SerializeField] private PlayerHealthSystem playerHealthSystem;

    [HideInInspector] public bool isReloading = false;
    [HideInInspector] public bool isShooting = false;
    [HideInInspector] public bool isScoped = false;
    [HideInInspector] public bool muzzleFlashShouldLit = false;

    private Inventory playerInventory = null;

    private float timeToShoot;
    private float timeToShootAgain;
    private float timeToReload;
    private float timeToReloadAgain;
    private float timeToShoot_sec;
    private float timeToShootAgain_sec;
    private float timeToReload_sec;
    private float timeToReloadAgain_sec;
    private float shotTimer;
    private float shotMaxTimer;
    private float reloadTimer;
    private float reloadMaxTimer;
    
    private float originalFOV;

    void Awake(){
        playerInventory = GameObject.Find("MainPlayer").GetComponent<Inventory>();
    }

    void Start(){
        PickupWeapon(Instantiate(initialWeapon));
    }

    void Update(){
        if(playerHealthSystem.IsPlayerDying()){
            if(playerInventory.IsHoldingWeapon()) DropWeapon(playerInventory.GetCurrentHeldObject());
            return;
        }

        HandlePickup();
        if (playerInventory.IsHoldingWeapon()){
            HandleShoot();
            HandleSniperScope();
            HandleReload();
        }
    }

    void PickupWeapon(GameObject newWeapon){
        if (IsWeaponPrimary(newWeapon) && playerInventory.HasPrimaryWeapon()) {
            // it could be that they're inactive, so when we drop it, it could just be invisible, so I'm just making sure it is visible when we drop it.
            playerInventory.primaryWeapon.SetActive(true);
            DropWeapon(playerInventory.primaryWeapon);
        }else if(!IsWeaponPrimary(newWeapon) && playerInventory.HasSecondaryWeapon()){
            playerInventory.primaryWeapon.SetActive(true);
            DropWeapon(playerInventory.secondaryWeapon);
        }

        // Enabling animator component on the visuals
        newWeapon.transform.GetChild(0).gameObject.GetComponent<Animator>().enabled = true;

        // Physics stuff
        {
            newWeapon.GetComponent<Rigidbody>().isKinematic = true;
            newWeapon.GetComponent<BoxCollider>().isTrigger = true;
            newWeapon.transform.SetParent(transform);
            newWeapon.transform.localPosition = Vector3.zero;
            newWeapon.transform.localRotation = Quaternion.Euler(Vector3.zero);
            newWeapon.transform.localScale = Vector3.one;
        }

        if(IsWeaponPrimary(newWeapon)){
            playerInventory.primaryWeapon = newWeapon;
            playerInventory.primaryWeaponConfig = playerInventory.primaryWeapon.GetComponent<WeaponConfig>();
            timeToShoot = playerInventory.primaryWeaponConfig.fireRate;
            timeToShootAgain = playerInventory.primaryWeaponConfig.fireRate;
            timeToReload = playerInventory.primaryWeaponConfig.reloadSpeed;
            timeToReloadAgain = playerInventory.primaryWeaponConfig.reloadSpeed;
        }else{
            playerInventory.secondaryWeapon = newWeapon;
            playerInventory.secondaryWeaponConfig = playerInventory.secondaryWeapon.GetComponent<WeaponConfig>();
            timeToShoot_sec = playerInventory.secondaryWeaponConfig.fireRate;
            timeToShootAgain_sec = playerInventory.secondaryWeaponConfig.fireRate;
            timeToReload_sec = playerInventory.secondaryWeaponConfig.reloadSpeed;
            timeToReloadAgain_sec = playerInventory.secondaryWeaponConfig.reloadSpeed;
        }
        playerInventory.ChangeHeldObjectToDefault();
    }

    void DropWeapon(GameObject weaponToDrop){
        if (weaponToDrop == null) return;
        if (playerInventory.IsHoldingWeapon()){
            weaponToDrop.transform.SetParent(null);
            GameObject newWeaponVisual = weaponToDrop.transform.GetChild(0).gameObject;
            newWeaponVisual.GetComponent<Animator>().enabled = false;

            weaponToDrop.GetComponent<Rigidbody>().isKinematic = false;
            weaponToDrop.GetComponent<BoxCollider>().isTrigger = false;

            //Add forces to weapon when dropped (up and forward)
            weaponToDrop.GetComponent<Rigidbody>().AddForce(dropUpwardForce * cameraObject.transform.up, ForceMode.Impulse);
            weaponToDrop.GetComponent<Rigidbody>().AddForce(dropForwardForce * cameraObject.transform.forward, ForceMode.Impulse);

            if(playerInventory.IsHoldingPrimaryWeapon()) playerInventory.primaryWeapon = null;
            else playerInventory.secondaryWeapon = null;
        }
    }

    IEnumerator OnScoped(){
        yield return new WaitForSeconds(0.15f);
        scopeOverlay.SetActive(true);

        weaponCameraObject.SetActive(false);
        originalFOV = cameraObject.fieldOfView;
        cameraObject.fieldOfView = sniperScopeZoom;
    }

    void OnUnscoped(){
        isScoped = false;

        scopeOverlay.SetActive(false);
        weaponCameraObject.SetActive(true);

        cameraObject.fieldOfView = originalFOV;
    }

    void HandlePickup(){
        Ray ray = cameraObject.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform.gameObject.tag.Equals("Weapon") && Input.GetKeyDown(KeyCode.E) && hit.distance <= pickupRange){
            PickupWeapon(hit.transform.gameObject);
        }
    }

    void HandleShoot(){
        WeaponConfig wc = playerInventory.GetCurrentWeaponConfig();
        if ((wc.isAutomatic ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1")) && 
            !isReloading && !isShooting && wc.currentBulletCount > 0){
            Shoot();
            CheckForZombieHits();
        }

        if(isShooting && shotTimer > 0){
            shotTimer -= Time.deltaTime;
            if(shotTimer <= 0){
                isShooting = false;
                shotTimer = shotMaxTimer;
            }
            if(muzzleFlashShouldLit && shotTimer <= shotMaxTimer * 0.75) muzzleFlashShouldLit = false;
        }
    }

    void Shoot(){
        isShooting = true;
        playerInventory.GetCurrentWeaponConfig().currentBulletCount--;
        if (playerInventory.GetCurrentWeaponConfig().currentBulletCount <= 0) playerInventory.GetCurrentWeaponConfig().currentBulletCount = 0;
        muzzleFlashShouldLit = true;
        
        shotTimer    = (playerInventory.IsHoldingPrimaryWeapon() ? timeToShoot : timeToShoot_sec);
        shotMaxTimer = (playerInventory.IsHoldingPrimaryWeapon() ? timeToShootAgain : timeToShootAgain_sec);

        playerInventory.GetCurrentWeaponConfig().PlaySound(0);
    }

    void CheckForZombieHits(){
        //check zombie hits
        Ray bulletRay = cameraObject.ScreenPointToRay(Input.mousePosition);
        bulletRay.direction += new Vector3(Random.Range(-playerInventory.GetCurrentWeaponConfig().inaccuracy / 2, playerInventory.GetCurrentWeaponConfig().inaccuracy / 2),
                                           Random.Range(-playerInventory.GetCurrentWeaponConfig().inaccuracy / 2, playerInventory.GetCurrentWeaponConfig().inaccuracy / 2),
                                           Random.Range(-playerInventory.GetCurrentWeaponConfig().inaccuracy / 2, playerInventory.GetCurrentWeaponConfig().inaccuracy / 2));

        if(Physics.Raycast(bulletRay, out RaycastHit bulletHit)){
            GameObject objectHit = bulletHit.transform.gameObject;
            if(objectHit.tag.Equals("Zombie") || objectHit.tag.Equals("ZombieHead")){
                ZombieHealthSystem zhs = objectHit.tag.Equals("Zombie") ? objectHit.GetComponent<ZombieHealthSystem>() : bulletHit.transform.parent.gameObject.GetComponent<ZombieHealthSystem>();
                
                if(objectHit.tag.Equals("ZombieHead")) zhs.Damage(playerInventory.GetCurrentWeaponConfig().damage * headShotMultiplier, true);
                else zhs.Damage(playerInventory.GetCurrentWeaponConfig().damage, false);

                GameObject bloodSplatter = Instantiate(bloodSplatterParticleSystem, bulletHit.point, Quaternion.LookRotation(bulletHit.normal));
                Destroy(bloodSplatter, bloodSplatterTime);
            }
        }
    }

    void HandleSniperScope(){
        if(Input.GetButtonDown("Fire2") && !isReloading && !isShooting && playerInventory.GetCurrentHeldObject().name.Equals("Sniper")){
            isScoped = !isScoped;

            if(isScoped) StartCoroutine(OnScoped());
            else OnUnscoped();
        }
    }

    void HandleReload(){
        if ((Input.GetKeyDown(KeyCode.R) && playerInventory.GetCurrentWeaponConfig().currentBulletCount < playerInventory.GetCurrentWeaponConfig().maxBulletCount 
            && !isShooting && !isReloading) || 
            (playerInventory.GetCurrentWeaponConfig().currentBulletCount == 0 && !isReloading)){
            Reload();
        }

        //reloading cooldown
        if(isReloading && reloadTimer > 0){
            reloadTimer -= Time.deltaTime;
            if(reloadTimer <= 0){
                isReloading = false;
                reloadTimer = reloadMaxTimer;
            }
        }
    }

    void Reload(){
        if(isScoped){
            isScoped = false;
            OnUnscoped();
        }
        isReloading = true;
        playerInventory.GetCurrentWeaponConfig().PlaySound(1);

        reloadTimer    = (playerInventory.IsHoldingPrimaryWeapon() ? timeToReload : timeToReload_sec);
        reloadMaxTimer = (playerInventory.IsHoldingPrimaryWeapon() ? timeToReloadAgain : timeToReloadAgain_sec);
    }

    bool IsWeaponPrimary(GameObject weapon){
        return weapon.name.Equals("Sniper") || weapon.name.Equals("Shotgun") || weapon.name.Equals("Rifle");
    }
}
