using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance { get; private set; }

    public float respawnDelay = 2f;
    public Vector3 CurrentCheckpointPosition { get; private set; }

    private PlayerHealth playerHealth;
    private PlayerAttack playerAttack;
    private PlayerMovement playerMovement;
    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private ItemPicker itemPicker;
    private Noti noti;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        playerHealth = GetComponent<PlayerHealth>();
        playerAttack = GetComponent<PlayerAttack>();
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        itemPicker = GetComponentInChildren<ItemPicker>();
        noti = GetComponent<Noti>();
    }

    public void OnPlayerDied()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        playerCollider.enabled = false;
        playerMovement.enabled = false;
        animator.SetBool("isDeath", true);
        noti.PrintText("Dưới sự nguyền rủa của quỷ vương Giảm 20 máu và 2 ATK");
        playerAttack.IncreaseAttack(-2);
        playerHealth.maxHealth -= 20;
        StartCoroutine(RespawnRoutine());
        playerHealth.CheckLose();
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        Respawn();
    }

    private void Respawn()
    {
        animator.SetBool("isDeath", false);
        animator.SetBool("isFalling", false);
        animator.SetBool("isRunning", false);

        transform.position = CurrentCheckpointPosition;

        playerCollider.enabled = true;
        playerMovement.enabled = true;
        if (rb != null)
            rb.bodyType = RigidbodyType2D.Dynamic;

        playerHealth.ResetState();
    }

    public void SetNewCheckpoint(Vector3 newPosition) => CurrentCheckpointPosition = newPosition;

    public void TriggerSaveGame()
    {
        if (SceneDataManager.Instance == null)
        {
            Debug.LogError("SceneDataManager not found!");
            return;
        }

        SaveSystem.SaveFullGameData(
            transform.position,
            playerHealth.GetCurrentHealth(),
            playerHealth.GetMaxHealth(),
            playerAttack.GetAttack(),
            itemPicker.GetTotalDiamonds()
        );
    }

    public void ApplyLoadedData(GameData data)
    {
        playerHealth.SetHealthData(data.playerCurrentHealth, data.playerMaxHealth);
        playerAttack.SetAttackData(data.playerAttackPower);
    }
}
