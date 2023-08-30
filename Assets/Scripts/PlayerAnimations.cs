using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] Animator playerAnimator;
    [SerializeField] PlayerHealthSystem playerHealthSystem;

    private const string IS_RUNNING = "IsRunning";
    private const string IS_DYING = "IsDying";

    void Update()
    {
        playerAnimator.SetBool(IS_RUNNING, Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D));
        if(playerHealthSystem.IsPlayerDying() && !playerHealthSystem.IsPlayerAlreadyDead()){
            playerAnimator.SetTrigger(IS_DYING);
        }
    }
}
