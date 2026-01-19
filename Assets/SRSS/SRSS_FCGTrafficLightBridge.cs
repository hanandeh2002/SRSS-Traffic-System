using UnityEngine;
using FCG;

public class SRSS_FCGTrafficLightBridge : MonoBehaviour
{
    [Header("Auto: if empty, it will use TrafficLight on SAME object")]
    public TrafficLight referenceLight;

    public enum LightState { Red, Yellow, Green, Pedestrians, Unknown }
    public LightState currentState = LightState.Unknown;

    void Awake()
    {
        // Auto bind
        if (referenceLight == null)
            referenceLight = GetComponent<TrafficLight>();

        if (referenceLight == null)
            Debug.LogWarning($"[Bridge] TrafficLight component not found on: {gameObject.name}");
    }

    void Update()
    {
        currentState = ReadState(referenceLight);
    }

    public string GetStateString()
    {
        return currentState.ToString();
    }

    private LightState ReadState(TrafficLight tl)
    {
        if (tl == null) return LightState.Unknown;

        // في سكربت FCG: Green/Yellow/Red/Pedestrians هم GameObjects
        if (tl.Green != null && tl.Green.activeSelf) return LightState.Green;
        if (tl.Yellow != null && tl.Yellow.activeSelf) return LightState.Yellow;
        if (tl.Red != null && tl.Red.activeSelf) return LightState.Red;

        if (tl.Pedestrians != null && tl.Pedestrians.activeSelf) return LightState.Pedestrians;

        return LightState.Unknown;
    }
}
