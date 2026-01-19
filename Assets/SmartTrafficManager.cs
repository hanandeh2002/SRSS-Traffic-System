using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FCG;
using TMPro;

public class SmartTrafficManager : MonoBehaviour
{
    [System.Serializable]
    public class LaneData
    {
        public string laneName;
        public LaneSensor sensor;
        public TrafficLight lightScript;
        public TextMeshPro infoText;

        [HideInInspector] public int currentCarCount;
        [HideInInspector] public int originalIndex;
    }

    public List<LaneData> allLanes;

    [Header("Traffic Flow Settings")]
    public int maxCarsPerTurn = 8;       // الحد الأقصى للسيارات
    public float extraTimeAfterLastCar = 2.5f; // ✅ وقت إضافي للسيارة الأخيرة عشان ما توقف
    public float allRedSafetyTime = 3.0f; // وقت تنظيف التقاطع
    public float maxWaitTime = 20.0f;    // أقصى وقت انتظار في حال علقت السيارات

    void Start()
    {
        if (allLanes == null || allLanes.Count == 0) return;

        for (int i = 0; i < allLanes.Count; i++)
        {
            allLanes[i].originalIndex = i;
        }

        // إغلاق تام وتصفير النصوص
        ForceCloseAllLights();

        StartCoroutine(SmartCycle());
    }

    // --- دالة موحدة لتحديث كل شيء (الضوء + النص + اللون) ---
    // هذه الدالة تضمن ان المستحيل النص يختلف عن الضوء
    void SetLaneState(LaneData lane, string state, string customText = "")
    {
        // 1. تحديث الإشارة الضوئية
        if (lane.lightScript != null) lane.lightScript.SetStatus(state);

        // 2. تجهيز الألوان والنصوص
        Color targetColor = Color.white;
        string stateText = "";

        if (state == "1") // Red
        {
            targetColor = Color.red;
            stateText = "STOP";
        }
        else if (state == "2") // Yellow
        {
            targetColor = Color.yellow;
            stateText = "SLOW";
        }
        else if (state == "3") // Green
        {
            targetColor = Color.green;
            stateText = "GO";
        }

        // إذا أرسلنا نصاً خاصاً نستخدمه، وإلا نستخدم النص الافتراضي
        if (customText != "") stateText = customText;

        // 3. تحديث النص المرئي
        if (lane.infoText != null)
        {
            int count = (lane.sensor != null) ? lane.sensor.GetCarCount() : 0;
            lane.infoText.text = $"Cars: {count}\n{stateText}";
            lane.infoText.color = targetColor;
        }
    }

    // دالة تحديث الرقم فقط (بدون تغيير اللون)
    void UpdateCountOnly(LaneData lane)
    {
        if (lane.infoText != null && lane.sensor != null)
        {
            // نأخذ النص الحالي (STOP/GO) ونحدث الرقم الي فوقه بس
            string currentText = lane.infoText.text.Split('\n')[1];
            lane.infoText.text = $"Cars: {lane.sensor.GetCarCount()}\n{currentText}";
        }
    }

    void ForceCloseAllLights()
    {
        foreach (var lane in allLanes)
        {
            SetLaneState(lane, "1", "STOP");
        }
    }

    IEnumerator SmartCycle()
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            // --- 1. تحديث العدادات ---
            foreach (var lane in allLanes)
            {
                if (lane.sensor != null) lane.currentCarCount = lane.sensor.GetCarCount();
                else lane.currentCarCount = 0;
                UpdateCountOnly(lane);
            }

            // --- 2. الترتيب ---
            var sortedLanes = allLanes.OrderByDescending(x => x.currentCarCount).ToList();

            // إذا الكل فاضي، انتظر قليلاً
            if (allLanes.All(x => x.sensor.GetCarCount() == 0))
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            // --- 3. التنفيذ ---
            foreach (var activeLane in sortedLanes)
            {
                if (activeLane.currentCarCount == 0) continue;

                // 🛑 مرحلة الأمان (الكل أحمر)
                ForceCloseAllLights();

                // تغيير نص الشارع النشط فقط للاستعداد
                SetLaneState(activeLane, "1", "READY..."); // لسا أحمر بس مكتوب Ready
                activeLane.infoText.color = Color.yellow;   // لون النص أصفر للتنبيه

                yield return new WaitForSeconds(allRedSafetyTime);

                // حساب الهدف
                int carsToPass = Mathf.Min(activeLane.currentCarCount, maxCarsPerTurn);
                int targetRemaining = activeLane.currentCarCount - carsToPass;
                if (targetRemaining < 0) targetRemaining = 0;

                // 🟢 انطلاق (أخضر)
                SetLaneState(activeLane, "3", "GO!");

                float safetyTimer = 0;

                // حلقة المراقبة
                while (activeLane.sensor.GetCarCount() > targetRemaining && safetyTimer < maxWaitTime)
                {
                    safetyTimer += Time.deltaTime;
                    UpdateCountOnly(activeLane); // تحديث الرقم لحظياً
                    yield return null;
                }

                // ✅ الحل السحري للسيارة الأخيرة:
                // بعد ما نخلص العدد المطلوب، ننتظر 2.5 ثانية زيادة والاشارة خضراء
                // عشان آخر سيارة تلحق تعبر وما يسكّر بوجها
                yield return new WaitForSeconds(extraTimeAfterLastCar);

                // 🟡 تمهل (أصفر)
                SetLaneState(activeLane, "2", "SLOW");
                yield return new WaitForSeconds(3.0f);

                // 🛑 قف (أحمر) - وسيتم تأكيد الإغلاق للجميع في بداية اللفة التالية
                SetLaneState(activeLane, "1", "STOP");
            }
        }
    }
}