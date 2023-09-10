using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoUpdate : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI primaryAmmoText;
    [SerializeField] private TextMeshProUGUI secondaryAmmoText;
    [SerializeField] private RawImage throwableIcon;
    [SerializeField] private RawImage healthpackIcon;
    [SerializeField] private RawImage pillsIcon;

    [SerializeField] private Image primaryAmmoTextBackground;
    [SerializeField] private Image secondaryAmmoTextBackground;
    [SerializeField] private Image throwableIconBackground;
    [SerializeField] private Image healthpackIconBackground;
    [SerializeField] private Image pillsIconBackground;

    private Inventory playerInventory;

    private Color activatedIconColor = new Color(1.0f, 1.0f, 1.0f, 1.0f); // 255 alpha default
    private Color deactivatedIconColor  = new Color(1.0f, 1.0f, 1.0f, 0.24f); // 60ish alpha default

    void Awake(){
        playerInventory = GameObject.Find("MainPlayer").GetComponent<Inventory>();
    }

    void Update(){
        if(playerInventory){
            primaryAmmoText.text   = (playerInventory.HasPrimaryWeapon())   ? playerInventory.primaryWeaponConfig.currentBulletCount + "/" + playerInventory.primaryWeaponConfig.maxBulletCount : "0/0";
            secondaryAmmoText.text = (playerInventory.HasSecondaryWeapon()) ? playerInventory.secondaryWeaponConfig.currentBulletCount + "/" + playerInventory.secondaryWeaponConfig.maxBulletCount : "0/0";
            throwableIcon.color    = (playerInventory.HasThrowable()) ?       activatedIconColor : deactivatedIconColor;
            healthpackIcon.color   = (playerInventory.HasHealthpack()) ?      activatedIconColor : deactivatedIconColor;
            pillsIcon.color        = (playerInventory.HasPills()) ?           activatedIconColor : deactivatedIconColor;

            if(playerInventory.IsHoldingPrimaryWeapon()){
                primaryAmmoTextBackground.enabled = true;
                secondaryAmmoTextBackground.enabled = false;
                throwableIconBackground.enabled = false;
                healthpackIconBackground.enabled = false;
                pillsIconBackground.enabled = false;
            }else if(playerInventory.IsHoldingSecondaryWeapon()){
                primaryAmmoTextBackground.enabled = false;
                secondaryAmmoTextBackground.enabled = true;
                throwableIconBackground.enabled = false;
                healthpackIconBackground.enabled = false;
                pillsIconBackground.enabled = false;
            }else if(playerInventory.IsHoldingThrowable()){
                primaryAmmoTextBackground.enabled = false;
                secondaryAmmoTextBackground.enabled = false;
                throwableIconBackground.enabled = true;
                healthpackIconBackground.enabled = false;
                pillsIconBackground.enabled = false;
            }else if(playerInventory.IsHoldingHealthpack()){
                primaryAmmoTextBackground.enabled = false;
                secondaryAmmoTextBackground.enabled = false;
                throwableIconBackground.enabled = false;
                healthpackIconBackground.enabled = true;
                pillsIconBackground.enabled = false;
            }else if(playerInventory.IsHoldingPills()){
                primaryAmmoTextBackground.enabled = false;
                secondaryAmmoTextBackground.enabled = false;
                throwableIconBackground.enabled = false;
                healthpackIconBackground.enabled = false;
                pillsIconBackground.enabled = true;
            }
        }
    }
}
