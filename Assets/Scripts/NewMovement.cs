using UnityEngine;

[RequireComponent(typeof(PlayerStats))] // Require the PlayerStats component
public class NewMovement : MonoBehaviour
{
    // Reference to orientation transform (for camera-relative movement)
    [SerializeField] Transform orientation;
    
    [Header("Movement")]
    [SerializeField] float baseMoveSpeed = 6f;   // Base movement speed
    [SerializeField] float moveMultiplier = 10f; // Multiplier applied to movement force
    [SerializeField] float airMultiplier = .8f;  // Reduced movement control in air
    [SerializeField] float groundDrag = 6f;      // Higher drag when on ground
    [SerializeField] float airDrag = 2f;         // Lower drag when in air
    [SerializeField] float bHopWindow = 4f;      // Time window for bunny hopping
    [SerializeField] float baseJumpForce = 30f;      // Force applied when jumping

    [SerializeField] bool easyBHop = false;
    private bool jumping = false;
    private bool jumpKeyHeld = false;
    float timeOnGround = 0f;                     // Tracks time spent on ground for bunny hopping
    
    // Reference to the PlayerStats component
    private PlayerStats playerStats;
    
    // Current effective move speed (will be updated with player stats)
    private float currentMoveSpeed;
    private float currentJumpForce;
    
    [Header("keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;  // Key used for jumping
    
    [Header("Ground Detection")]
    [SerializeField] LayerMask groundMask;       // Layers that count as ground
    [SerializeField] Transform groundCheck;      // Position to check for ground
    float groundDistance = 0.4f;                 // Distance to check for ground
    
    bool isGrounded;                            // Current grounded state

    public float gravityForce = -70f;           // Custom gravity value
    
    Rigidbody rb;                               // Reference to player's rigidbody
    RaycastHit slopeHit;                        // Stores information about slope raycast
    bool onSlope;                               // Whether player is on a slope

    Vector3 moveDirection;                      // Current movement direction
    Vector3 slopeMoveDirection;                 // Direction adjusted for slopes

    // Movement input values
    float horizontalMovement;
    float verticalMovement;

    float playerHeight = 2f;                     // Height of player for ground checks

    // Checks if player is on a sloped surface
    private bool OnSlope()
    {
        // Cast ray downward to detect ground
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2f + .5f))
        {
            // If the normal isn't pointing straight up, it's a slope
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
        }
        return false;
    }

    private void Start()
    {
        // Get and configure the rigidbody
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;  // Prevent physics from rotating the player
        
        // Get reference to PlayerStats component
        playerStats = GetComponent<PlayerStats>();
        
        // Initialize current move speed with base value if PlayerStats is not found
        if (playerStats == null)
        {
            Debug.LogWarning("PlayerStats component not found! Using baseMoveSpeed instead.");
            currentMoveSpeed = baseMoveSpeed;
        }
    }

    private void Update()
    {
        // Update current movement speed from player stats
        UpdateStats();
        
        // Check if player is on ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        // Get player input and update drag
        MyInput();
        ControlDrag();

        jumpKeyHeld = Input.GetKey(jumpKey);
        // Handle jumping
        if (Input.GetKeyDown(jumpKey) && isGrounded && !jumping)
        {
            Jump();
        }

        // Project movement direction onto slope surface
        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }
    
    // Updates current move speed from PlayerStats
    void UpdateStats()
    {
        if (playerStats != null)
        {
            // Use player stats move speed 
            currentMoveSpeed = playerStats.moveSpeed;
            currentJumpForce = playerStats.jumpForce;

        }
        else
        {
            // Fallback to base speed if no PlayerStats exists
            currentMoveSpeed = baseMoveSpeed;
            currentJumpForce = baseJumpForce;
        }
    }

    // Applies jump force
    void Jump() 
    {
        // Reset vertical velocity before jumping
        jumping = true;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(transform.up * currentJumpForce, ForceMode.Impulse);
    }

    // Gets movement input
    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        // Calculate movement direction relative to orientation
        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
    }

    // Adjusts drag based on ground state
    void ControlDrag() 
    {
        if (isGrounded) {
            rb.linearDamping = groundDrag;
        } else {
            rb.linearDamping = airDrag;
        }
    }
    
    // Physics calculations happen in FixedUpdate
    private void FixedUpdate()
    {
        onSlope = OnSlope();
        if (isGrounded && jumpKeyHeld && easyBHop && !jumping) 
        {
            Jump();    
        }
        MovePlayer();
    }
    
    // Applies movement forces to the player
    void MovePlayer()
    {
        // If on ground and on a slope
        if (isGrounded && onSlope) 
        {
            // Use slope-adjusted direction
            jumping = false;
            rb.AddForce(slopeMoveDirection * currentMoveSpeed * moveMultiplier, ForceMode.Acceleration);
        }
        // If on ground but not on slope
        else if (isGrounded)
        {
            jumping = false;
            // Track time on ground for bunny hop mechanics
            timeOnGround += Time.fixedDeltaTime;
            
            // If within bunny hop window, maintain some air mobility
            if (timeOnGround < bHopWindow) {
                rb.AddForce(moveDirection * currentMoveSpeed * moveMultiplier * airMultiplier, ForceMode.Acceleration);
            } 
            else  // Normal ground movement
            {
                rb.AddForce(moveDirection * currentMoveSpeed * moveMultiplier, ForceMode.Acceleration);
            }
        }
        // If in air
        else
        {
            // Reset ground time when airborne
            timeOnGround = 0f;
            
            // Apply air movement with reduced control
            rb.AddForce(moveDirection * currentMoveSpeed * moveMultiplier * airMultiplier, ForceMode.Acceleration);
            rb.AddForce(Vector3.down + new Vector3(0, gravityForce,0), ForceMode.Acceleration);
        }
    }

    public void PushImpulse(Vector3 forceVector) 
    {
        rb.AddForce(forceVector, ForceMode.Impulse);   
    }
    public void PushAccel(Vector3 forceVector) 
    {
        rb.AddForce(forceVector, ForceMode.Acceleration);   
    }
}