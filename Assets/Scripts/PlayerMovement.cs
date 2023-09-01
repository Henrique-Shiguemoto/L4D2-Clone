using UnityEngine;

public class PlayerMovement : MonoBehaviour{
    [SerializeField] private float playerSpeed;
    [SerializeField] private float scopedInSpeed;
    [SerializeField] private float scopedOutSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpForce;

    [SerializeField] private LayerMask groundMask;
    [SerializeField] private BoxCollider groundCheckCollider;

    [SerializeField] private CharacterController playerCharacterController;

    [SerializeField] private PlayerHealthSystem playerHealthSystem;

    //This is only used for Y velocity...
    Vector3 velocity;
    
    private bool isGrounded;

    private WeaponBehavior playerWeaponBehavior;

    void Awake(){
        playerWeaponBehavior = GameObject.Find("Weapon Holder").GetComponent<WeaponBehavior>();
    }

    void Start(){
        scopedInSpeed = 0.5f * playerSpeed;
        scopedOutSpeed = playerSpeed;
    }

    void Update(){
        if(playerWeaponBehavior.isScoped) playerSpeed = scopedInSpeed;
        else playerSpeed = scopedOutSpeed;

        if(playerHealthSystem.IsPlayerDying()) playerSpeed = 0.0f;

        isGrounded = Physics.CheckBox(groundCheckCollider.bounds.center, 
                                        0.5f * groundCheckCollider.bounds.size, 
                                        Quaternion.identity, 
                                        groundMask);

        // Calculating and Applying gravity
        velocity.y += gravity * Time.deltaTime;

        // Checking if the player can jump
        if (isGrounded && velocity.y < 0){
            velocity.y = 0f;
            if (Input.GetButtonDown("Jump") && !playerHealthSystem.IsPlayerDying()) velocity.y += jumpForce;
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
