using System.Collections;
using UnityEngine;

public class ThrowableBehavior : MonoBehaviour {
    [SerializeField] private float pickupRange;
    [SerializeField] private float dropUpwardForce;
    [SerializeField] private float dropForwardForce;
    [SerializeField] private float pipeBombExplosionRadius = 5.5f;
    [SerializeField] private int pipeBombExplosionDamage = 100;
    [SerializeField] private float throwUpwardForce;
    [SerializeField] private float throwForwardForce;
    [SerializeField] private float timeForPipeBombToExplode = 5.0f;
    [SerializeField] private GameObject explosionParticleSystem;

    [HideInInspector] public bool throwableHasBeenThrown = false;
    [HideInInspector] public bool isThrowing = false;

    private Camera cameraObject;
    private Inventory playerInventory = null;
    
    //refactor this (grenade config)
    private AudioSource explosionAudio;
    private AudioSource groundImpactAudio;

    private float timeForThrowableToGetDestroyed = 1.0f;

    public GameObject thrownThrowable = null;

    void Awake(){
        cameraObject = GameObject.Find("Main Camera").GetComponent<Camera>();
        playerInventory = GameObject.Find("MainPlayer").GetComponent<Inventory>();
    }

    void Update(){
        HandleThrowablePickup();
        HandleThrow();
    }

    void PickupThrowable(GameObject newThrowable){
        if(playerInventory.HasThrowable()){
            playerInventory.throwable.SetActive(true);
            DropThrowable(playerInventory.throwable, false);
        }

        newThrowable.transform.GetChild(0).gameObject.GetComponent<Animator>().enabled = true;

        playerInventory.throwable = newThrowable;

        newThrowable.GetComponent<Rigidbody>().isKinematic = true;
        newThrowable.GetComponent<BoxCollider>().isTrigger = true;

        newThrowable.transform.SetParent(transform);
        newThrowable.transform.localPosition = Vector3.zero;
        newThrowable.transform.localRotation = Quaternion.Euler(Vector3.zero);
        newThrowable.transform.localScale = Vector3.one;
        
        AudioSource[] audios = playerInventory.throwable.GetComponents<AudioSource>();
        explosionAudio = audios[0];
        groundImpactAudio = audios[1];
    }

    void DropThrowable(GameObject throwableToDrop, bool throwing){
        if(playerInventory.HasThrowable()){
            throwableToDrop.transform.SetParent(null);
            throwableToDrop.GetComponent<Rigidbody>().isKinematic = false;
            throwableToDrop.GetComponent<BoxCollider>().isTrigger = false;

            GameObject visuals = throwableToDrop.transform.GetChild(0).gameObject;
            visuals.GetComponent<Animator>().enabled = false;

            //Add forces to weapon when dropped (up and forward)
            throwableToDrop.GetComponent<Rigidbody>().AddForce(dropUpwardForce * cameraObject.transform.up, ForceMode.Impulse);
            throwableToDrop.GetComponent<Rigidbody>().AddForce(dropForwardForce * cameraObject.transform.forward, ForceMode.Impulse);

            if(throwing){
                thrownThrowable = throwableToDrop;
                StartCoroutine(OnPipeBombThrown());
            }
            playerInventory.throwable = null;
        }
    }

    void HandleThrowablePickup(){
        Ray ray = cameraObject.ScreenPointToRay(Input.mousePosition);
        bool cameraRaycastIsHittingSomething = Physics.Raycast(ray, out RaycastHit hit);

        if (cameraRaycastIsHittingSomething && hit.transform.gameObject.tag.Equals("Throwable") && Input.GetKeyDown(KeyCode.E) && hit.distance <= pickupRange){
            PickupThrowable(hit.transform.gameObject);
        }
    }

    void HandleThrow(){
        if(playerInventory.IsHoldingThrowable() && Input.GetButtonDown("Fire1")) isThrowing = true;
    }

    IEnumerator OnPipeBombExplosion(){
        yield return new WaitForSeconds(timeForPipeBombToExplode);
        explosionAudio.Play();
        thrownThrowable.GetComponent<BoxCollider>().enabled = false;
        thrownThrowable.transform.GetChild(0).gameObject.SetActive(false);
        
        GameObject explosion = Instantiate(explosionParticleSystem, thrownThrowable.transform.position, thrownThrowable.transform.rotation);
        Destroy(explosion, 1.0f);

        Collider[] collidersInsidePipeBombRadius = Physics.OverlapSphere(thrownThrowable.transform.position, pipeBombExplosionRadius);
        bool alreadyDamagedPlayer = false;
        foreach(Collider collider in collidersInsidePipeBombRadius){
            if(collider.gameObject.tag.Equals("Zombie")) collider.gameObject.GetComponent<ZombieHealthSystem>().Damage(pipeBombExplosionDamage, false);
            if(collider.gameObject.tag.Equals("Player") && !alreadyDamagedPlayer) {
                PlayerHealthSystem phs = collider.gameObject.GetComponent<PlayerHealthSystem>();
                phs.Damage((int)(pipeBombExplosionDamage * (1 - phs.resistanceToThrowableDamage)));
                alreadyDamagedPlayer = true;
            }
        }

        Destroy(thrownThrowable, timeForThrowableToGetDestroyed);
        thrownThrowable = null;

        throwableHasBeenThrown = false;
    }

    IEnumerator OnPipeBombThrown(){
        yield return new WaitForSeconds(0.5f);
        throwableHasBeenThrown = true;
    }

    public void Throw(){
        float originalForceUp = dropUpwardForce;
        float originalForceForward = dropForwardForce;
        dropUpwardForce = throwUpwardForce;
        dropForwardForce = throwForwardForce;
        DropThrowable(playerInventory.GetCurrentHeldObject(), true);
        dropUpwardForce = originalForceUp;
        dropForwardForce = originalForceForward;
        playerInventory.ChangeHeldObjectToDefault();
        StartCoroutine(OnPipeBombExplosion());
    }
}
