using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieMovement : MonoBehaviour {
    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private float walkSpeed = 1.0f;
    [SerializeField] private float rotationSpeed = 1.0f;
    [SerializeField] private float gravity = 1.0f;
    [SerializeField] private float timeToWalkAgain = 5.0f;
    [SerializeField] private float timeToIdleAgain = 5.0f;
    [SerializeField] private float maxDistanceForZombieToFollow = 10.0f;
    [SerializeField] private float minDistanceFromPlayerToAttack = 1.0f;
    [SerializeField] private float radiusToWalk = 10.0f;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform zombieTransform;
    [SerializeField] private CharacterController zombieCharacterController;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private BoxCollider groundCheckCollider;

    private bool isIdling = true;
    private bool isRunning = false;
    private bool isWalking = false;
    private bool isAttacking = false;
    private bool isDying = false;
    
    private bool isGrounded;
    private float distanceFromPlayer;
    private float timeLeftToIdleAgain;
    private float timeLeftToWalkAgain;
    private Transform zombieVisualTransform;
    private Vector3 nextWalkDirectionToFollow;
    private Vector3 downVelocity;

    void Start(){
        zombieVisualTransform = transform.GetChild(0).GetComponent<Transform>();
        timeToWalkAgain = Random.Range(0.5f * timeToWalkAgain, 1.5f * timeToWalkAgain);
        timeToIdleAgain = Random.Range(0.5f * timeToIdleAgain, 1.5f * timeToIdleAgain);
        timeLeftToIdleAgain = timeToWalkAgain;
        timeLeftToWalkAgain = timeToIdleAgain;
        nextWalkDirectionToFollow = zombieVisualTransform.forward;
        downVelocity = Vector3.zero;
        transform.Rotate(0, Random.Range(0, 359), 0);
    }

    void Update(){        
        zombieVisualTransform.localPosition = new Vector3(0.0f, zombieVisualTransform.localPosition.y, 0.0f);
        Vector3 zombiePlayerDirection = Vector3.Normalize(playerTransform.position - zombieTransform.position);
        distanceFromPlayer = Vector3.Distance(playerTransform.position, zombieTransform.position);

        // this condition needs to change (use field of view)
        if(distanceFromPlayer <= maxDistanceForZombieToFollow && distanceFromPlayer > minDistanceFromPlayerToAttack){
            isRunning = true;
        }
        if(isRunning){
            zombieCharacterController.Move(zombiePlayerDirection * movementSpeed * Time.deltaTime);

            // Look towards the player when running at the player
            zombieTransform.rotation = Quaternion.Lerp(zombieTransform.rotation, Quaternion.LookRotation(zombiePlayerDirection), rotationSpeed * Time.deltaTime);

            if(distanceFromPlayer <= minDistanceFromPlayerToAttack){
                isAttacking = true;
                isRunning = false;
                isWalking = false;
                isIdling = false;
            }else{
                isAttacking = false;
            }
        }else{
            // Zombies, when they're not aggressive, will either walk around a little bit or just idle.
            if(isIdling){
                timeLeftToWalkAgain -= Time.deltaTime;
                if(timeLeftToWalkAgain <= 0f){
                    isIdling = false;
                    isWalking = true;
                    timeLeftToWalkAgain = timeToWalkAgain;
                    
                    float xRandomOffset = Random.Range(-radiusToWalk, radiusToWalk);
                    float zRandomOffset = Random.Range(-radiusToWalk, radiusToWalk);

                    nextWalkDirectionToFollow = new Vector3(zombieTransform.position.x + xRandomOffset,
                                                            zombieVisualTransform.position.y,
                                                            zombieTransform.position.z + zRandomOffset);
                    nextWalkDirectionToFollow = nextWalkDirectionToFollow - zombieVisualTransform.position;
                    nextWalkDirectionToFollow = Vector3.Normalize(nextWalkDirectionToFollow);
                }
            }else if(isWalking){
                timeLeftToIdleAgain -= Time.deltaTime;
                if(timeLeftToIdleAgain <= 0f){
                    isWalking = false;
                    isIdling = true;
                    timeLeftToIdleAgain = timeToIdleAgain;
                }
                zombieCharacterController.Move(nextWalkDirectionToFollow * walkSpeed * Time.deltaTime);

                zombieTransform.rotation = Quaternion.Lerp(zombieTransform.rotation, Quaternion.LookRotation(nextWalkDirectionToFollow), rotationSpeed * Time.deltaTime);
            }else if(isAttacking){
                if(distanceFromPlayer > minDistanceFromPlayerToAttack){
                    isAttacking = false;
                    isRunning = true;
                }
                zombieTransform.LookAt(playerTransform);
                // zombieTransform.rotation = Quaternion.Lerp(zombieTransform.rotation, Quaternion.LookRotation(zombiePlayerDirection), rotationSpeed * Time.deltaTime);
            }
        }

        //Apply gravity
        isGrounded = Physics.CheckBox(groundCheckCollider.bounds.center, 
                                        0.5f * groundCheckCollider.bounds.size, 
                                        Quaternion.identity, 
                                        groundMask);

        downVelocity.y += gravity * Time.deltaTime;
        if (isGrounded && downVelocity.y < 0) downVelocity.y = 0f;
        zombieCharacterController.Move(downVelocity * Time.deltaTime);
    }

    public bool IsZombieRunning(){
        return isRunning;
    }

    public bool IsZombieWalking(){
        return isWalking;
    }

    public bool IsZombieDying(){
        return isDying;
    }

    public bool IsZombieAttacking(){
        return isAttacking;
    }
}
