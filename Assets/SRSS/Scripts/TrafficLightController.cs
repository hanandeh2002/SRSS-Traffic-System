using UnityEngine;
using System.Collections;

public class TrafficLightController : MonoBehaviour
{
    public Renderer redLight;
    public Renderer yellowLight;
    public Renderer greenLight;
    public float redDuration = 5f;
    public float yellowDuration = 2f;
    public float greenDuration = 5f;
    public bool policeOverride = false;
    public enum LightState { Red, Yellow, Green }
    public LightState currentState;


    private void Start()
    {
        StartCoroutine(TrafficCycle());
    }

    private IEnumerator TrafficCycle()
    {
        while (true)
        {
            //  أحمر
            currentState = LightState.Red;
            yield return StartCoroutine(WaitWithOverride(
                () => SetLight(Color.red, Color.black, Color.black),
                redDuration
            ));

            //  أخضر
            currentState = LightState.Green;
            yield return StartCoroutine(WaitWithOverride(
                () => SetLight(Color.black, Color.black, Color.green),
                greenDuration
            ));

            //  أصفر
            currentState = LightState.Yellow;
            yield return StartCoroutine(WaitWithOverride(
                () => SetLight(Color.black, Color.yellow, Color.black),
                yellowDuration
            ));
        }
    }

    private IEnumerator WaitWithOverride(System.Action setLight, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            if (policeOverride)
            {
                //  أولوية الشرطة → أخضر دايم
                SetLight(Color.black, Color.black, Color.green);
                yield return null;
                continue;
            }

            setLight();
            timer += Time.deltaTime;
            yield return null;
        }
    }


    private void SetLight(Color red, Color yellow, Color green)
    {
        redLight.material.color = red;
        yellowLight.material.color = yellow;
        greenLight.material.color = green;
    }

}
