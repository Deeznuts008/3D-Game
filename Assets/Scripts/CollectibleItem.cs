using UnityEngine;

// Handles collectible item behavior (trigger, sound, destruction, and notifying manager)
public class CollectibleItem : MonoBehaviour
{
    public string itemName = "Item";          // Optional: name of the item
    public AudioClip collectSound;            // Optional: sound to play when collected

    private AudioSource audioSource;          // Audio source component to play sound
    private bool collected = false;           // Prevents collecting the item multiple times

    void Start()
    {
        // Add an AudioSource component for playing collection sound
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return; // Skip if already collected

        if (other.CompareTag("Player"))
        {
            collected = true;  // Mark as collected
            OnCollect();       // Handle collection logic
        }
    }

    public void OnCollect()
    {
        // Notify the CollectibleManager to update the counter
        CollectibleManager.Instance.AddCollectible();

        // Play the collection sound (if any), then destroy the object
        if (collectSound != null)
        {
            audioSource.PlayOneShot(collectSound);
            Destroy(gameObject, collectSound.length); // Destroy after sound finishes
        }
        else
        {
            Destroy(gameObject); // Destroy immediately if no sound
        }
    }
}
