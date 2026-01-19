using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceCarAI_Smooth : MonoBehaviour
{
    public Transform[] patrolPath;
    public float speed = 10f;        // السرعة
    public float rotationSpeed = 5f; // سرعة الالتفاف
    public float reachThreshold = 1f; // المسافة اللي يعتبر فيها وصل للنقطة

    private int currentIndex = 0;

    void Update()
    {
        if (patrolPath == null || patrolPath.Length == 0)
            return;

        Transform target = patrolPath[currentIndex];

        // 🔹 الاتجاه نحو النقطة
        Vector3 direction = (target.position - transform.position).normalized;

        // 🔹 حركة للأمام
        transform.position += direction * speed * Time.deltaTime;

        // 🔹 دوران سلس
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // 🔹 التبديل للنقطة التالية
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance < reachThreshold)
        {
            currentIndex++;
            if (currentIndex >= patrolPath.Length)
                currentIndex = 0; // رجوع للبداية (اختياري)
        }
    }
}
