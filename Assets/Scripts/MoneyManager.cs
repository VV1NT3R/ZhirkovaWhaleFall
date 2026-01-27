using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;
    public int currentMoney = 0;

    [Header("UI Reference")]
    public TMP_Text moneyText;

    [Header("Настройки")]
    public string prefix = "На счету: ";
    public bool showSymbol = true;

    [Header("Настройки текста")]
    public bool enableWordWrapping = false; // Отключаем перенос
    public TextOverflowModes overflowMode = TextOverflowModes.Overflow;

    [Header("Звуки")]
    public AudioClip collectSound;
    public AudioClip spendSound;
    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        SetupTextMeshPro();
        UpdateMoneyUI();
    }

    void SetupTextMeshPro()
    {
        if (moneyText != null)
        {
            // Отключаем перенос текста
            moneyText.enableWordWrapping = enableWordWrapping;

            // Настраиваем режим переполнения
            moneyText.overflowMode = overflowMode;

            // Устанавливаем выравнивание
            moneyText.alignment = TextAlignmentOptions.Right; // Или Left, Center

            // Минимальная ширина для текста
            if (moneyText.rectTransform != null)
            {
                // Автоматически подбираем ширину
                moneyText.rectTransform.SetSizeWithCurrentAnchors(
                    RectTransform.Axis.Horizontal,
                    Mathf.Max(300f, moneyText.preferredWidth + 50f)
                );
            }
        }
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateMoneyUI();

        if (audioSource != null && collectSound != null)
            audioSource.PlayOneShot(collectSound);
    }

    public bool TrySpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            UpdateMoneyUI();

            if (audioSource != null && spendSound != null)
                audioSource.PlayOneShot(spendSound);
            return true;
        }
        return false;
    }

    void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            string symbol = showSymbol ? "₽" : "";
            string text = $"{prefix}{currentMoney}{symbol}";

            // Применяем текст
            moneyText.text = text;

            // Автоматически подстраиваем ширину под текст
            AdjustWidthToText(text);
        }
    }

    void AdjustWidthToText(string text)
    {
        if (moneyText == null || moneyText.rectTransform == null) return;

        // Вычисляем необходимую ширину
        moneyText.ForceMeshUpdate(); // Обновляем меш, чтобы получить актуальные размеры
        float preferredWidth = moneyText.preferredWidth;

        // Устанавливаем ширину с небольшим запасом
        moneyText.rectTransform.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            preferredWidth + 30f
        );
    }

    // Контекстное меню для тестов
    [ContextMenu("Тест длинного текста")]
    void TestLongText()
    {
        if (moneyText != null)
        {
            moneyText.text = "На счету: 999999999999₽";
            AdjustWidthToText(moneyText.text);
        }
    }
}