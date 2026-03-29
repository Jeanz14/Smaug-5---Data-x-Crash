using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioSource musicaSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // música continua entre cenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AlterarVolumeMusica(float volume)
    {
        musicaSource.volume = volume;
    }

    public void AlterarVolumeSons(float volume)
    {
        // Aqui você pode referenciar outros AudioSources de SFX futuramente
        AudioListener.volume = volume;
    }

    public void PararMusica()
    {
        musicaSource.Stop();
    }
}