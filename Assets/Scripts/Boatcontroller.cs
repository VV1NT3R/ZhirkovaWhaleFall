using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class ArcadeBoatController : MonoBehaviour
{
    [Header("Движение")]
    public float speed = 20f;
    public float turnSpeed = 100f;
    public float acceleration = 2f;
    public float drag = 1f;

    [Header("Визуальный Крен")]
    public float maxLeanAngle = 15f;
    public float leanSmoothing = 3f;

    [Header("Вода")]
    public float waterLevel = 8.3f;
    public float hoverHeight = 0.5f;

    [Header("Настройки Звука")]
    public float soundFadeSpeed = 4f;
    [Range(0, 1)] public float maxVolume = 1f;

    private Rigidbody rb;
    private AudioSource engineAudio;
    private float verticalInput;
    private float horizontalInput;
    private float currentRotation;
    private bool isAccelerating;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        engineAudio = GetComponent<AudioSource>();

        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ |
                         RigidbodyConstraints.FreezePositionY;

        currentRotation = transform.eulerAngles.y;

        if (engineAudio != null)
        {
            engineAudio.loop = true;
            engineAudio.playOnAwake = false;
            engineAudio.volume = 0f;
        }
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.wKey.wasPressedThisFrame)
        {
            engineAudio.time = 0f; 
            if (!engineAudio.isPlaying) engineAudio.Play();
        }

        isAccelerating = kb.wKey.isPressed;

        horizontalInput = Mathf.Lerp(horizontalInput, (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f), Time.deltaTime * 5f);
        verticalInput = Mathf.Lerp(verticalInput, (isAccelerating ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f), Time.deltaTime * 5f);

        HandleEngineSound();
    }

    void HandleEngineSound()
    {
        if (engineAudio == null) return;

        float targetVolume = isAccelerating ? maxVolume : 0f;

        engineAudio.volume = Mathf.MoveTowards(engineAudio.volume, targetVolume, Time.deltaTime * soundFadeSpeed);

        if (engineAudio.volume <= 0 && !isAccelerating && engineAudio.isPlaying)
        {
            engineAudio.Stop();
        }
    }

    void FixedUpdate()
    {
        // поворот
        float movementFactor = Mathf.Clamp01(rb.linearVelocity.magnitude / 2f);
        currentRotation += horizontalInput * turnSpeed * Time.fixedDeltaTime * movementFactor;

        // движение
        Vector3 forwardForce = transform.forward * verticalInput * speed;
        rb.AddForce(forwardForce * acceleration, ForceMode.Acceleration);

        // трение
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.fixedDeltaTime * drag);

        // позиция Y
        Vector3 nextPos = rb.position;
        nextPos.y = Mathf.Lerp(nextPos.y, waterLevel + hoverHeight, Time.fixedDeltaTime * 5f);
        rb.MovePosition(nextPos);

        // вращение и крен
        float targetLean = -horizontalInput * maxLeanAngle * movementFactor;
        Quaternion targetRot = Quaternion.Euler(0, currentRotation, targetLean);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, Time.fixedDeltaTime * leanSmoothing));
    }
}