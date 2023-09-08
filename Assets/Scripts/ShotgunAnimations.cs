using UnityEngine;

public class ShotgunAnimations : MonoBehaviour {
    [SerializeField] private WeaponConfig shotgunConfig;
    private WeaponBehavior shotgunBehavior;
    private Animator shotgunAnimator;

    private const string IS_FIRING = "IsFiring";
    private const string IS_RELOADING = "IsReloading";

    void Start(){
        shotgunAnimator = GetComponent<Animator>();
        shotgunBehavior = GameObject.Find("Weapon Holder").GetComponent<WeaponBehavior>();
    }

    void Update(){
        shotgunAnimator.SetBool(IS_RELOADING, shotgunBehavior.isReloading);
        shotgunAnimator.SetBool(IS_FIRING, shotgunBehavior.isShooting);
    }

    void TriggerRefreshCurrentWeaponAmmo_Shotgun(){
        shotgunConfig.currentBulletCount = shotgunConfig.maxBulletCount;
    }
}
