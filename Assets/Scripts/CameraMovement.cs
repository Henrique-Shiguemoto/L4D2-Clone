using UnityEngine;

public class CameraMovement : MonoBehaviour{
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float distanceAwayFromPlayerAfterDeath = 1.0f;
    [SerializeField] private float distanceUpFromPlayerAfterDeath = 1.0f;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private PlayerHealthSystem playerHealthSystem;

    private float xRotation = 0f;
    private bool cameraAlreadyHasMovedAfterPlayerDied = false;

    void Start(){
        Cursor.lockState = CursorLockMode.Locked; // Cursor at screen center
    }

    void Update(){
        if(playerHealthSystem.IsPlayerDying()){
            if(!cameraAlreadyHasMovedAfterPlayerDied){
                transform.localPosition = transform.localPosition + transform.forward * distanceAwayFromPlayerAfterDeath;
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + distanceUpFromPlayerAfterDeath, transform.localPosition.z);
                LookAtDeadPlayer();
                cameraAlreadyHasMovedAfterPlayerDied = true;
            }
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -85f, 85f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerTransform.Rotate(Vector3.up * mouseX);
    }

    void LookAtDeadPlayer(){
        // Look at the player
        Vector3 dirToLook = Vector3.Normalize(playerTransform.position - transform.position);
        transform.rotation = Quaternion.LookRotation(dirToLook);
    }
}
