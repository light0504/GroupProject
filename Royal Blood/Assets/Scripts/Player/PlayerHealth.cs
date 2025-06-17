using UnityEngine;
using UnityEngine.Playables;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    private PlayerState playerState;

    private void Awake()
    {
        currentHealth = maxHealth;
        playerState = GetComponent<PlayerState>();
    }

    public void TakeDamage(int damage)
    {
        UpdateHealth(-damage);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            playerState.Respawn();
        }
    }

    public int[] GetHealth()
    {
        return new int[] { currentHealth, maxHealth };
    }

    public void SetHealthData(int[] loadedHealth)
    {
        maxHealth = loadedHealth[0];
        currentHealth = loadedHealth[1] > 0 ? loadedHealth[1] : maxHealth;
    }

    public void UpdateHealth(int heal) 
    {
        currentHealth += heal;
    }

    public void SetMaxHealth()
    {
        currentHealth = maxHealth;
    }
}