using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunAnimations : MonoBehaviour {
    [SerializeField] private WeaponConfig shotgunConfig;

    void Update(){
        
    }

    void TriggerRefreshCurrentWeaponAmmo(){
        shotgunConfig.currentBulletCount = shotgunConfig.maxBulletCount;
    }
}
