using UnityEngine;

public class NewCamera : MonoBehaviour
{
   // Mouse sensitivity settings
   [SerializeField] private float sensX;       // Horizontal mouse sensitivity
   [SerializeField] private float sensY;       // Vertical mouse sensitivity
   
   // References to important transforms
   [SerializeField] Transform cam;             // Camera transform (for rotation)
   [SerializeField] Transform orientation;     // Player orientation reference
   
   // Mouse input values
   float mouseX;                              // Raw horizontal mouse input
   float mouseY;                              // Raw vertical mouse input
   
   float multiplier = 0.01f;                  // Converts raw mouse input to reasonable rotation values
   float xRotation;                           // Current vertical camera angle (looking up/down)
   float yRotation;                           // Current horizontal camera angle (looking left/right)
   
   private void Start()
   {
       // Lock and hide cursor for FPS camera control
       Cursor.lockState = CursorLockMode.Locked;
       Cursor.visible = false;
   }
   
   private void LateUpdate()
   {
       // Get input and apply rotations in LateUpdate (after all other updates)
       MyInput();
       
       // Apply vertical rotation to camera (looking up/down)
       cam.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
       
       // Apply only horizontal rotation to orientation (used for movement direction)
       orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
   }
   
   void MyInput()
   {
       // Get raw mouse input
       mouseX = Input.GetAxisRaw("Mouse X");
       mouseY = Input.GetAxisRaw("Mouse Y");
       
       // Apply sensitivity and multiplier to mouse movement
       yRotation += mouseX * sensX * multiplier;   // Add to yRotation (accumulate rotation)
       xRotation -= mouseY * sensY * multiplier;   // Subtract from xRotation (invert Y axis)
       
       // Clamp vertical rotation to prevent over-rotation
       xRotation = Mathf.Clamp(xRotation, -89f, 89f);
   }
}