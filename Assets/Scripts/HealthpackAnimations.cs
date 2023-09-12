using UnityEngine;

public class HealthpackAnimations : MonoBehaviour {
    private const string IS_HEALING = "IsHealing";
    private Animator healthpackAnimator;
    private HealthpackBehavior healthpackBehavior;

    void Awake() {
        healthpackAnimator = GetComponent<Animator>();
        healthpackBehavior = GameObject.Find("Weapon Holder").GetComponent<HealthpackBehavior>();
    }

    void Update() {
        healthpackAnimator.SetBool(IS_HEALING, healthpackBehavior.isHealing);
    }
}
