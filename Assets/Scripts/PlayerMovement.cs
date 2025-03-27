using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Range(0.1f, 40f)][SerializeField] float moveSpeed = 20f;
    [Range(0f, 100f)][SerializeField] float jumpForce = 10f;
    [Range(-100f, 100f)][SerializeField] float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 PlayerMovementInput;
    private bool isGrounded;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void LateUpdate()
    {
        PlayerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        MovePlayer();

    }

    private void MovePlayer() 
    {
        
        // Get the forward and right vectors from the transform
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        
        // Project these vectors onto the horizontal plane (zero out the y component)
        forward.y = 0;
        right.y = 0;
        
        // Calculate the movement direction using the flattened vectors
        Vector3 moveVector = forward * PlayerMovementInput.z + right * PlayerMovementInput.x;
        
        isGrounded = controller.isGrounded;
        Debug.Log(isGrounded);
        if (isGrounded)
        {
            velocity.y = -1f;
        }
        else
        {
            velocity.y -= gravity * -2f * Time.deltaTime;
        }

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = jumpForce; 
        }

        controller.Move(moveVector * moveSpeed * Time.deltaTime);
        controller.Move(velocity * Time.deltaTime);
    }
}
