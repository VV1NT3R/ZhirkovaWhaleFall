using UnityEngine;

public class ArcadeCamera : MonoBehaviour
{
    [Header("“очки прив€зки")]
    public Transform lookAtTarget;
    public Vector3 offset = new Vector3(0, 5f, -10f);

    [Header("ѕлавность")]
    public float smoothTime = 0.12f;

    private Vector3 currentVelocity = Vector3.zero;

    void LateUpdate()
    {
        if (lookAtTarget == null) return;

        // 1. –ассчитываем позицию с учетом вращени€ цели вручную
        // Ёто работает стабильнее, чем TransformPoint при физических столкновени€х
        Vector3 targetPos = lookAtTarget.position + (lookAtTarget.rotation * offset);

        // 2. ѕлавное движение
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref currentVelocity,
            smoothTime
        );

        // 3. ѕлавный взгл€д на цель (чтобы камера не дергалась при повороте самой лодки)
        Vector3 lookPosition = lookAtTarget.position + Vector3.up * 1f;
        Quaternion targetRot = Quaternion.LookRotation(lookPosition - transform.position);

        // »спользуем Slerp дл€ вращени€, чтобы взгл€д камеры не "дрожал"
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
    }
}