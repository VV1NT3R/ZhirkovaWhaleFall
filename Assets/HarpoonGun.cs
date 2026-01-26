using UnityEngine;
using UnityEngine.InputSystem;

public class HarpoonGun : MonoBehaviour
{
    [Header("Настройки выстрела")]
    public GameObject harpoonPrefab;
    public Transform shootPoint;
    public float forceMultiplier = 5f;

    [Header("Индикатор силы")]
    public float power = 1f;
    public float powerSpeed = 10f;
    private bool increasing = true;

    private GameObject currentHarpoon;

    void Update()
    {
        // 1. Счетчик силы работает, только когда гарпун в пушке
        if (currentHarpoon == null)
        {
            UpdatePowerMeter();
        }

        // 2. Выстрел на ЛКМ
        if (Mouse.current.leftButton.wasPressedThisFrame && currentHarpoon == null)
        {
            Shoot();
        }
    }

    void UpdatePowerMeter()
    {
        if (increasing)
        {
            power += Time.deltaTime * powerSpeed;
            if (power >= 10f) { power = 10f; increasing = false; }
        }
        else
        {
            power -= Time.deltaTime * powerSpeed;
            if (power <= 1f) { power = 1f; increasing = true; }
        }
    }

    void Shoot()
    {
        // Создаем гарпун и запускаем его
        currentHarpoon = Instantiate(harpoonPrefab, shootPoint.position, shootPoint.rotation);
        HarpoonProjectile projectile = currentHarpoon.GetComponent<HarpoonProjectile>();

        if (projectile != null)
        {
            projectile.Launch(power * forceMultiplier, shootPoint);
        }
    }

    void OnGUI()
    {
        GUIStyle labelStyle = new GUIStyle();
        labelStyle.fontSize = 40;
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.normal.textColor = Color.white;

        float centerX = Screen.width / 2;
        float centerY = Screen.height / 2;

        if (currentHarpoon == null)
        {
            string powerText = Mathf.Round(power).ToString();
            GUI.Label(new Rect(centerX - 50, centerY + 50, 100, 50), powerText, labelStyle);

            GUI.backgroundColor = Color.black;
            GUI.Box(new Rect(centerX - 100, centerY + 100, 200, 20), "");

            GUI.color = Color.Lerp(Color.yellow, Color.red, (power - 1) / 9);
            GUI.Box(new Rect(centerX - 100, centerY + 100, power * 20, 20), "");
            GUI.color = Color.white;
        }
    }
}