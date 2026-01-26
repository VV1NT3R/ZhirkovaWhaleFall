using UnityEngine;
using System.Collections.Generic;

public class PortManager : MonoBehaviour
{
    [Header("Настройки порта")]
    public int buildCost = 20; // Цена одного объекта
    public GameObject[] portObjects; // Положи сюда 3 объекта в инспекторе

    private int currentObjectIndex = 0;

    void Start()
    {
        // В начале игры скрываем все объекты
        foreach (GameObject obj in portObjects)
        {
            if (obj != null) obj.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что в триггер вошла лодка (у нее должен быть тег "Player" или другой твой тег)
        if (other.CompareTag("Player"))
        {
            TryUpgradePort();
        }
    }

    void TryUpgradePort()
    {
        // Если еще есть что открывать
        if (currentObjectIndex < portObjects.Length)
        {
            // Пытаемся списать деньги через MoneyManager
            if (MoneyManager.Instance != null && MoneyManager.Instance.TrySpendMoney(buildCost))
            {
                // Показываем текущий объект по очереди
                portObjects[currentObjectIndex].SetActive(true);
                currentObjectIndex++;

                Debug.Log("Порт улучшен! Открыт объект номер: " + currentObjectIndex);
            }
            else
            {
                Debug.Log("Недостаточно монет для улучшения порта!");
            }
        }
        else
        {
            Debug.Log("Порт полностью достроен!");
        }
    }
}