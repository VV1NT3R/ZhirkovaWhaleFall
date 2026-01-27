using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip buttonClickSound;
    public float soundDelay = 0.3f;

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