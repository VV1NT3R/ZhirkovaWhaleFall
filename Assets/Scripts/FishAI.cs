using UnityEngine;

public class FishAI : MonoBehaviour
{
    [Header("Настройки движения")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 2f;
    public float roamRadius = 10f;
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;

    [Header("Коррекция модели")]
    public float rotationYOffset = 180f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isWaiting = false;
    private float waitTimer;
    private bool isCaught = false;

    void Start()
    {
        startPosition = transform.position;
        SetNewRandomTarget();
    }

    void Update()
    {
        if (isCaught) return;

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                isWaiting = false;
                SetNewRandomTarget();
            }
        }
        else
        {
            MoveTowardsTarget();
        }
    }

    void MoveTowardsTarget()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);

            targetRot *= Quaternion.Euler(0, rotationYOffset, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.2f)
        {
            isWaiting = true;
            waitTimer = Random.Range(minWaitTime, maxWaitTime);
        }
    }

    void SetNewRandomTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle * roamRadius;
        targetPosition = startPosition + new Vector3(randomCircle.x, 0, randomCircle.y);
    }

    public void OnCaught()
    {
        isCaught = true;
    }
}