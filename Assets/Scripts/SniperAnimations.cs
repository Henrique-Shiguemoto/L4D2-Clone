using UnityEngine;

public class SniperAnimations : MonoBehaviour {
    [SerializeField] private WeaponConfig sniperConfig;
    [SerializeField] private Animator sniperAnimator;
    [SerializeField] private WeaponBehavior sniperBehavior;

    private const string IS_SCOPED = "IsScoped";

    void Awake(){
        sniperBehavior = GameObject.Find("Weapon Holder").GetComponent<WeaponBehavior>();
    }

    void Update(){
        sniperAnimator.SetBool(IS_SCOPED, sniperBehavior.IsSniperScoped());
    }

    void TriggerRefreshCurrentWeaponAmmo(){
        sniperConfig.currentBulletCount = sniperConfig.maxBulletCount;
    }
}
