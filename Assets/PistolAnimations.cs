using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolAnimations : MonoBehaviour {
    
    [SerializeField] private WeaponConfig pistolConfig;

    void Update(){
        
    }

    void TriggerRefreshCurrentWeaponAmmo(){
        pistolConfig.currentBulletCount = pistolConfig.maxBulletCount;
    }
}
