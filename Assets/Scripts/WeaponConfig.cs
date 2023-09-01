using UnityEngine;

public class WeaponConfig : MonoBehaviour {
    [HideInInspector] public int currentBulletCount;
    public int maxBulletCount;
    public int damage;
    public bool isAutomatic;
    public float fireRate;
    public float reloadSpeed;
    public float inaccuracy;

    void Start(){
        currentBulletCount = maxBulletCount;
    }
}
