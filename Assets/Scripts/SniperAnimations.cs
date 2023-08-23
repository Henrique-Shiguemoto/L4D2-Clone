using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperAnimations : MonoBehaviour {
    [SerializeField] private WeaponConfig sniperConfig;

    void Update(){
        
    }

    void TriggerRefreshCurrentWeaponAmmo(){
        sniperConfig.currentBulletCount = sniperConfig.maxBulletCount;
    }
}
