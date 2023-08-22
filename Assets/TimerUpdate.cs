using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerUpdate : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI timerText;

    void Update(){
        timerText.text = Time.timeSinceLevelLoad.ToString("0.##");
    }
}
