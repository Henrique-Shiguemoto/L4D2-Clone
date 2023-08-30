using UnityEngine;
using TMPro;

public class TimerUpdate : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private PlayerHealthSystem playerHealthSystem;

    void Update(){
        if(!playerHealthSystem.IsPlayerDying()) timerText.text = Time.timeSinceLevelLoad.ToString("0.##");
    }
}
