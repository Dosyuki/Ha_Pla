using UnityEngine;

public class BGMLoop : Singleton<BGMLoop>
{
    public AudioSource audioSource;
    public AudioClip audioClip;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource.Play();
        DontDestroyOnLoad(this);
    }

}
