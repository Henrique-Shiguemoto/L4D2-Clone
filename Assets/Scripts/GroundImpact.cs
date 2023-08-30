using UnityEngine;

public class GroundImpact : MonoBehaviour {
    private AudioSource groundImpact;
    private BoxCollider grenadeBoxCollider;
    private ThrowableBehavior throwableBehaviorScript;

    [SerializeField] private LayerMask groundMask;

    private bool isGrounded = false;
    private bool wasGroundedLastCoupleOfFrames = false;

    void Awake() {
        groundImpact = GetComponents<AudioSource>()[1];
        grenadeBoxCollider = GetComponent<BoxCollider>();
        throwableBehaviorScript = GameObject.Find("Weapon Holder").GetComponent<ThrowableBehavior>();
    }

    void Update() {
        if(throwableBehaviorScript.throwableHasBeenThrown){
            isGrounded = Physics.CheckBox(grenadeBoxCollider.bounds.center, 
                                            0.5f * grenadeBoxCollider.bounds.size, 
                                            Quaternion.identity, 
                                            groundMask);

            if(isGrounded && !wasGroundedLastCoupleOfFrames){
                wasGroundedLastCoupleOfFrames = true;
                isGrounded = false;
                groundImpact.Play();
            }
        }
    }
}
