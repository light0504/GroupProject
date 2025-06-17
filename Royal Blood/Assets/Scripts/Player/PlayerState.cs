using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerHealth), typeof(PlayerAttack))]
public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance { get; private set; }
    public Vector3 CurrentCheckpointPosition { get; private set; }

    private PlayerHealth playerHealth;
    private PlayerAttack playerAttack;

    void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); return; }
        playerHealth = GetComponent<PlayerHealth>();
        playerAttack = GetComponent<PlayerAttack>();
        CurrentCheckpointPosition = transform.position;
    }

    public void SetNewCheckpoint(Vector3 newPosition) => CurrentCheckpointPosition = newPosition;

    public void TriggerSaveGame()
    {
        SaveSystem.SaveGame(new GameData(
            SceneManager.GetActiveScene().name,
            transform.position,
            playerHealth.GetHealth(),
            playerAttack.GetAttack()
        ));
    }

    public void ApplyLoadedData(GameData data)
    {
        playerHealth.SetHealthData(data.playerCurrentHealth);
        playerAttack.SetAttackData(data.playerAttackPower);
    }

    public void Respawn()
    {
        transform.position = CurrentCheckpointPosition;
        if (TryGetComponent<Rigidbody2D>(out var rb)) rb.linearVelocity = Vector2.zero;
        playerHealth.SetMaxHealth();
    }
}