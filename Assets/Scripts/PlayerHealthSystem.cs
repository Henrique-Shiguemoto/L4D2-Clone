using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthSystem : MonoBehaviour{
    public int maxHealth;
    [SerializeField] private TextMeshProUGUI healthBarText;
    [SerializeField] private Gradient healthBarGradient;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private RectTransform healthBarRectTransform;
    [HideInInspector] public int currentHealth;
    [Range(0, 1)] public float resistanceToThrowableDamage = 0.8f;

    private LevelManager levelManager;

    private float maxHealthBarLocalXScale;
    private bool isDying = false;
    private bool isAlreadyDead = false;

    void Awake(){
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
    }

    void Start(){
        currentHealth = maxHealth;
        maxHealthBarLocalXScale = healthBarRectTransform.localScale.x;
        healthBarImage.color = healthBarGradient.Evaluate(1.0f);
    }

    void Update(){
        if(isDying) isAlreadyDead = true;
    }

    public void Damage(int amount){
        currentHealth -= amount;
        if(currentHealth <= 0) {
            currentHealth = 0;
            isDying = true;
            levelManager.EndGame();
        }
        healthBarText.text = currentHealth.ToString();
        ChangeHealthBarWidthBasedOnHealth(currentHealth);
        ChangeHealthBarColorBasedOnHealth(currentHealth);
    }

    public void Heal(int amount){
        currentHealth += amount;
        if(currentHealth > maxHealth) currentHealth = maxHealth;
        healthBarText.text = currentHealth.ToString();
    }

    private void ChangeHealthBarWidthBasedOnHealth(float healthAmount){
        healthBarRectTransform.localScale = new Vector3(maxHealthBarLocalXScale * ((float)healthAmount / (float)maxHealth), healthBarRectTransform.localScale.y, healthBarRectTransform.localScale.z);
    }

    private void ChangeHealthBarColorBasedOnHealth(float healthAmount){
        healthBarImage.color = healthBarGradient.Evaluate((float)healthAmount / (float)maxHealth);
    }

    public bool IsPlayerDying(){
        return isDying;
    }

    public bool IsPlayerAlreadyDead(){
        return isAlreadyDead;
    }
}
