using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private float maxDistanceForZombieToFollow = 10.0f;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform zombieTransform;
    [SerializeField] private CharacterController zombieCharacterController;

    private bool isRunning = false;
    private float distanceFromPlayer;
    private Transform zombieVisualTransform;

    void Start()
    {
        zombieVisualTransform = transform.GetChild(0).GetComponent<Transform>();
    }

    void Update()
    {
        zombieTransform.LookAt(playerTransform);
        
        zombieVisualTransform.localPosition = new Vector3(0.0f, zombieVisualTransform.localPosition.y, 0.0f);
        Vector3 zombiePlayerDirection = Vector3.Normalize(playerTransform.position - zombieTransform.position);
        distanceFromPlayer = Vector3.Distance(playerTransform.position, zombieTransform.position);

        isRunning = distanceFromPlayer <= maxDistanceForZombieToFollow;
        if(isRunning){
            // We need to do this because, if the player jumps, the zombie will start to levitate to get to a higher Y value.
            // In this game, zombies don't jump! They stay in their same heights relative to the floor they're on top of.
            Vector3 target = playerTransform.position;
            target.y = zombieTransform.position.y;
            // zombieTransform.position = Vector3.MoveTowards(zombieTransform.position, target, movementSpeed * Time.deltaTime);

            Vector3 moveDir = Vector3.Normalize(target - zombieTransform.position);

            zombieCharacterController.Move(moveDir * movementSpeed * Time.deltaTime);
        }
    }

    public bool IsZombieRunning(){
        return isRunning;
    }
}
