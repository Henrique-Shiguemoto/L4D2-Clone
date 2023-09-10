using UnityEngine;

public class FlashlightControls : MonoBehaviour {
    [SerializeField] private float intensityOn = 1.25f;
    [SerializeField] private float intensityOff = 0.0f;
    [SerializeField] private bool flashLightStartsOn = true;

    private Light flashLight;
    private bool flashLightOn;

    void Start(){
        flashLight = GetComponent<Light>();
        flashLight.intensity = flashLightStartsOn ? intensityOn : intensityOff;
        flashLightOn = flashLightStartsOn;
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.F)) {
            flashLightOn = !flashLightOn;
            flashLight.intensity = flashLightOn ? intensityOn : intensityOff;
        }
    }
}
