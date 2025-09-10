using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ChangeMasterVolume()
    {
        mixer.SetFloat("Master",masterSlider.value);
    }
    public void ChangeBGMVolume()
    {
        mixer.SetFloat("BGM",musicSlider.value);
    }
    public void ChangeSFXVolume()
    {
        mixer.SetFloat("SFX",sfxSlider.value);
    }
}
