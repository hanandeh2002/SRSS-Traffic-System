using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 2.2f, -5.5f);
    public float posSmooth = 10f;
    public float rotSmooth = 8f;

    void LateUpdate()
    {
        if (!target) return;

        // موقع الكاميرا
        Vector3 desiredPos = target.TransformPoint(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * posSmooth);

        // دوران الكاميرا (تطلع على السيارة)
        Vector3 lookPoint = target.position + Vector3.up * 1.2f;
        Quaternion desiredRot = Quaternion.LookRotation(lookPoint - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, Time.deltaTime * rotSmooth);
    }
}
