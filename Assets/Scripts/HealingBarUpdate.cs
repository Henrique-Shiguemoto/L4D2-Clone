using UnityEngine;

public class HealingBarUpdate : MonoBehaviour {
    [SerializeField] private RectTransform healthBarRectTransform;

    private HealthpackBehavior healthPackBehavior;

    void Awake(){
        healthPackBehavior = GameObject.Find("Weapon Holder").GetComponent<HealthpackBehavior>();
    }

    void Update(){
        healthBarRectTransform.localScale = new Vector3(healthPackBehavior.HealingProgress(), healthBarRectTransform.localScale.y, healthBarRectTransform.localScale.z);
    }
}
