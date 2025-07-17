using UnityEngine;
using TMPro;
using UnityEngine.UI;  // Needed for the Image component

// This ensures the GameObject has an AudioSource component
[RequireComponent(typeof(AudioSource))]
public class NPCInteractionRaycast : MonoBehaviour
{
    public Camera playerCamera;                        // Reference to the player's camera
    public float maxInteractDistance = 4f;             // Max distance to interact with NPCs

    [Header("UI Elements")]
    public GameObject dialoguePanel;                   // The dialogue panel UI GameObject
    public Image dialoguePanelBackground;              // Background image of the panel
    public TextMeshProUGUI dialogueText;               // The TMP text component displaying the message
    public string npcMessage = "Hello there, traveler!"; // The message shown by the NPC
    public float messageDuration = 3f;                 // Duration for which the message stays on screen

    [Header("Audio")]
    public AudioClip npcVoiceClip;                     // Audio clip that plays when the NPC talks

    private float messageTimer = 0f;                   // Internal timer for hiding the message
    private bool isMessageVisible = false;             // Whether the message is currently showing
    private AudioSource audioSource;                   // AudioSource used to play the NPC voice

    void Start()
    {
        // Assign the main camera if not already assigned
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
                Debug.LogError("No playerCamera assigned and no MainCamera found.");
        }

        // Hide the dialogue panel at the start
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        else
            Debug.LogError("Dialogue panel is not assigned!");

        // Warn if text is not set
        if (dialogueText == null)
            Debug.LogError("Dialogue Text is not assigned!");

        // Set the dialogue panel background color (optional)
        if (dialoguePanelBackground != null)
        {
            dialoguePanelBackground.color = Color.white;
        }
        else
        {
            Debug.LogWarning("Dialogue panel background Image component not assigned!");
        }

        // Setup audio source (required by RequireComponent)
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;         // Play once
        audioSource.playOnAwake = false;  // Don't auto-play
    }

    void Update()
    {
        // On E key press, check for NPC interaction via raycast
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxInteractDistance))
            {
                // Check if the object hit belongs to this NPC
                if (hit.collider.GetComponentInParent<NPCInteractionRaycast>() == this)
                {
                    ShowDialogue();
                }
            }
        }

        // If a message is visible, count down and hide when time runs out
        if (isMessageVisible)
        {
            messageTimer -= Time.deltaTime;

            if (messageTimer <= 0f)
            {
                HideDialogue();
            }
        }
    }

    // Displays the NPC's message and optionally plays audio
    void ShowDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (dialogueText != null)
            dialogueText.text = npcMessage;

        messageTimer = messageDuration;
        isMessageVisible = true;

        // Play the voice clip once
        if (npcVoiceClip != null && audioSource != null)
        {
            audioSource.clip = npcVoiceClip;
            audioSource.Play();
        }
    }

    // Hides the message and stops any playing audio
    void HideDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (dialogueText != null)
            dialogueText.text = "";

        // Stop any currently playing audio
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        isMessageVisible = false;
    }
}
