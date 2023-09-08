using UnityEngine;

public class WeaponConfig : MonoBehaviour {
    [HideInInspector] public int currentBulletCount;
    public int maxBulletCount;
    public int damage;
    public bool isAutomatic;
    public float fireRate;
    public float reloadSpeed;
    public float inaccuracy;
    public AudioSource[] audios;

    void Start(){
        currentBulletCount = maxBulletCount;
    }

    public void PlaySound(int i){
        if(i >= 0 || i < audios.Length){
            audios[i].Play();
        }
    }
}
