using UnityEngine;

public class PortManager : MonoBehaviour
{
    [Header("Настройки порта")]
    public int buildCost = 20;
    public GameObject[] portObjects;
    public float speedBonus = 5f;

    private int currentObjectIndex = 0;

    void Start()
    {
        foreach (GameObject obj in portObjects)
        {
            if (obj != null) obj.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        ArcadeBoatController boat = other.GetComponentInParent<ArcadeBoatController>();
        if (boat == null) return;

        TryUpgradePort(boat);
    }

    void TryUpgradePort(ArcadeBoatController boat)
    {
        if (MoneyManager.Instance == null) return;

        int money = MoneyManager.Instance.currentMoney;

        // Сколько зданий можем купить по деньгам
        int canBuildByMoney = money / buildCost;

        // Сколько зданий осталось построить
        int remainingBuildings = portObjects.Length - currentObjectIndex;

        // Итоговое количество построек
        int buildingsToBuild = Mathf.Min(canBuildByMoney, remainingBuildings);

        if (buildingsToBuild <= 0)
        {
            Debug.Log("Недостаточно монет!");
            return;
        }

        // Списываем деньги ОДИН РАЗ
        int totalCost = buildingsToBuild * buildCost;
        MoneyManager.Instance.TrySpendMoney(totalCost);

        // Строим нужное количество зданий
        for (int i = 0; i < buildingsToBuild; i++)
        {
            portObjects[currentObjectIndex].SetActive(true);
            currentObjectIndex++;

            boat.speed += speedBonus;
            boat.turnSpeed += 25f;
        }

        Debug.Log($"Построено зданий: {buildingsToBuild}, потрачено: {totalCost}");
    }
}
