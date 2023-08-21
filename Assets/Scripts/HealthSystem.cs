public class HealthSystem {
    public int maxHealth;
    public int currentHealth;

    HealthSystem(int startingHealth){
        maxHealth = startingHealth;
        currentHealth = maxHealth;
    }

    void Damage(int amount){
        currentHealth -= amount;
        if(currentHealth < 0) currentHealth = 0;
    }

    void Heal(int amount){
        currentHealth += amount;
        if(currentHealth > maxHealth) currentHealth = maxHealth;
    }    
}
