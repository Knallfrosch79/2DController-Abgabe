using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private GameObject player;

    private void Reset()
    {
        // Fallback, falls Du im Inspector nichts zugewiesen hast
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Wenn der Player in den Trigger reingeht
        if (other.CompareTag("Player"))
        {
            // respawne ihn
            GameManager.Instance.DeathZoneTrigger();
        }
    }
}

