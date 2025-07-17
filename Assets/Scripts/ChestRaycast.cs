using UnityEngine;
using TMPro;

public class ChestRaycast : MonoBehaviour
{
    // Assign in the inspector or it defaults to Camera.main
    public Camera playerCamera;

    // Animator for chest opening animation
    public Animator animator;

    // The sword prefab to spawn when the chest opens
    public GameObject swordPrefab;

    // Where the sword will spawn
    public Transform swordSpawnPoint;

    // UI text to display item message
    public TextMeshProUGUI itemMessageText;

    // Duration the message will be shown on screen
    public float messageDuration = 10f;

    // Sound clip for when the chest opens
    public AudioClip chestOpenSound;

    // Audio source component to play the sound
    private AudioSource audioSource;

    // Tracks whether the chest has already been opened
    private bool isOpen = false;

    // Internal timer to hide the item message after a few seconds
    private float messageTimer = 10f;

    void Start()
    {
        // Use the main camera if none assigned
        if (playerCamera == null)
            playerCamera = Camera.main;

        // Get animator on this object if none assigned
        if (animator == null)
            animator = GetComponent<Animator>();

        // Clear item message at start
        if (itemMessageText != null)
            itemMessageText.text = "";

        // Get or add an AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Configure the AudioSource for 2D sound (non-positional)
        audioSource.spatialBlend = 0f;
        audioSource.volume = 1f;
    }

    void Update()
    {
        // Detect E key press to open chest if it's under the cursor
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 10f))
            {
                // Only open the chest if it's the one being looked at and hasn't been opened
                if (hit.collider.gameObject == gameObject && !isOpen)
                {
                    OpenChest();
                }
            }
        }

        // Countdown for hiding the message text
        if (itemMessageText != null && messageTimer > 0)
        {
            messageTimer -= Time.deltaTime;

            if (messageTimer <= 0)
            {
                itemMessageText.text = "";
            }
        }
    }

    // Handles chest opening logic
    void OpenChest()
    {
        // Trigger animation
        animator.SetTrigger("Open");
        isOpen = true;

        Debug.Log("Chest opened!");

        // Play chest opening sound if available
        if (chestOpenSound != null && audioSource != null)
        {
            Debug.Log("Playing chest open sound");
            audioSource.PlayOneShot(chestOpenSound);
        }

        // Spawn the reward
        SpawnSword();
    }

    // Instantiates the sword prefab at the spawn point or default position
    void SpawnSword()
    {
        if (swordPrefab != null)
        {
            Vector3 spawnPosition = swordSpawnPoint != null
                ? swordSpawnPoint.position
                : transform.position + Vector3.up * 1.5f;

            Quaternion spawnRotation = swordSpawnPoint != null
                ? swordSpawnPoint.rotation
                : Quaternion.identity;

            Instantiate(swordPrefab, spawnPosition, spawnRotation);

            // Show item message on screen
            ShowItemMessage("You received: Sword");
        }
        else
        {
            Debug.LogWarning("Sword prefab not assigned.");
        }
    }

    // Displays the item message text for a few seconds
    void ShowItemMessage(string message)
    {
        if (itemMessageText != null)
        {
            itemMessageText.text = message;
            messageTimer = messageDuration;
        }
    }
}
