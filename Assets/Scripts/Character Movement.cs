using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ensure required components are present
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerMovementWithCollection : MonoBehaviour
{
    [Header("Movement Settings")]
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float defaultHeight = 4f;

    [Header("Footstep Audio")]
    public AudioClip footstepSound;

    [Header("Item Collection")]
    public float collectRange = 3f;          // Range within which the player can collect items
    public LayerMask collectibleLayer;       // Layer mask for collectible items
    public KeyCode collectKey = KeyCode.E;   // Key used to collect items

    // Internal variables
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;
    private AudioSource audioSource;
    private bool canMove = true;

    void Start()
    {
        // Get required components
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

        // Setup footstep audio
        audioSource.clip = footstepSound;
        audioSource.loop = true;

        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();     // Process player movement
        HandleCollection();   // Handle item collection input
    }

    void HandleMovement()
    {
        // Get forward and right directions relative to player rotation
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Handle walking/running speed input
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;

        // Preserve vertical movement (e.g., falling)
        float movementDirectionY = moveDirection.y;

        // Calculate horizontal movement
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        // Jump
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity if not grounded
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        else
        {
            characterController.height = defaultHeight; // Reset character height
            walkSpeed = 6f;  // Reset walk speed
            runSpeed = 9f;   // Reset run speed
        }

        // Apply movement
        characterController.Move(moveDirection * Time.deltaTime);

        // Handle footstep sounds
        bool isMoving = new Vector3(moveDirection.x, 0, moveDirection.z).magnitude > 0.1f;
        if (characterController.isGrounded && isMoving)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }

        // Handle camera and player rotation
        if (canMove)
        {
            // Vertical look (mouse Y)
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

            // Horizontal look (mouse X)
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    void HandleCollection()
    {
        // When the collect key is pressed
        if (Input.GetKeyDown(collectKey))
        {
            // Check for collectibles in range using OverlapSphere
            Collider[] hits = Physics.OverlapSphere(transform.position, collectRange, collectibleLayer);
            foreach (Collider hit in hits)
            {
                CollectibleItem item = hit.GetComponent<CollectibleItem>();
                if (item != null)
                {
                    item.OnCollect(); // Trigger item collection
                    break; // Collect only one item per press
                }
            }
        }
    }

    // Enables or disables movement and cursor control
    public void SetCanMove(bool state)
    {
        canMove = state;
        Cursor.lockState = state ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !state;
    }
}
