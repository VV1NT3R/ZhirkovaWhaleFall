using UnityEngine;
using System.Collections.Generic;

public class PortManager : MonoBehaviour
{
    [Header("Настройки порта")]
    public int buildCost = 20; // Цена одного объекта
    public GameObject[] portObjects; // Объекты для активации
    public float speedBonus = 5f;   // На сколько увеличивать скорость

    private int currentObjectIndex = 0;

    void Start()
    {
        // Скрываем все объекты при старте
        foreach (GameObject obj in portObjects)
        {
            if (obj != null) obj.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что вошел игрок
        if (other.CompareTag("Player"))
        {
            // Пытаемся получить контроллер лодки
            // Ищем сначала в объекте, потом в его родителях (на случай сложной иерархии)
            ArcadeBoatController boat = other.GetComponentInParent<ArcadeBoatController>();

            if (boat != null)
            {
                TryUpgradePort(boat);
            }
        }
    }

    void TryUpgradePort(ArcadeBoatController boat)
    {
        // Если еще есть что открывать
        if (currentObjectIndex < portObjects.Length)
        {
            // Проверка денег
            if (MoneyManager.Instance != null && MoneyManager.Instance.TrySpendMoney(buildCost))
            {
                // 1. Активируем постройку
                portObjects[currentObjectIndex].SetActive(true);
                currentObjectIndex++;

                // 2. УВЕЛИЧИВАЕМ СКОРОСТЬ ЛОДКИ
                boat.speed += speedBonus;
                boat.turnSpeed += 25f;

                Debug.Log($"Порт улучшен! Новая скорость лодки: {boat.speed}");
            }
            else
            {
                Debug.Log("Недостаточно монет!");
            }
        }
        else
        {
            Debug.Log("Порт полностью достроен!");
        }
    }
}