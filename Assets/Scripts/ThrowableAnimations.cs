using UnityEngine;

public class ThrowableAnimations : MonoBehaviour {
    private const string IS_THROWING = "IsThrowing";
    private Animator throwableAnimator;
    private ThrowableBehavior throwableBehavior;

    void Awake() {
        throwableAnimator = GetComponent<Animator>();
        throwableBehavior = GameObject.Find("Weapon Holder").GetComponent<ThrowableBehavior>();
    }

    void Update() {
        throwableAnimator.SetBool(IS_THROWING, throwableBehavior.throwableHasBeenThrown);
    }
}
