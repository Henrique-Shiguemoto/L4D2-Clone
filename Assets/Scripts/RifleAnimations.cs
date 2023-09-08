using UnityEngine;

public class RifleAnimations : MonoBehaviour {
    [SerializeField] private WeaponConfig rifleConfig;
    private WeaponBehavior rifleBehavior;
    private Animator rifleAnimator;

    private const string IS_FIRING = "IsFiring";
    private const string IS_RELOADING = "IsReloading";

    void Start(){
        rifleAnimator = GetComponent<Animator>();
        rifleBehavior = GameObject.Find("Weapon Holder").GetComponent<WeaponBehavior>();
    }

    void Update(){
        rifleAnimator.SetBool(IS_RELOADING, rifleBehavior.isReloading);
        rifleAnimator.SetBool(IS_FIRING, rifleBehavior.isShooting);
    }

    void TriggerRefreshCurrentWeaponAmmo_Rifle(){
        rifleConfig.currentBulletCount = rifleConfig.maxBulletCount;
    }
}
