using UnityEngine;

public class PoliceCarAI : MonoBehaviour
{
    // هذه المصفوفة يمكن تعبئتها يدوياً في الـ Inspector أو عبر StartPatrol عند الـ Instantiate
    public Transform[] path;
    public float speed = 6f;
    public float rotationSpeed = 4f;
    public float stopDistance = 0.5f;

    private int currentIndex = 0;
    private bool isPatrolling = false; // متحكم ببدء الحركة فقط بعد استدعاء StartPatrol
    public enum PoliceState { Idle, Dispatched, Arrived }
    public PoliceState currentState = PoliceState.Idle;


    void Start()
    {
        // إذا تم ملء path يدوياً في Inspector، نفعل الحركة فوراً
        if (path != null && path.Length > 0)
        {
            isPatrolling = true;
            currentIndex = 0;
        }
    }

    void Update()
    {
        if (!isPatrolling) return;
        if (path == null || path.Length == 0) return;

        Transform target = path[currentIndex];

        transform.position = Vector3.MoveTowards(
    transform.position,
    target.position,
    speed * Time.deltaTime
);

        // دوران سلس
        Vector3 dir = target.position - transform.position;
        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }

        // اذا وصل للنقطة انتقل للتالية
        if (Vector3.Distance(transform.position, target.position) <= stopDistance)
        {
            currentIndex++;
            if (currentIndex >= path.Length)
            {
                isPatrolling = false;
                currentState = PoliceState.Arrived;
                Debug.Log("[PoliceCarAI] Police arrived at accident.");

            }
        }
    }

    // هذه الدالة يستدعيها AccidentDetector بعد Instantiate للـ Prefab
    public void StartPatrol(Transform[] newPath)
    {
        if (newPath == null || newPath.Length == 0)
        {
            Debug.LogWarning("[PoliceCarAI] StartPatrol called with empty path.");
            return;
        }

        path = newPath;
        currentIndex = 0;
        isPatrolling = true;

        currentState = PoliceState.Dispatched; 
    }

}
