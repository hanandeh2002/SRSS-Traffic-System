using UnityEngine;

public class TrafficSignalSensor : MonoBehaviour
{
    public TrafficLightController trafficLight; 
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerStay(Collider other)
    {
        // إذا دخلت سيارة المنطقة
        if (other.CompareTag("Car") && trafficLight != null)
        {
            // لو الإشارة حمراء , توقف
            if (trafficLight.redLight.material.color == Color.red)
            {
                Rigidbody carRb = other.GetComponent<Rigidbody>();
                if (carRb != null)
                    carRb.velocity = Vector3.zero;
            }
        }
    }
}
