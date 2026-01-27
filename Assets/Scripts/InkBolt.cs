using System.Collections.Generic;
using UnityEngine;

public class InkBolt : MonoBehaviour
{
    public GameObject puddlePrefab; // Сюда перетащи префаб лужи
    private ParticleSystem part;
    private List<ParticleCollisionEvent> collisionEvents;

    void Start()
    {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {
        // Получаем все точки столкновения частиц
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < numCollisionEvents; i++)
        {
            Vector3 pos = collisionEvents[i].intersection; // Точка удара
            Vector3 normal = collisionEvents[i].normal;    // Направление поверхности

            // Создаем лужу
            GameObject puddle = Instantiate(puddlePrefab, pos + normal * 0.01f, Quaternion.LookRotation(normal));

            // Немного рандомим размер и поворот для естественности
            puddle.transform.Rotate(Vector3.up, Random.Range(0, 360), Space.Self);
            puddle.transform.localScale *= Random.Range(0.8f, 1.2f);

            // Опционально: удаляем лужу через 10 секунд, чтобы не перегружать память
            Destroy(puddle, 10f);
        }
    }
}