using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float playerSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpForce;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private CharacterController playerCharacterController;
    [SerializeField] private BoxCollider playerBoxCollider;

    //This is only used for Y velocity...
    Vector3 velocity;
    
    private bool isGrounded;

    void Update()
    {
        isGrounded = Physics.CheckBox(playerBoxCollider.bounds.center, 0.5f * playerBoxCollider.bounds.size, Quaternion.identity, groundMask);

        // Checking if the player can jump
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0f;
            if (Input.GetButtonDown("Jump"))
            {
                // Jump!
                velocity.y += jumpForce;
            }
        }

        // Calculating and Applying gravity
        velocity.y += gravity * Time.deltaTime;
        playerCharacterController.Move(velocity * Time.deltaTime);

        // Movement at the direction we're pointing at
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        playerCharacterController.Move(move * playerSpeed * Time.deltaTime);
    }
}
