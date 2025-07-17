using UnityEngine;
using TMPro; // For using TextMeshProUGUI

// Manages the collectible count and UI display
public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance; // Singleton instance to ensure global access

    public TextMeshProUGUI counterText; // Reference to the UI text element that shows the collectible count
    private int collectedCount = 0;     // Tracks the number of collected items

    void Awake()
    {
        // Set up the singleton instance
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // Destroy duplicate instances to maintain a single manager
    }

    void Start()
    {
        // Initialize the UI counter at the start of the game
        UpdateCounterUI();
    }

    // Call this method when an item is collected
    public void AddCollectible()
    {
        collectedCount++;     // Increase the collected item count
        UpdateCounterUI();    // Update the UI to reflect the new count
    }

    // Updates the UI counter with the current collectible count
    void UpdateCounterUI()
    {
        if (counterText != null)
            counterText.text = "" + collectedCount; // Display the count as text
    }
}
