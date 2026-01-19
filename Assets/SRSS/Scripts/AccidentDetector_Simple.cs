using UnityEngine;

public class AccidentDetector_Simple : MonoBehaviour
{
    public CarPlayerController playerControl;
    public AutoCar autoCarControl;            

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Car")) return;

        Debug.Log($"⚠️ Accident between {gameObject.name} and {collision.gameObject.name}");

       
        if (playerControl != null) playerControl.enabled = false;
        if (autoCarControl != null) autoCarControl.enabled = false;
    }
}
