using UnityEngine;
using UnityEngine.UI;

public class PauseMenuButtons : MonoBehaviour
{
    [Header("Кнопки паузы")]
    public Button continueButton; 
    public Button quitButton;     

    [Header("Слайдер громкости")]
    public Slider volumeSlider;       
    public AudioSource musicSource;   

    private void Awake()
    {
    
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);

       
        if (volumeSlider != null)
        {
            float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            volumeSlider.value = savedVolume;

            if (musicSource != null)
                musicSource.volume = savedVolume;

            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    private void OnContinueClicked()
    {
        if (PauseManager.Instance != null)
            PauseManager.Instance.Resume(); 
    }

    private void OnQuitClicked()
    {
        Debug.Log("Выход из игры");
        Application.Quit(); 
    }

    private void SetVolume(float value)
    {
        if (musicSource != null)
            musicSource.volume = value;

        PlayerPrefs.SetFloat("MusicVolume", value);
    }
}
