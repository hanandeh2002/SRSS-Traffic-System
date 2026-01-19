using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class SystemData
{
    public float speed;

    // حالات الإشارات الأربعة
    public string lightN;
    public string lightS;
    public string lightE;
    public string lightW;

    // ازدحام (جاهز للمستقبل)
    public int carsN;
    public int carsS;
    public int carsE;
    public int carsW;

    // حادث
    public bool accident;
    public string accidentImage;
    public int totalAccidents;

    // إشعارات الحادث
    public string policeStatus;
    public string ambulanceStatus;
    public string policeMessage;
    public string ambulanceMessage;
    public string driversMessage;

    // مخالفة سرعة
    public bool violation;
    public string violationImage;
    public int totalViolations;
    public string violationMessage;

    public string lastUpdate;
}

[System.Serializable]
public class SystemDataContainer
{
    public List<SystemData> records = new List<SystemData>();
}

public class SystemDataWriter : MonoBehaviour
{
    public float writeInterval = 3f;

    [Header("Data Sources")]
    public SpeedSensor speedSensor;

    [Header("Traffic Lights (FCG Bridge)")]
    public SRSS_FCGTrafficLightBridge bridgeN;
    public SRSS_FCGTrafficLightBridge bridgeS;
    public SRSS_FCGTrafficLightBridge bridgeE;
    public SRSS_FCGTrafficLightBridge bridgeW;

    [Header("Lane Sensors (Optional - for congestion later)")]
    public LaneSensor laneN;
    public LaneSensor laneS;
    public LaneSensor laneE;
    public LaneSensor laneW;

    [Header("Incident")]
    public AccidentDetector accidentDetector;

    private string filePath;
    private SystemDataContainer container = new SystemDataContainer();

    void Start()
    {
        filePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "SRSS_Dashbord",
            "data.json"
        );

        InvokeRepeating(nameof(WriteData), 1f, writeInterval);
    }

    void WriteData()
    {
        LoadOldFileIfExists();

        SystemData data = new SystemData
        {
            // السرعة
            speed = speedSensor != null ? speedSensor.currentSpeed : 0f,

            // الإشارات
            lightN = GetLightState(bridgeN),
            lightS = GetLightState(bridgeS),
            lightE = GetLightState(bridgeE),
            lightW = GetLightState(bridgeW),

            // ازمه
            carsN = GetCarsCount(laneN),
            carsS = GetCarsCount(laneS),
            carsE = GetCarsCount(laneE),
            carsW = GetCarsCount(laneW),

            // حادث
            accident = accidentDetector != null && accidentDetector.accidentHappened,
            accidentImage = GetLastAccidentImage(),
            totalAccidents = GetTotalAccidents(),

            // إشعارات + حالات
            policeStatus = GetPoliceStatus(),
            ambulanceStatus = GetAmbulanceStatus(),
            policeMessage = GetPoliceMessage(),
            ambulanceMessage = GetAmbulanceMessage(),
            driversMessage = GetDriversMessage(),

            // مخالفة سرعة
            violation = speedSensor != null && speedSensor.violationHappened,
            violationImage = GetLastViolationImage(),
            totalViolations = GetTotalViolations(),
            violationMessage = GetViolationMessage(),

            lastUpdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        container.records.Add(data);

        // نحتفظ بآخر 5 فقط
        if (container.records.Count > 5)
            container.records.RemoveAt(0);

        string json = JsonUtility.ToJson(container, true);
        File.WriteAllText(filePath, json);

        Debug.Log("📄 data.json updated (4 lights + accident + violation + totals + messages)");
    }

    void LoadOldFileIfExists()
    {
        if (!File.Exists(filePath)) return;

        string oldJson = File.ReadAllText(filePath);
        if (string.IsNullOrEmpty(oldJson)) return;

        try
        {
            container = JsonUtility.FromJson<SystemDataContainer>(oldJson);
            if (container == null || container.records == null)
                container = new SystemDataContainer();
        }
        catch
        {
            container = new SystemDataContainer();
        }
    }

    string GetLightState(SRSS_FCGTrafficLightBridge b)
    {
        if (b == null) return "Unknown";
        return b.GetStateString();
    }

    int GetCarsCount(LaneSensor s)
    {
        if (s == null) return 0;
        return s.GetCarCount();
    }

    //    Notified    بدل  Dispatched 
    string GetPoliceStatus()
    {
        if (accidentDetector == null) return "Idle";
        return accidentDetector.policeNotified ? "Notified" : "Idle";
    }

    string GetAmbulanceStatus()
    {
        if (accidentDetector == null) return "Idle";
        return accidentDetector.ambulanceNotified ? "Notified" : "Idle";
    }

    string GetPoliceMessage()
    {
        if (accidentDetector == null) return "";
        return accidentDetector.policeMessage ?? "";
    }

    string GetAmbulanceMessage()
    {
        if (accidentDetector == null) return "";
        return accidentDetector.ambulanceMessage ?? "";
    }

    string GetDriversMessage()
    {
        if (accidentDetector == null) return "";
        return accidentDetector.driversMessage ?? "";
    }

    int GetTotalAccidents()
    {
        if (accidentDetector == null) return 0;
        return accidentDetector.totalAccidents;
    }

    string GetLastAccidentImage()
    {
        if (accidentDetector == null) return "";
        if (string.IsNullOrEmpty(accidentDetector.lastAccidentImage)) return "";
        return accidentDetector.lastAccidentImage;
    }

    // ✅ مخالفات السرعة
    int GetTotalViolations()
    {
        if (speedSensor == null) return 0;
        return speedSensor.totalViolations;
    }

    string GetLastViolationImage()
    {
        if (speedSensor == null) return "";
        return speedSensor.lastViolationImage ?? "";
    }

    string GetViolationMessage()
    {
        if (speedSensor == null) return "";
        return speedSensor.violationMessage ?? "";
    }
}
