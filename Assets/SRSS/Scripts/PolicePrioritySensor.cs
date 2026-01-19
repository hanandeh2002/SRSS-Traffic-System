using UnityEngine;

public class PolicePrioritySensor : MonoBehaviour
{
    private TrafficLightController trafficLight;

    void Start()
    {
        trafficLight = GetComponentInParent<TrafficLightController>();

        if (trafficLight == null)
        {
            Debug.LogError("❌ TrafficLightController NOT found in parent!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Police")) return;

        trafficLight.policeOverride = true;
        Debug.Log("🚓 Police priority ON");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Police")) return;

        trafficLight.policeOverride = false;
        Debug.Log("🚦 Police priority OFF");
    }
}
