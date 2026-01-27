using UnityEngine;

public class RotationSway : MonoBehaviour
{
    [Header("Настройки покачивания")]
    [SerializeField] private float angle = 5f;      // Угол покачивания в градусах
    [SerializeField] private float speed = 1f;     // Скорость покачивания

    [Header("Оси вращения")]
    [SerializeField] private bool swayX = true;    // Покачивание вокруг оси X
    [SerializeField] private bool swayZ = true;    // Покачивание вокруг оси Z

    [Header("Случайная фаза")]
    [SerializeField] private bool randomizePhase = true; // Начать со случайного смещения

    private Vector3 startRotation;
    private float phaseOffsetX = 0f;
    private float phaseOffsetZ = 0f;

    void Start()
    {
        // Сохраняем начальный поворот
        startRotation = transform.localEulerAngles;

        // Добавляем случайную фазу для более естественного вида
        if (randomizePhase)
        {
            phaseOffsetX = Random.Range(0f, Mathf.PI * 2f);
            phaseOffsetZ = Random.Range(0f, Mathf.PI * 2f);
        }
    }

    void Update()
    {
        // Вычисляем текущий угол поворота
        float time = Time.time * speed;

        // Создаем новый вектор поворота
        Vector3 newRotation = startRotation;

        // Применяем покачивание к выбранным осям
        if (swayX)
            newRotation.x += Mathf.Sin(time + phaseOffsetX) * angle;

        if (swayZ)
            newRotation.z += Mathf.Cos(time + phaseOffsetZ) * angle * 0.7f;

        // Обновляем поворот объекта
        transform.localEulerAngles = newRotation;
    }
}