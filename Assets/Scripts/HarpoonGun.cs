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
            float verticalOffset = 250f;

            // 1. Рисуем белую рамку (скроется под фоном)
            GUI.color = Color.white;
            GUI.Box(new Rect(centerX - 102, centerY + verticalOffset - 2, 204, 24), "");

            // 2. Рисуем цветную рамку поверх
            GUI.color = new Color(0f, 0.5f, 0.7f, 0.9f);
            GUI.Box(new Rect(centerX - 102, centerY + verticalOffset - 2, 204, 24), "");

            // 3. Фон индикатора
            GUI.color = new Color(0.1f, 0.3f, 0.6f, 0.8f);
            GUI.Box(new Rect(centerX - 100, centerY + verticalOffset, 200, 20), "");

            // 4. Полоска силы
            Color startColor = new Color(0f, 0.8f, 1f, 0.9f);
            Color endColor = new Color(0f, 0.4f, 0.8f, 0.9f);
            GUI.color = Color.Lerp(startColor, endColor, (power - 1) / 9f);

            // Используем стиль без фона для полоски
            GUIStyle fillStyle = new GUIStyle();
            Texture2D fillTex = new Texture2D(1, 1);
            fillTex.SetPixel(0, 0, GUI.color);
            fillTex.Apply();
            fillStyle.normal.background = fillTex;

            GUI.Box(new Rect(centerX - 100, centerY + verticalOffset, power * 20, 20), "", fillStyle);

            GUI.color = Color.white;
        }
    }
}