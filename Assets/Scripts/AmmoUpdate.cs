using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoUpdate : MonoBehaviour {
    private TextMeshProUGUI primaryAmmoText;
    private TextMeshProUGUI secondaryAmmoText;
    private RawImage throwableIcon;
    private RawImage healthpackIcon;
    private RawImage pillsIcon;

    private Inventory playerInventory;

    // private Color activatedIconColor = new Color(1.0f, 1.0f, 1.0f, 1.0f); // 255 alpha default
    // private Color deactivatedIconColor  = new Color(1.0f, 1.0f, 1.0f, 0.24f); // 60ish alpha default

    void Awake(){
        playerInventory = GameObject.Find("MainPlayer").GetComponent<Inventory>();
    }

    void Update(){
        if(playerInventory){
            // primaryAmmoText.text = playerInventory.primaryWeaponConfig.currentBulletCount + " / " + playerInventory.primaryWeaponConfig.maxBulletCount;
            // secondaryAmmoText.text = playerInventory.secondaryWeaponConfig.currentBulletCount + " / " + playerInventory.secondaryWeaponConfig.maxBulletCount;

        }
    }
}
