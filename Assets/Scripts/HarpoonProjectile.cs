using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer), typeof(AudioSource))]
public class HarpoonProjectile : MonoBehaviour
{
    [Header("Настройки боя")]
    public float maxDistance = 15f;
    public float fightDuration = 10f;
    public float fishPullForce = 5f;
    public float waterSurfaceY = 8.3f;

    [Header("Физика (Баллистика по Эйлеру)")]
    public float mass = 1f;               // кг
    public float projectileRadius = 0.05f; // м
    public float dragCoefficient = 0.47f; // Коэффициент сопротивления 
    public float airDensity = 1.225f;    // Плотность воздуха 
    private float area;                  
    private Vector3 wind = Vector3.zero;  

    [Header("Коррекция модели рыбы")]
    public Vector3 fishRotationOffset = new Vector3(0, 180f, 0);

    [Header("Настройки веревки")]
    public LineRenderer lineRenderer;
    public float ropeWidth = 0.05f;

    [Header("Звуки Гарпуна")]
    public AudioClip hitFishSound;
    public AudioClip escapeSound;
    public AudioClip returnSound;
    public AudioClip fightLoopSound;

    private AudioSource audioSource;
    private AudioSource fightLoopSource;
    private Vector3 velocity;
    private Transform origin;
    private bool isReturning = false;
    private bool hasHit = false;
    private Transform caughtFish;
    private float fightTimer = 0f;
    private bool isFishExhausted = false;
    private float randomOffset;

    public void Launch(float launchForce, Transform returnPoint)
    {
        origin = returnPoint;
        // начальная скорость 
        velocity = transform.forward * launchForce;

        //площадь сечения снаряда для формулы сопротивления
        area = Mathf.PI * projectileRadius * projectileRadius;

        randomOffset = Random.Range(0f, 100f);
        audioSource = GetComponent<AudioSource>();

        if (fightLoopSound != null)
        {
            fightLoopSource = gameObject.AddComponent<AudioSource>();
            fightLoopSource.clip = fightLoopSound;
            fightLoopSource.loop = true;
            fightLoopSource.playOnAwake = false;
            fightLoopSource.volume = 0.5f;
        }

        SetupRope();
    }

    private void SetupRope()
    {
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = ropeWidth;
            lineRenderer.enabled = true;
        }
    }

    void Update()
    {
        UpdateRope();

        if (isReturning)
        {
            ReturnToBoatLogic();
            return;
        }

        if (!hasHit) SimulatePhysics();
        else if (caughtFish != null && !isFishExhausted) DoFishFight();

        // синхронизация рыбы и гарпуна
        if (caughtFish != null && !isReturning)
        {
            Vector3 targetPos = transform.position;
            if (targetPos.y < waterSurfaceY) targetPos.y = waterSurfaceY;
            transform.position = targetPos;
            caughtFish.position = targetPos;
            caughtFish.rotation = transform.rotation * Quaternion.Euler(fishRotationOffset);
        }

        if (Mouse.current.rightButton.isPressed)
        {
            if (caughtFish == null || isFishExhausted)
            {
                StartReturning();
            }
        }

        if (PauseManager.Instance != null && PauseManager.Instance.IsPaused)
            return;
    }

    void StartReturning()
    {
        if (isReturning) return;
        isReturning = true;
        hasHit = false;

        if (audioSource != null && returnSound != null)
            audioSource.PlayOneShot(returnSound);

        if (fightLoopSource != null) fightLoopSource.Stop();
    }

    //баллистика
    void SimulatePhysics()
    {
        // 1.расчет силы сопротивления воздуха
        float currentSpeed = velocity.magnitude;
        Vector3 dragForce = Vector3.zero;

        if (currentSpeed > 0.01f)
        {
            dragForce = -0.5f * airDensity * dragCoefficient * area * currentSpeed * velocity;
        }

        // 2.расчет ускорения: a = g + Fd/m
        Vector3 acceleration = Physics.gravity + (dragForce / mass);

        // 3.интеграция Эйлера,обновление скорости и позицию
        velocity += acceleration * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;

        // Поворот гарпуна по направлению полета
        if (velocity != Vector3.zero)
            transform.forward = velocity.normalized;

        if (transform.position.y < waterSurfaceY)
        {
            transform.position = new Vector3(transform.position.x, waterSurfaceY, transform.position.z);
            velocity.y = 0;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, velocity.normalized, out hit, 0.7f))
        {
            if (hit.collider.CompareTag("fish"))
                HitFish(hit.transform);
            else
            {
                hasHit = true;
                velocity = Vector3.zero;
            }
        }
    }

    void HitFish(Transform fish)
    {
        hasHit = true;
        velocity = Vector3.zero;
        caughtFish = fish;

        if (caughtFish.TryGetComponent<FishAI>(out FishAI ai)) ai.OnCaught();
        if (caughtFish.TryGetComponent<Collider>(out Collider col)) col.enabled = false;

        if (audioSource != null && hitFishSound != null) audioSource.PlayOneShot(hitFishSound);
        if (fightLoopSource != null) fightLoopSource.Play();
    }

    void DoFishFight()
    {
        float dist = Vector3.Distance(transform.position, origin.position);
        if (dist > maxDistance) { FishEscaped(); return; }

        Vector3 pullDir = (transform.position - origin.position);
        pullDir.y = 0;
        Vector3 finalDir = (pullDir.normalized + transform.right * Mathf.Sin(Time.time * 4f + randomOffset) * 0.6f).normalized;

        transform.position += finalDir * fishPullForce * Time.deltaTime;
        transform.forward = finalDir;

        fightTimer += Time.deltaTime;
        if (fightTimer >= fightDuration)
        {
            isFishExhausted = true;
            if (fightLoopSource != null) fightLoopSource.Stop();
        }
    }

    void ReturnToBoatLogic()
    {
        if (origin == null) { Destroy(gameObject); return; }

        transform.position = Vector3.MoveTowards(transform.position, origin.position, 25f * Time.deltaTime);
        transform.LookAt(origin.position);

        if (caughtFish != null)
        {
            caughtFish.position = transform.position;
            caughtFish.rotation = transform.rotation * Quaternion.Euler(fishRotationOffset);
        }

        if (Vector3.Distance(transform.position, origin.position) < 0.8f)
        {
            if (caughtFish != null)
            {
                if (MoneyManager.Instance != null) MoneyManager.Instance.AddMoney(Random.Range(3, 11));
                Destroy(caughtFish.gameObject);
            }
            Destroy(gameObject);
        }
    }

    void FishEscaped()
    {
        if (caughtFish != null)
        {
            if (caughtFish.TryGetComponent<Collider>(out Collider col)) col.enabled = true;
            caughtFish = null;
        }

        if (audioSource != null && escapeSound != null) audioSource.PlayOneShot(escapeSound);
        StartReturning();
    }

    void UpdateRope()
    {
        if (lineRenderer != null && origin != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, origin.position);
        }
    }
}