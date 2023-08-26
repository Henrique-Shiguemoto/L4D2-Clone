using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieNavigation : MonoBehaviour {
    [SerializeField] private float maxDistanceForZombieToFollow = 10.0f;
    [SerializeField] private float timeToWalkAgain = 5.0f;
    [SerializeField] private float timeToIdleAgain = 5.0f;
    [SerializeField] private float radiusToWalk = 5.0f;

    private float timeLeftToIdleAgain;
    private float timeLeftToWalkAgain;

    private Transform targetToFollow;
    private NavMeshAgent zombieAgent;

    private bool isIdling = true;
    private bool isRunning = false;
    private bool isWalking = false;

    void Start(){
        zombieAgent = GetComponent<NavMeshAgent>();
        targetToFollow = GameObject.Find("MainPlayer").GetComponent<Transform>();

        timeToWalkAgain = Random.Range(0.5f * timeToWalkAgain, 1.5f * timeToWalkAgain);
        timeToIdleAgain = Random.Range(0.5f * timeToIdleAgain, 1.5f * timeToIdleAgain);
        timeLeftToIdleAgain = timeToWalkAgain;
        timeLeftToWalkAgain = timeToIdleAgain;
    }

    void Update(){
        float distanceToTarget = Vector3.Distance(targetToFollow.position, transform.position);
        if(distanceToTarget <= maxDistanceForZombieToFollow){
            zombieAgent.SetDestination(targetToFollow.position);
        }else{
            //zombie will take turns idling and walking
            if(isIdling) {
                timeLeftToWalkAgain -= Time.deltaTime;
                
                if(timeLeftToWalkAgain <= 0f){
                    isIdling = false;
                    isWalking = true;
                    timeLeftToWalkAgain = timeToWalkAgain;
                    
                    // it will search for a point where it has a path to.
                    do{
                        float xRandomOffset = Random.Range(-radiusToWalk, radiusToWalk);
                        float zRandomOffset = Random.Range(-radiusToWalk, radiusToWalk);
                        zombieAgent.SetDestination(new Vector3(transform.position.x + xRandomOffset, 
                                                               transform.position.y, 
                                                               transform.position.z + zRandomOffset));
                    }while(!zombieAgent.hasPath);
                }else{
                    zombieAgent.SetDestination(transform.position);
                }
            }else if(isWalking){
                timeLeftToIdleAgain -= Time.deltaTime;
                if(timeLeftToIdleAgain <= 0f){
                    isWalking = false;
                    isIdling = true;
                    timeLeftToIdleAgain = timeToIdleAgain;
                }
            }

            // if(isIdling){
            //     timeLeftToWalkAgain -= Time.deltaTime;
            //     if(timeLeftToWalkAgain <= 0f){
            //         isIdling = false;
            //         isWalking = true;
            //         timeLeftToWalkAgain = timeToWalkAgain;
                    
            //         float xRandomOffset = Random.Range(-radiusToWalk, radiusToWalk);
            //         float zRandomOffset = Random.Range(-radiusToWalk, radiusToWalk);

            //         nextWalkDirectionToFollow = new Vector3(transform.position.x + xRandomOffset,
            //                                                 zombieVisualTransform.position.y,
            //                                                 transform.position.z + zRandomOffset);
            //         nextWalkDirectionToFollow = nextWalkDirectionToFollow - zombieVisualTransform.position;
            //         nextWalkDirectionToFollow = Vector3.Normalize(nextWalkDirectionToFollow);
            //     }
            // }else if(isWalking){
            //     timeLeftToIdleAgain -= Time.deltaTime;
            //     if(timeLeftToIdleAgain <= 0f){
            //         isWalking = false;
            //         isIdling = true;
            //         timeLeftToIdleAgain = timeToIdleAgain;
            //     }
            //     zombieCharacterController.Move(nextWalkDirectionToFollow * walkSpeed * Time.deltaTime);

            //     transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(nextWalkDirectionToFollow), rotationSpeed * Time.deltaTime);
            // }
        }
    }
}
