using UnityEngine;

public class PistolAnimations : MonoBehaviour {
    [SerializeField] private WeaponConfig pistolConfig;
    private WeaponBehavior pistolBehavior;
    private Animator pistolAnimator;

    private const string IS_FIRING = "IsFiring";
    private const string IS_RELOADING = "IsReloading";

    void Awake(){
        pistolAnimator = GetComponent<Animator>();
        pistolBehavior = GameObject.Find("Weapon Holder").GetComponent<WeaponBehavior>();
    }

    void Update(){
        pistolAnimator.SetBool(IS_RELOADING, pistolBehavior.isReloading);
        pistolAnimator.SetBool(IS_FIRING, pistolBehavior.isShooting);
    }

    void TriggerRefreshCurrentWeaponAmmo_Pistol(){
        pistolConfig.currentBulletCount = pistolConfig.maxBulletCount;
    }
}
