using UnityEngine;
using System;
using System.IO;
using System.Collections;

public class SpeedSensor : MonoBehaviour
{
    [Header("Speed")]
    public float currentSpeed;
    public float speedLimit = 60f;

    [Header("Violation Capture")]
    public Camera speedCam;
    public float captureCooldown = 5f;
    public float violationActiveSeconds = 4f;

    public bool violationHappened = false;
    public int totalViolations = 0;
    public string lastViolationImage = "";
    public Vector3 lastViolationPosition;

    [Header("Notification (Latest)")]
    [TextArea] public string violationMessage = "";

    [Header("Effects (Optional)")]
    public AudioSource audioSource;
    public Renderer zoneRenderer;
    private Color normalColor = Color.green;
    private Color alertColor = Color.red;

    private float lastCaptureTime = -999f;
    private Coroutine clearRoutine;

    void Start()
    {
        if (zoneRenderer != null)
            zoneRenderer.material.color = normalColor;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Car")) return;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb == null) return;

        currentSpeed = Mathf.Abs(rb.velocity.magnitude * 3.6f);

        bool over = currentSpeed > speedLimit;

        if (zoneRenderer != null)
            zoneRenderer.material.color = over ? alertColor : normalColor;

        if (!over) return;

        if (audioSource != null && !audioSource.isPlaying)
            audioSource.Play();

        if (Time.time - lastCaptureTime >= captureCooldown)
        {
            lastCaptureTime = Time.time;
            lastViolationPosition = other.transform.position;
            StartCoroutine(CaptureViolationImage(other.name));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Car")) return;

        currentSpeed = 0f;
        if (zoneRenderer != null)
            zoneRenderer.material.color = normalColor;
    }

    private IEnumerator CaptureViolationImage(string carName)
    {
        violationHappened = true;

        if (speedCam == null)
        {
            Debug.LogWarning("[SpeedSensor] speedCam is NULL - cannot capture violation image");
            yield break;
        }

        yield return new WaitForEndOfFrame();

        string imagesFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "SRSS_Dashbord",
            "images"
        );
        if (!Directory.Exists(imagesFolder))
            Directory.CreateDirectory(imagesFolder);

        int w = 256, h = 256;
        RenderTexture rt = new RenderTexture(w, h, 24);
        speedCam.targetTexture = rt;

        Texture2D tex = new Texture2D(w, h, TextureFormat.RGB24, false);
        speedCam.Render();

        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        tex.Apply();

        speedCam.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        string fileName = $"violation_{DateTime.Now:yyyyMMdd_HHmmss}.png";
        string imagePath = Path.Combine(imagesFolder, fileName);
        File.WriteAllBytes(imagePath, tex.EncodeToPNG());

        lastViolationImage = fileName;
        totalViolations++;

        violationMessage =
            $"Speed violation #{totalViolations}: Car={carName} Speed={currentSpeed:F1}km/h " +
            $"Limit={speedLimit:F0} Image={lastViolationImage} " +
            $"Loc=({lastViolationPosition.x:F1},{lastViolationPosition.z:F1})";

        Debug.Log("🚨 " + violationMessage);

        // خلّيها True فترة وبعدين False
        if (clearRoutine != null) StopCoroutine(clearRoutine);
        clearRoutine = StartCoroutine(ClearViolationFlagAfter());
    }

    private IEnumerator ClearViolationFlagAfter()
    {
        yield return new WaitForSeconds(violationActiveSeconds);
        violationHappened = false;
    }
}
