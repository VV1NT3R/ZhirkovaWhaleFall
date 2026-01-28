using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip buttonClickSound;
    public float soundDelay = 0.3f;

    [Header("UI")]
    public GameObject mainButtonsPanel;
    public GameObject aboutPanel;       

    void Start()
    {
        mainButtonsPanel.SetActive(true);
        aboutPanel.SetActive(false);
    }

    public void StartGame()
    {
        PlayButtonSound();
        Invoke("LoadGameScene", soundDelay);
    }

    public void ExitGame()
    {
        PlayButtonSound();
        Invoke("QuitApplication", soundDelay);
    }

    public void OpenAbout()
    {
        PlayButtonSound();
        mainButtonsPanel.SetActive(false);
        aboutPanel.SetActive(true);
    }

    public void CloseAbout()
    {
        PlayButtonSound();
        aboutPanel.SetActive(false);
        mainButtonsPanel.SetActive(true);
    }

    private void PlayButtonSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene(1);
    }

    private void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
