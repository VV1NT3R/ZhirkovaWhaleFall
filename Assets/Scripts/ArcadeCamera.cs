using UnityEngine;

public class ArcadeCamera : MonoBehaviour
{
    [Header("Точки привязки")]
    public Transform lookAtTarget;
    public Vector3 offset = new Vector3(0, 5f, -10f);

    [Header("Плавность")]
    public float smoothTime = 0.12f;

    private Vector3 currentVelocity = Vector3.zero;

    void LateUpdate()
    {
        if (lookAtTarget == null) return;

        // 1. расчет позиции с учетом вращения цели вручную
        Vector3 targetPos = lookAtTarget.position + (lookAtTarget.rotation * offset);

        // 2.плавное движение
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref currentVelocity,
            smoothTime
        );

        // 3. плавный взгляд на цель (чтобы камера не дергалась при повороте самой лодки)
        Vector3 lookPosition = lookAtTarget.position + Vector3.up * 1f;
        Quaternion targetRot = Quaternion.LookRotation(lookPosition - transform.position);

        //для вращения, чтобы взгляд камеры не дрожал
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
    }
}