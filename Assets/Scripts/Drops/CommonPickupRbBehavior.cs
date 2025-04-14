using UnityEngine;

public class CommonPickupRbBehavior : MonoBehaviour
{

    public Rigidbody rb;
    public LayerMask groundMask;
    public float groundDistance = 1f;

    // Should probably also add a timer to this, disable rb after 1 second or something
    // also possibly more efficient option of removing or disabling component instead of just making kinematic
    void OnTriggerStay(Collider other)
    {
        Debug.Log("Exp collider triggered");
        if (Physics.CheckSphere(transform.position, groundDistance, groundMask))
        {
            // Debug.Log(rb.linearVelocity.y);
            if ((rb.linearVelocity.y <= .1f) && (rb.linearVelocity.y >= -.1f))
            {
                Debug.Log("setting exp rb to kinematic");
                rb.isKinematic = true;
                enabled = false;
            }
        }
    }
}
