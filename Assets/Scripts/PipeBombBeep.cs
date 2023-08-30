using UnityEngine;

public class PipeBombBeep : MonoBehaviour {
    private AudioSource beep;
    private ThrowableBehavior throwableBehaviorScript;

    private float timeBetweenBeeps = 1.2f;
    private float timeLeftToBeep = 0f;

    void Awake() {
        beep = GetComponents<AudioSource>()[2];
        throwableBehaviorScript = GameObject.Find("Weapon Holder").GetComponent<ThrowableBehavior>();
    }

    //I want time between beeps to be fixed
    void FixedUpdate() {
        if(throwableBehaviorScript.throwableHasBeenThrown){
            timeLeftToBeep -= Time.fixedDeltaTime;
            if(timeLeftToBeep <= 0){
                beep.Play();
                timeLeftToBeep = timeBetweenBeeps;
            }
        }
    }
}
