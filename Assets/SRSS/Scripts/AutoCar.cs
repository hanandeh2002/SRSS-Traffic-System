using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AutoCar : MonoBehaviour
{
    public float speed = 8f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        rb.AddForce(transform.forward * speed, ForceMode.Acceleration);
    }
}
