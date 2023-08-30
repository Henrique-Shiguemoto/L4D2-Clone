using UnityEngine;

public class RifleAnimations : MonoBehaviour {
    [SerializeField] private WeaponConfig rifleConfig;

    void Update(){
        
    }

    void TriggerRefreshCurrentWeaponAmmo(){
        rifleConfig.currentBulletCount = rifleConfig.maxBulletCount;
    }
}
