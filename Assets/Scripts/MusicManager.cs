using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    // Static instance to implement Singleton pattern
    private static MusicManager Instance;

    // Reference to the AudioSource component that will play the music
    private AudioSource audioSource;

    // Background music AudioClip to be assigned in the Inspector
    public AudioClip backgroundMusic;

    // Reference to a UI Slider used for controlling music volume
    [SerializeField] private Slider musicSlider;

    // Called when the script instance is being loaded
    private void Awake()
    {
        // Ensure only one instance of MusicManager exists (Singleton pattern)
        if (Instance == null)
        {
            Instance = this;  // Set this as the instance
            audioSource = GetComponent<AudioSource>(); // Get the attached AudioSource component
            DontDestroyOnLoad(gameObject); // Persist this object across scene loads
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate MusicManager instances
        }
    }

    // Called before the first frame update
    void Start()
    {
        // If backgroundMusic is assigned, play it
        if (backgroundMusic != null)
        {
            PlayBackgroundMusic(false, backgroundMusic);
        }

        // If the musicSlider is assigned, add a listener to adjust volume when the slider value changes
        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(delegate { SetVolume(musicSlider.value); });
    }

    // Static method to set the volume of the music
    public static void SetVolume(float volume)
    {
        if (Instance != null && Instance.audioSource != null)
            Instance.audioSource.volume = volume; // Set the volume of the AudioSource
    }

    // Plays the background music, with optional reset and custom AudioClip
    public void PlayBackgroundMusic(bool resetSong, AudioClip audioclip = null)
    {
        if (audioclip != null)
        {
            audioSource.clip = audioclip; // Set the AudioSource to use the provided AudioClip
        }

        if (audioSource.clip != null)
        {
            if (resetSong)
            {
                audioSource.Stop(); // Stop playback if reset is requested
            }
            audioSource.Play(); // Play the AudioClip
        }
    }
}