using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    public string checkpointName;

    private bool isActivated = false;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            if (PlayerState.Instance != null)
            {
                PlayerState.Instance.SetNewCheckpoint(transform.position);
            }

            isActivated = true;
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.green;
            }
        }
    }
}