using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;
    public int currentMoney = 0;

    [Header("Звуки")]
    public AudioClip collectSound; // Звук монет
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
        {
            audioSource.PlayOneShot(collectSound);
        }
    }

    void OnGUI()
    {
        GUIStyle moneyStyle = new GUIStyle();
        moneyStyle.fontSize = 30;
        moneyStyle.alignment = TextAnchor.UpperRight;
        moneyStyle.normal.textColor = Color.yellow;
        GUI.Label(new Rect(Screen.width - 210, 20, 190, 50), "Монеты: " + currentMoney, moneyStyle);
    }
}