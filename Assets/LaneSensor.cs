using UnityEngine;
using System.Collections.Generic;

public class LaneSensor : MonoBehaviour
{
    public List<GameObject> carsInLane = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        // البحث عن السيارة بذكاء (بالتاج أو بالاسم)
        GameObject car = FindCar(other.transform);

        if (car != null)
        {
            if (!carsInLane.Contains(car))
            {
                carsInLane.Add(car);
                // جملة تأكيد ستظهر لك في الكونسول
                Debug.Log("✅ تم أخيراً صيد السيارة: " + car.name);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject car = FindCar(other.transform);

        if (car != null && carsInLane.Contains(car))
        {
            carsInLane.Remove(car);
        }
    }

    // دالة الذكاء الاصطناعي للبحث عن السيارة
    GameObject FindCar(Transform t)
    {
        // نتسلق للأعلى 10 مرات كحد أقصى (حتى لا يعلق الجهاز)
        int steps = 0;
        while (t != null && steps < 10)
        {
            // 1. هل يحمل التاج المطلوب؟
            if (t.CompareTag("IaCar")) return t.gameObject;

            // 2. (خطة بديلة) هل اسمه يدل على أنه سيارة؟
            string name = t.name.ToLower();
            if (name.Contains("car") || name.Contains("bus") || name.Contains("fury") || name.Contains("truck"))
            {
                // نتأكد أنه ليس الحساس نفسه أو الشارع
                if (!name.Contains("sensor") && !name.Contains("road"))
                    return t.gameObject;
            }

            t = t.parent; // اصعد للأب
            steps++;
        }
        return null; // لم نجد شيئاً
    }

    public int GetCarCount()
    {
        carsInLane.RemoveAll(item => item == null);
        return carsInLane.Count;
    }
}