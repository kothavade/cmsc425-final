using UnityEngine;

public class NewMovement : MonoBehaviour
{
    // Reference to orientation transform (for camera-relative movement)
    [SerializeField] Transform orientation;
    
    [Header("Movement")]
    [SerializeField] float moveSpeed = 6f;       // Base movement speed
    [SerializeField] float moveMultiplier = 10f; // Multiplier applied to movement force
    [SerializeField] float airMultiplier = .8f;  // Reduced movement control in air
    [SerializeField] float groundDrag = 6f;      // Higher drag when on ground
    [SerializeField] float airDrag = 2f;         // Lower drag when in air
    [SerializeField] float bHopWindow = 4f;      // Time window for bunny hopping
    [SerializeField] float jumpForce = 15f;      // Force applied when jumping
    [SerializeField] bool easyBHop = false;
    private bool jumping = false;
    private bool jumpKeyHeld = false;
    float timeOnGround = 0f;                     // Tracks time spent on ground for bunny hopping
    
    [Header("keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;  // Key used for jumping
    

    [Header("Ground Detection")]
    [SerializeField] LayerMask groundMask;       // Layers that count as ground
    [SerializeField] Transform groundCheck;      // Position to check for ground
    float groundDistance = 0.4f;                 // Distance to check for ground
    
    bool isGrounded;                            // Current grounded state

    public float gravityForce = -.4f;           // Custom gravity value
    
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
    }

    private void Update()
    {
        // Check if player is on ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        print(isGrounded);  // Debug output
        
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

    // Applies jump force
    void Jump() 
    {
        // Reset vertical velocity before jumping
        jumping = true;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
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
            rb.AddForce(slopeMoveDirection * moveSpeed * moveMultiplier, ForceMode.Acceleration);
        }
        // If on ground but not on slope
        else if (isGrounded)
        {
            jumping = false;
            // Track time on ground for bunny hop mechanics
            timeOnGround += Time.fixedDeltaTime;
            
            // If within bunny hop window, maintain some air mobility
            if (timeOnGround < bHopWindow) {
                rb.AddForce(moveDirection * moveSpeed * moveMultiplier * airMultiplier, ForceMode.Acceleration);
            } 
            else  // Normal ground movement
            {
                rb.AddForce(moveDirection * moveSpeed * moveMultiplier, ForceMode.Acceleration);
            }
        }
        // If in air
        else
        {
            // Reset ground time when airborne
            timeOnGround = 0f;
            
            // Apply gravity to movement direction
            moveDirection += new Vector3(0, gravityForce, 0);
            
            // Apply air movement with reduced control
            rb.AddForce(moveDirection * moveSpeed * moveMultiplier * airMultiplier, ForceMode.Acceleration);
        }
    }
}