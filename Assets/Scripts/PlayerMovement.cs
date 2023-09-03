using UnityEngine;

public class PlayerMovement : MonoBehaviour{
    [SerializeField] private float playerSpeed;
    [SerializeField] private float scopedInSpeed;
    [HideInInspector] private float scopedOutSpeed;
    [SerializeField] private float crouchHeight;
    [SerializeField] private float crouchHeightForBoxCollider;
    [SerializeField] private float standingHeight;
    [SerializeField] private float standingHeightForBoxCollider;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private Vector3 crouchCenter;
    [SerializeField] private Vector3 standingCenter;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpForce;

    [SerializeField] private LayerMask groundMask;
    [SerializeField] private BoxCollider groundCheckCollider;

    [SerializeField] private CharacterController playerCharacterController;

    [SerializeField] private PlayerHealthSystem playerHealthSystem;

    //This is only used for Y velocity...
    private Vector3 velocity;
    
    private bool isGrounded;
    private bool isCrouched;

    private float playerMaxSpeed;

    private WeaponBehavior playerWeaponBehavior;

    void Awake(){
        playerWeaponBehavior = GameObject.Find("Weapon Holder").GetComponent<WeaponBehavior>();
    }

    void Start(){
        scopedInSpeed = 0.5f * playerSpeed;
        scopedOutSpeed = playerSpeed;
        playerMaxSpeed = playerSpeed;
    }

    void Update(){
        //edge cases
        {
            if(playerWeaponBehavior.isScoped) playerSpeed = scopedInSpeed;
            else playerSpeed = scopedOutSpeed;

            if(playerHealthSystem.IsPlayerDying()) playerSpeed = 0.0f;
        }

        //Jumping handling
        {
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
        }

        //Handling crouch
        {
            if(Input.GetKey(KeyCode.LeftControl) && !playerHealthSystem.IsPlayerDying()) {
                playerSpeed = 0.25f * playerMaxSpeed;
                playerCharacterController.height = crouchHeight;
                playerCharacterController.center = crouchCenter;
                groundCheckCollider.center = new Vector3(groundCheckCollider.center.x, crouchHeightForBoxCollider, groundCheckCollider.center.z);
            }else if(Input.GetKeyUp(KeyCode.LeftControl) && !playerHealthSystem.IsPlayerDying()){
                playerSpeed = playerMaxSpeed;
                playerCharacterController.height = standingHeight;
                playerCharacterController.center = standingCenter;
                groundCheckCollider.center = new Vector3(groundCheckCollider.center.x, standingHeightForBoxCollider, groundCheckCollider.center.z);
                Vector3 upVelocityToGetOutOfTheGround = new Vector3(0, Mathf.Abs(standingHeightForBoxCollider - crouchHeightForBoxCollider), 0);
                playerCharacterController.Move(upVelocityToGetOutOfTheGround);
            }
        }

        // Movement at the direction we're pointing at
        {
            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");
    
            Vector3 move = transform.right * x + transform.forward * z;
            Vector3 normalizedMove = Vector3.Normalize(move);
            playerCharacterController.Move(normalizedMove * playerSpeed * Time.deltaTime);
        }
    }
}
