using UnityEngine;

public class Button : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Wenn der Player in den Trigger reingeht
        if (other.CompareTag("Player"))
        {
            // respawne ihn
            GameManager.Instance.DeathZoneTrigger();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
