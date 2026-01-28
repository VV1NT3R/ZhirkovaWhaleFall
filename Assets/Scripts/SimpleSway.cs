using UnityEngine;

public class RotationSway : MonoBehaviour
{
    [Header("Настройки покачивания")]
    [SerializeField] private float angle = 5f;      
    [SerializeField] private float speed = 1f;    

    [Header("Оси вращения")]
    [SerializeField] private bool swayX = true;    
    [SerializeField] private bool swayZ = true;    

    [Header("Случайная фаза")]
    [SerializeField] private bool randomizePhase = true; 

    private Vector3 startRotation;
    private float phaseOffsetX = 0f;
    private float phaseOffsetZ = 0f;

    void Start()
    {

        startRotation = transform.localEulerAngles;

        if (randomizePhase)
        {
            phaseOffsetX = Random.Range(0f, Mathf.PI * 2f);
            phaseOffsetZ = Random.Range(0f, Mathf.PI * 2f);
        }
    }

    void Update()
    {
        float time = Time.time * speed;

        // новый вектор поворота
        Vector3 newRotation = startRotation;

        if (swayX)
            newRotation.x += Mathf.Sin(time + phaseOffsetX) * angle;

        if (swayZ)
            newRotation.z += Mathf.Cos(time + phaseOffsetZ) * angle * 0.7f;

        transform.localEulerAngles = newRotation;
    }
}