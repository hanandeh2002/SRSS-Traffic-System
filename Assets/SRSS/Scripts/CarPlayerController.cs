using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarPlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float turnSpeed = 50f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        float move = Input.GetAxis("Vertical");
        float turn = Input.GetAxis("Horizontal");

         
        rb.AddForce(transform.forward * -move * moveSpeed, ForceMode.Acceleration);

        
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn * turnSpeed * Time.fixedDeltaTime, 0f));
    }

    public float GetSpeedKmh()
    {
        
        return Mathf.Abs(rb.velocity.magnitude * 3.6f);
    }
}
