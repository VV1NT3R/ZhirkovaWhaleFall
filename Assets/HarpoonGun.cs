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
        if (currentHarpoon == null)
        {
            UpdatePowerMeter();
        }

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
        currentHarpoon = Instantiate(harpoonPrefab, shootPoint.position, shootPoint.rotation);
        HarpoonProjectile projectile = currentHarpoon.GetComponent<HarpoonProjectile>();

        if (projectile != null)
        {
            projectile.Launch(power * forceMultiplier, shootPoint);
        }
    }

    void OnGUI()
    {
        if (currentHarpoon == null)
        {
            float centerX = Screen.width / 2;
            float centerY = Screen.height / 2;
            float verticalOffset = 200f;

            // 1. Создаем стиль для закругленной рамки
            GUIStyle frameStyle = new GUIStyle(GUI.skin.box);
            frameStyle.normal.background = Texture2D.whiteTexture; // Используем белую текстуру

            // 2. Рисуем белую рамку (просто пустой прямоугольник)
            GUI.color = Color.white;
            // Рисуем 4 линии или одну рамку через Box (внешние границы)
            GUI.Box(new Rect(centerX - 102, centerY + verticalOffset - 2, 204, 24), "");

            // 3. Рисуем прозрачный фон внутри рамки (опционально, сейчас он пустой)
            GUI.color = new Color(0, 0, 0, 0); // Полностью прозрачный
            GUI.Box(new Rect(centerX - 100, centerY + verticalOffset, 200, 20), "");

            // 4. Настраиваем цвета для полоски
            Color cyanBlue = new Color(0f, 0.7f, 1f, 0.7f); // 70% прозрачности (0.7f в конце)
            Color deepBlue = new Color(0f, 0.2f, 0.8f, 0.7f); // 70% прозрачности

            GUI.color = Color.Lerp(cyanBlue, deepBlue, (power - 1) / 9f);

            // 5. Рисуем закругленную полоску силы
            // Чтобы углы были закругленными, используем стандартный GUI.skin.box
            GUI.Box(new Rect(centerX - 100, centerY + verticalOffset, power * 20, 20), "");

            // Сброс цвета
            GUI.color = Color.white;
        }
    }
}