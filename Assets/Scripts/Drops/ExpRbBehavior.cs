using UnityEngine;

public class ExpRbBehavior : MonoBehaviour
{

    public Rigidbody rb;
    public LayerMask groundMask;
    public float groundDistance = 1f;
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
