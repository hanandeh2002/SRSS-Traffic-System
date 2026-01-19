using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class AccidentDetector : MonoBehaviour
{
    public Camera accidentCam;

    [Header("UI")]
    public RawImage accidentPreview;

    [Header("Incident Settings")]
    public float cooldownSeconds = 5f;
    public float incidentActiveSeconds = 6f;
    public float stopAutoCarSeconds = 2f;

    [Header("Cars Controllers (Optional)")]
    public AutoCar autoCarControl; // اختياري

    [Header("States")]
    public bool accidentHappened = false;
    public string lastAccidentImage = "";
    public Vector3 lastAccidentPosition;

    // بدل Dispatched
    public bool policeNotified = false;
    public bool ambulanceNotified = false;

    [Header("Notifications (Latest)")]
    [TextArea] public string policeMessage = "";
    [TextArea] public string ambulanceMessage = "";
    [TextArea] public string driversMessage = "";

    [Header("Counters")]
    public int totalAccidents = 0;

    private bool busy = false;
    private float lastAccidentTime = -999f;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Car")) return;

        if (busy) return;
        if (Time.time - lastAccidentTime < cooldownSeconds) return;

        lastAccidentTime = Time.time;

        // حادث جديد
        totalAccidents++;
        accidentHappened = true;

        policeNotified = false;
        ambulanceNotified = false;

        policeMessage = "";
        ambulanceMessage = "";
        driversMessage = "";

        lastAccidentPosition = transform.position;

        Debug.Log($"⚠️ Accident #{totalAccidents} between {gameObject.name} and {collision.gameObject.name}");

        if (autoCarControl != null)
            StartCoroutine(StopAutoCarTemporarily());

        StartCoroutine(HandleIncident());
    }

    private IEnumerator HandleIncident()
    {
        busy = true;

        yield return StartCoroutine(CaptureAccidentImage());

        // إشعارات بدل سيارات
        NotifyPolice();
        NotifyAmbulance();

        // إشعار للسائقين (جاهز للازدحام لاحقاً)
        NotifyDrivers_AccidentArea();

        yield return new WaitForSeconds(incidentActiveSeconds);

        accidentHappened = false;
        busy = false;
    }

    private IEnumerator StopAutoCarTemporarily()
    {
        autoCarControl.enabled = false;
        yield return new WaitForSeconds(stopAutoCarSeconds);
        autoCarControl.enabled = true;
    }

    private IEnumerator CaptureAccidentImage()
    {
        yield return new WaitForEndOfFrame();
        if (accidentCam == null) yield break;

        string imagesFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "SRSS_Dashbord",
            "images"
        );
        if (!Directory.Exists(imagesFolder))
            Directory.CreateDirectory(imagesFolder);

        int w = 256, h = 256;
        RenderTexture rt = new RenderTexture(w, h, 24);
        accidentCam.targetTexture = rt;

        Texture2D tex = new Texture2D(w, h, TextureFormat.RGB24, false);
        accidentCam.Render();

        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        tex.Apply();

        accidentCam.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        if (accidentPreview != null)
            accidentPreview.texture = tex;

        string fileName = $"accident_{DateTime.Now:yyyyMMdd_HHmmss}.png";
        string imagePath = Path.Combine(imagesFolder, fileName);

        File.WriteAllBytes(imagePath, tex.EncodeToPNG());
        lastAccidentImage = fileName;

        Debug.Log("📸 Accident image saved: " + imagePath);
    }

    private void NotifyPolice()
    {
        policeNotified = true;

        // رسالة مفهومة الداش
        policeMessage =
            $"Police notified: Accident #{totalAccidents} at " +
            $"({lastAccidentPosition.x:F1},{lastAccidentPosition.y:F1},{lastAccidentPosition.z:F1}) " +
            $"Image={lastAccidentImage}";

        Debug.Log("🚓 " + policeMessage);
    }

    private void NotifyAmbulance()
    {
        ambulanceNotified = true;

        ambulanceMessage =
            $"Ambulance notified: Accident #{totalAccidents} at " +
            $"({lastAccidentPosition.x:F1},{lastAccidentPosition.y:F1},{lastAccidentPosition.z:F1}) " +
            $"Image={lastAccidentImage}";

        Debug.Log("🚑 " + ambulanceMessage);
    }

    private void NotifyDrivers_AccidentArea()
    {
        // هاي مبدئياً للحادث، وبالازدحام رح نبدّلها برسالة ازدحام
        driversMessage =
            $"Drivers alert: Accident reported near intersection. Avoid the area. " +
            $"Location=({lastAccidentPosition.x:F1},{lastAccidentPosition.z:F1})";

        Debug.Log("📢 " + driversMessage);
    }
}
