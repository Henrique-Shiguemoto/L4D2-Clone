using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour {
    [SerializeField] private GameObject zombieObject;

    private Transform playerCameraTransform;

    private GameObject[] zombiePool = new GameObject[30];
    private ZombieHealthSystem[] zombieHealthSystems = new ZombieHealthSystem[30];

    private List<GameObject> zombieSpawnerLocations = new List<GameObject>();
    private List<GameObject> currentlyAvailableSpawners = new List<GameObject>();

    void Awake(){
        playerCameraTransform = GameObject.Find("Main Camera").GetComponent<Transform>();
     
        foreach(Transform childrenTransform in transform) {
            if(childrenTransform.gameObject.tag.Equals("Spawner")) {
                zombieSpawnerLocations.Add(childrenTransform.gameObject);
                currentlyAvailableSpawners.Add(childrenTransform.gameObject);
            }
        }

        // Initialize zombiePool with zombie prefabs (different positions and rotations)
        for(int i = 0; i < zombiePool.Length; i++){
            zombiePool[i] = Instantiate(zombieObject, 
                                        GetRandomSpawnLocation(), 
                                        Quaternion.Euler(0, Random.Range(0, 359), 0));
            zombiePool[i].SetActive(true);
            zombiePool[i].transform.SetParent(transform);
            zombieHealthSystems[i] = zombiePool[i].GetComponent<ZombieHealthSystem>();
        }
    }

    void Update(){
        // Infected cannot be spawned in places where the player can see directly, so were verifying which spawn locations are available
        currentlyAvailableSpawners.Clear();
        for(int i = 0; i < zombieSpawnerLocations.Count; i++){
            Vector3 playerToSpawnerDirection = Vector3.Normalize(zombieSpawnerLocations[i].transform.position - playerCameraTransform.position);
            float dot = Vector3.Dot(playerToSpawnerDirection, playerCameraTransform.forward);
            
            if(dot < 0) currentlyAvailableSpawners.Add(zombieSpawnerLocations[i]);
            else{
                // even if the dot product isn't negative, there are still cases where the spawner could be blocked by a wall.
                Ray ray = new Ray(playerCameraTransform.position, playerToSpawnerDirection);
                bool cameraRaycastIsHittingSomething = Physics.Raycast(ray, out RaycastHit hit, Vector3.Distance(zombieSpawnerLocations[i].transform.position, playerCameraTransform.position));
                if(cameraRaycastIsHittingSomething && hit.transform.gameObject.tag.Equals("Walls-Ceiling-Floor")) {
                    // spawner is being blocked by wall
                    currentlyAvailableSpawners.Add(zombieSpawnerLocations[i]);
                }
            }
        }

        // Respawning zombies if necessary
        for(int i = 0; i < zombiePool.Length; i++){
            if(zombieHealthSystems[i].deathEventTriggered) RespawnOnDeathEventTriggered(i);
        }
    }

    void RespawnOnDeathEventTriggered(int i){
        zombieHealthSystems[i].SetRespawnFlag(true);
        zombiePool[i].transform.position = GetRandomSpawnLocation();
    }

    Vector3 GetRandomSpawnLocation(){
        int randomSpawnerLocationIndex = Mathf.FloorToInt(Random.Range(0, currentlyAvailableSpawners.Count));
        return currentlyAvailableSpawners[randomSpawnerLocationIndex].transform.position;
    }
}
