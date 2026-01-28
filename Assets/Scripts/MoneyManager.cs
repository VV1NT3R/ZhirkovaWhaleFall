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
    public bool enableWordWrapping = false; 
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
            moneyText.enableWordWrapping = enableWordWrapping;

            moneyText.overflowMode = overflowMode;

            moneyText.alignment = TextAlignmentOptions.Right; 

            if (moneyText.rectTransform != null)
            {
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

            moneyText.text = text;

            AdjustWidthToText(text);
        }
    }

    void AdjustWidthToText(string text)
    {
        if (moneyText == null || moneyText.rectTransform == null) return;

        moneyText.ForceMeshUpdate(); 
        float preferredWidth = moneyText.preferredWidth;

        moneyText.rectTransform.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            preferredWidth + 30f
        );
    }
}