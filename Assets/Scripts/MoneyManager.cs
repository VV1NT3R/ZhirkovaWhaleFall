using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;
    public int currentMoney = 0;

    [Header("Визуал UI")]
    public Font customFont; // Сюда перетащи свой шрифт в Инспекторе
    public Color uiColor = new Color(0f, 0.7f, 1f); // Голубой цвет

    [Header("Звуки")]
    public AudioClip collectSound;
    public AudioClip spendSound;
    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null) Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        if (audioSource != null && collectSound != null)
            audioSource.PlayOneShot(collectSound);
    }

    public bool TrySpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            if (audioSource != null && spendSound != null)
                audioSource.PlayOneShot(spendSound);
            return true;
        }
        return false;
    }

    void OnGUI()
    {
        GUIStyle moneyStyle = new GUIStyle();

        // Настройка шрифта
        if (customFont != null) moneyStyle.font = customFont;

        moneyStyle.fontSize = 40;
        moneyStyle.alignment = TextAnchor.UpperRight;

        float x = Screen.width - 350;
        float y = 20;
        float width = 230;
        float height = 60;
        string text = "$ " + currentMoney;

        // 1. РИСУЕМ ОБВОДКУ (Рамку вокруг букв)
        moneyStyle.normal.textColor = Color.white; // Цвет обводки
        int outLineSize = 2; // Толщина обводки

        // Рисуем текст со смещением во все стороны
        GUI.Label(new Rect(x - outLineSize, y, width, height), text, moneyStyle);
        GUI.Label(new Rect(x + outLineSize, y, width, height), text, moneyStyle);
        GUI.Label(new Rect(x, y - outLineSize, width, height), text, moneyStyle);
        GUI.Label(new Rect(x, y + outLineSize, width, height), text, moneyStyle);

        // 2. РИСУЕМ ОСНОВНОЙ ТЕКСТ
        moneyStyle.normal.textColor = uiColor; // Твой голубой цвет
        GUI.Label(new Rect(x, y, width, height), text, moneyStyle);
    }
}