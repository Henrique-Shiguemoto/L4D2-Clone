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

    private Camera cameraObject;
    private GameObject currentThrowable;
    private bool playerIsHoldingThrowable = false;
    
    [HideInInspector] public bool throwableHasBeenThrown = false;

    private AudioSource explosionAudio;
    private AudioSource groundImpactAudio;

    private float timeForThrowableToGetDestroyed = 1.0f;

    void Awake(){
        cameraObject = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    void Start(){
        currentThrowable = null;
    }

    void Update(){
        // grenade pickup
        Ray ray = cameraObject.ScreenPointToRay(Input.mousePosition);
        bool cameraRaycastIsHittingSomething = Physics.Raycast(ray, out RaycastHit hit);

        if (cameraRaycastIsHittingSomething && hit.transform.gameObject.tag.Equals("Throwable") && Input.GetKeyDown(KeyCode.E) && hit.distance <= pickupRange){
            PickupThrowable(hit.transform.gameObject);
        }

        if(playerIsHoldingThrowable){
            if(Input.GetButtonDown("Fire1")){
                //throw
                throwableHasBeenThrown = true;
                float originalForceUp = dropUpwardForce;
                float originalForceForward = dropForwardForce;
                dropUpwardForce = throwUpwardForce;
                dropForwardForce = throwForwardForce;
                DropThrowable(currentThrowable);
                currentThrowable.name = "PipeBombThrown";
                dropUpwardForce = originalForceUp;
                dropForwardForce = originalForceForward;
                StartCoroutine(OnPipeBombExplosion());
            }
        }
    }

    void PickupThrowable(GameObject newThrowable){
        // Debug.Log("Picked up throwable");
        if(playerIsHoldingThrowable) DropThrowable(currentThrowable);

        newThrowable.GetComponent<Rigidbody>().isKinematic = true;
        newThrowable.GetComponent<BoxCollider>().isTrigger = true;
        playerIsHoldingThrowable = true;

        newThrowable.transform.SetParent(transform);
        newThrowable.transform.localPosition = Vector3.zero;
        newThrowable.transform.localRotation = Quaternion.Euler(Vector3.zero);
        newThrowable.transform.localScale = Vector3.one;

        currentThrowable = newThrowable;
        
        AudioSource[] audios = currentThrowable.GetComponents<AudioSource>();
        explosionAudio = audios[0];
        groundImpactAudio = audios[1];
    }

    void DropThrowable(GameObject throwableToDrop){
        // Debug.Log("Dropped throwable");
        if(throwableToDrop == null) return;
        if(playerIsHoldingThrowable){
            throwableToDrop.transform.SetParent(null);
            throwableToDrop.GetComponent<Rigidbody>().isKinematic = false;
            throwableToDrop.GetComponent<BoxCollider>().isTrigger = false;

            //Add forces to weapon when dropped (up and forward)
            throwableToDrop.GetComponent<Rigidbody>().AddForce(dropUpwardForce * cameraObject.transform.up, ForceMode.Impulse);
            throwableToDrop.GetComponent<Rigidbody>().AddForce(dropForwardForce * cameraObject.transform.forward, ForceMode.Impulse);

            playerIsHoldingThrowable = false;
        }
    }

    IEnumerator OnPipeBombExplosion(){
        yield return new WaitForSeconds(timeForPipeBombToExplode);
        explosionAudio.Play();
        currentThrowable.GetComponent<BoxCollider>().enabled = false;
        currentThrowable.transform.GetChild(0).gameObject.SetActive(false);
        throwableHasBeenThrown = false;
        
        Collider[] collidersInsidePipeBombRadius = Physics.OverlapSphere(currentThrowable.transform.position, pipeBombExplosionRadius);
        foreach(Collider collider in collidersInsidePipeBombRadius){
            if(collider.gameObject.tag.Equals("Zombie")) collider.gameObject.GetComponent<ZombieHealthSystem>().Damage(pipeBombExplosionDamage);
        }

        Destroy(currentThrowable, timeForThrowableToGetDestroyed);
    }
}
