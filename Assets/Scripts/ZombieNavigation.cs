using UnityEngine;
using UnityEngine.AI;

public class ZombieNavigation : MonoBehaviour {
    [SerializeField] private float maxDistanceForZombieToFollow = 10.0f;
    [SerializeField] private float timeToWalkAgain = 5.0f;
    [SerializeField] private float timeToIdleAgain = 5.0f;
    [SerializeField] private float radiusToWalk = 10.0f;
    [SerializeField] private float walkSpeed = 1.0f;
    [SerializeField] private float runningSpeed = 3.5f;
    [SerializeField] private float minDistanceToAttack = 2.3f;

    private float timeLeftToIdleAgain;
    private float timeLeftToWalkAgain;

    private Transform targetToFollow;
    private Transform mainTargetToFollow;
    private PlayerHealthSystem playerHealthSystem;
    private ZombieHealthSystem zombieHealthSystem;
    private ThrowableBehavior throwableBehaviorScript;
    private NavMeshAgent zombieAgent;

    [HideInInspector] public bool isIdling = true;
    [HideInInspector] public bool isRunning = false;
    [HideInInspector] public bool isWalking = false;
    [HideInInspector] public bool isAttacking = false;

    void Awake(){
        zombieAgent = GetComponent<NavMeshAgent>();
        targetToFollow = GameObject.Find("MainPlayer").GetComponent<Transform>();
        mainTargetToFollow = targetToFollow;
        playerHealthSystem = targetToFollow.GetComponent<PlayerHealthSystem>();
        zombieHealthSystem = GetComponent<ZombieHealthSystem>();
        throwableBehaviorScript = GameObject.Find("Weapon Holder").GetComponent<ThrowableBehavior>();
    }

    void Start(){
        SetTimers();
    }

    void Update(){
        // Edge cases
        if(zombieHealthSystem.IsZombieDying() && !zombieHealthSystem.ZombieHasRespawned()){
            MakeZombieNotDoAnything();
            return;
        }
        if(playerHealthSystem.IsPlayerDying()) {
            ChangeToIdle();
            return;
        }
        if(zombieHealthSystem.ZombieHasRespawned()) {
            SetTimers();
            ChangeToIdle();
        }
        if(throwableBehaviorScript.throwableHasBeenThrown){
            targetToFollow = GameObject.Find("PipeBombThrown").transform;
        }else{
            targetToFollow = mainTargetToFollow;
        }

        float distanceToTarget = Vector3.Distance(targetToFollow.position, transform.position);
        if(distanceToTarget <= maxDistanceForZombieToFollow && !isAttacking && !zombieHealthSystem.IsZombieDying()) ChangeToRunning();
        if(isRunning){
            zombieAgent.SetDestination(targetToFollow.position);
            if(ZombieIsCloseEnoughToTarget(targetToFollow.position, minDistanceToAttack)) ChangeToAttacking();
        }else{
            //zombie will take turns idling and walking, or it could be attacking (it doesn't move while attacking)
            if(isIdling){
                timeLeftToWalkAgain -= Time.deltaTime;
                if(timeLeftToWalkAgain <= 0f){
                    ChangeToWalking();
                    timeLeftToWalkAgain = timeToWalkAgain;
                    
                    float xRandomOffset = Random.Range(-radiusToWalk, radiusToWalk);
                    float zRandomOffset = Random.Range(-radiusToWalk, radiusToWalk);
                    zombieAgent.SetDestination(new Vector3(transform.position.x + xRandomOffset, transform.position.y, transform.position.z + zRandomOffset));
                }
                else StopMovingAgent();
            }else if(isWalking){
                timeLeftToIdleAgain -= Time.deltaTime;
                if(ZombieIsCloseEnoughToTarget(zombieAgent.destination, 1.01f) || timeLeftToIdleAgain <= 0f){
                    ChangeToIdle();
                    timeLeftToIdleAgain = timeToIdleAgain;
                }
            }else if(isAttacking){
                if(!ZombieIsCloseEnoughToTarget(targetToFollow.position, minDistanceToAttack)) ChangeToRunning();
                StopMovingAgent();
                if(!targetToFollow.gameObject.name.Equals("PipeBombThrown")){
                    transform.LookAt(targetToFollow);
                }
            }
        }
    }

    public void StopMovingAgent(){
        zombieAgent.SetDestination(transform.position);
        zombieAgent.speed = 0.0f;
    }

    private bool ZombieIsCloseEnoughToTarget(Vector3 target, float threshold){
        return Vector3.Distance(transform.position, target) <= threshold;
    }

    private void MakeZombieNotDoAnything(){
        isIdling = isWalking = isRunning = isAttacking = false;
        StopMovingAgent();
    }

    private void ChangeToIdle(){
        isIdling = true;
        isWalking = isRunning = isAttacking = false;
        StopMovingAgent();
    }

    private void ChangeToWalking(){
        isWalking = true;
        isIdling = isRunning = isAttacking = false;
        zombieAgent.speed = walkSpeed;
    }

    private void ChangeToRunning(){
        isRunning = true;
        isIdling = isWalking = isAttacking = false;
        zombieAgent.speed = runningSpeed;
    }

    private void ChangeToAttacking(){
        isAttacking = true;
        isIdling = isWalking = isRunning = false;        
    }

    private void SetTimers(){
        timeToWalkAgain = Random.Range(0.85f * timeToWalkAgain, 2.1f * timeToWalkAgain);
        timeToIdleAgain = Random.Range(0.85f * timeToIdleAgain, 2.1f * timeToIdleAgain);
        timeLeftToIdleAgain = timeToWalkAgain;
        timeLeftToWalkAgain = timeToIdleAgain;
    }
}
