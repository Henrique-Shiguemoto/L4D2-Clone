using UnityEngine;

public class SniperAnimations : MonoBehaviour {
    [SerializeField] private WeaponConfig sniperConfig;
    private Animator sniperAnimator;
    private WeaponBehavior sniperBehavior;

    private const string IS_FIRING = "IsFiring";
    private const string IS_RELOADING = "IsReloading";
    private const string IS_SCOPED = "IsScoped";

    void Awake(){
        sniperBehavior = GameObject.Find("Weapon Holder").GetComponent<WeaponBehavior>();
        sniperAnimator = GetComponent<Animator>();
    }

    void Update(){
        sniperAnimator.SetBool(IS_SCOPED, sniperBehavior.isScoped);
        sniperAnimator.SetBool(IS_RELOADING, sniperBehavior.isReloading);
        sniperAnimator.SetBool(IS_FIRING, sniperBehavior.isShooting);
    }

    void TriggerRefreshCurrentWeaponAmmo_Sniper(){
        sniperConfig.currentBulletCount = sniperConfig.maxBulletCount;
    }
}
