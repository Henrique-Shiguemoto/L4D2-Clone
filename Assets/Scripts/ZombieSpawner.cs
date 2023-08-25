using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour {
    [SerializeField] private GameObject zombieObject;

    GameObject[] zombiePool = new GameObject[30];

    void Start(){
        // Initialize zombiePool with zombie prefabs (different positions and rotations)
        for(int i = 0; i < zombiePool.Length; i++){
            zombiePool[i] = Instantiate(zombieObject, 
                                        new Vector3(Random.Range(-10, 10), 5, Random.Range(-10, 10)), 
                                        Quaternion.Euler(0, Random.Range(0, 359), 0));
            zombiePool[i].SetActive(true);
            zombiePool[i].transform.SetParent(transform);
        }
    }

    void Update(){
        
    }
}
