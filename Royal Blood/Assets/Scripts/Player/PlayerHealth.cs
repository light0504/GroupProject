using UnityEngine;
using UnityEngine.Playables;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Stats")]
    public int maxHealth = 100;
    private int currentHealth;

    private PlayerState playerState;
    private PlayerMovement playerMovement;
    private Animator animator;
    private void Awake()
    {
        currentHealth = maxHealth;
        playerState = GetComponent<PlayerState>();
        playerMovement  = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        // Check if player is already dead or maybe invincible (e.g., during a dash).
        if (currentHealth <= 0 || playerMovement.GetDash())
        {
            return;
        }

        currentHealth -= damage;
        animator.SetTrigger("TakeHit"); // Play the player's "get hit" animation.
        Debug.Log("Player health: " + currentHealth);

        // Optional: Add a knockback effect here.
        // rb.AddForce(new Vector2(knockbackX, knockbackY), ForceMode2D.Impulse);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died!");  
        animator.SetBool("isDeath", true);

        // Disable player movement script and physics.
        this.enabled = false; // This disables the Update() loop of this script.
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        GetComponent<Collider2D>().enabled = false;

        playerState.Respawn();
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