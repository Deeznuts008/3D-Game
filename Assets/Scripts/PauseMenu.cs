using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // Reference to the pause menu UI GameObject (assign in Inspector)
    [SerializeField] private GameObject pauseMenu;

    // Tracks whether the game is currently paused
    private bool isPaused = false;

    // Reference to the player movement script to enable/disable movement when paused
    private PlayerMovementWithCollection playerMovement;

    void Start()
    {
        // Disable pause menu UI at start
        pauseMenu.SetActive(false);

        // Find the player movement script in the scene
        playerMovement = FindObjectOfType<PlayerMovementWithCollection>();

        // Warn if playerMovement script was not found (to avoid null refs)
        if (playerMovement == null)
        {
            Debug.LogWarning("PlayerMovementWithCollection script not found in scene.");
        }
    }

    void Update()
    {
        // Listen for Escape key press to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume(); // If paused, resume the game
            else
                Pause();  // If not paused, pause the game
        }
    }

    // Resumes gameplay by hiding pause menu and restoring time scale
    public void Resume()
    {
        pauseMenu.SetActive(false);    // Hide pause UI
        Time.timeScale = 1f;           // Resume normal game time

        // Enable player movement if script reference exists
        if (playerMovement != null)
            playerMovement.SetCanMove(true);

        isPaused = false;              // Update paused state

        // Lock and hide the cursor for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Pauses gameplay by showing pause menu and freezing time
    public void Pause()
    {
        pauseMenu.SetActive(true);     // Show pause UI
        Time.timeScale = 0f;           // Freeze game time

        // Disable player movement if script reference exists
        if (playerMovement != null)
            playerMovement.SetCanMove(false);

        isPaused = true;               // Update paused state

        // Unlock and show the cursor so player can interact with UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Loads the MainMenu scene and resumes time scale
    public void Home()
    {
        Time.timeScale = 1f;                   // Resume time before scene change
        SceneManager.LoadScene("MainMenu");   // Load scene named "MainMenu"
    }

    // Loads the previous scene based on build index and resumes time scale
    public void Quit()
    {
        Time.timeScale = 1f;                                       // Resume time before scene change
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1); // Load previous scene in build order
    }
}
