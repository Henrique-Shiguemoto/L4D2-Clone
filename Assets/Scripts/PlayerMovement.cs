using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour{
    [SerializeField] private float playerSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpForce;

    [SerializeField] private LayerMask groundMask;
    [SerializeField] private BoxCollider groundCheckCollider;

    [SerializeField] private CharacterController playerCharacterController;

    [SerializeField] private PlayerHealthSystem playerHealthSystem;

    //This is only used for Y velocity...
    Vector3 velocity;
    
    private bool isGrounded;

    void Update(){

        // we don't want to just return; here because the player might die on the air and then it'd be just floating on the air.
        if(playerHealthSystem.IsPlayerDying()){
            playerSpeed = 0.0f;
        }

        isGrounded = Physics.CheckBox(groundCheckCollider.bounds.center, 
                                        0.5f * groundCheckCollider.bounds.size, 
                                        Quaternion.identity, 
                                        groundMask);

        // Calculating and Applying gravity
        velocity.y += gravity * Time.deltaTime;

        // Checking if the player can jump
        if (isGrounded && velocity.y < 0){
            velocity.y = 0f;
            if (Input.GetButtonDown("Jump")) velocity.y += jumpForce;
        }
        playerCharacterController.Move(velocity * Time.deltaTime);

        // Movement at the direction we're pointing at
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        Vector3 normalizedMove = Vector3.Normalize(move);
        playerCharacterController.Move(normalizedMove * playerSpeed * Time.deltaTime);
    }
}
