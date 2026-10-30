using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Fontes de áudio")]
    [SerializeField] private AudioSource musicaSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Músicas")]
    [SerializeField] private string nomeCenaMenu = "MenuPrincipal";
    [SerializeField] private AudioClip musicaMenu;
    [SerializeField] private AudioClip musicaFase;

    [Header("Efeitos sonoros")]
    [SerializeField] private AudioClip sfxBotao;
    [SerializeField] private AudioClip sfxDanoPlayer;
    [SerializeField] private AudioClip sfxDanoInimigo;

    public AudioSource MusicaSource => musicaSource;
    public float VolumeMusica { get; private set; } = 1f;
    public float VolumeSFX { get; private set; } = 1f;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicaSource.loop = true;
        musicaSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;

        SceneManager.sceneLoaded += AoCarregarCena;
        TocarMusicaDaCena(SceneManager.GetActiveScene()); // a 1ª cena carrega antes de assinarmos o evento
    }

    void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= AoCarregarCena;
    }

    private void AoCarregarCena(Scene cena, LoadSceneMode modo)
    {
        musicaSource.pitch = 1f; // garante que o efeito de câmera lenta da morte não "vaze"
        TocarMusicaDaCena(cena);
    }

    private void TocarMusicaDaCena(Scene cena)
    {
        TocarMusica(cena.name == nomeCenaMenu ? musicaMenu : musicaFase);
    }

    // ── Música ────────────────────────────────────────
    public void TocarMusica(AudioClip clip)
    {
        if (clip == null) return;
        if (musicaSource.clip == clip && musicaSource.isPlaying) return; // não reinicia a mesma música

        musicaSource.clip = clip;
        musicaSource.pitch = 1f;
        musicaSource.Play();
    }

    public void PararMusica()
    {
        musicaSource.Stop();
        musicaSource.clip = null; // assim a próxima TocarMusica sempre recomeça
    }

    // ── Efeitos ───────────────────────────────────────
    public void TocarSFX(AudioClip clip)
    {
        if (clip != null) sfxSource.PlayOneShot(clip);
    }

    public void TocarSomBotao() => TocarSFX(sfxBotao);
    public void TocarSomDanoPlayer() => TocarSFX(sfxDanoPlayer);
    public void TocarSomDanoInimigo() => TocarSFX(sfxDanoInimigo);

    // ── Volume (via Mixer, valor 0 a 1) ──────────────
    public void AlterarVolumeMusica(float valor)
    {
        VolumeMusica = valor;
        audioMixer.SetFloat("VolumeMusica", ParaDecibeis(valor));
    }

    public void AlterarVolumeSFX(float valor)
    {
        VolumeSFX = valor;
        audioMixer.SetFloat("VolumeSFX", ParaDecibeis(valor));
    }

    private float ParaDecibeis(float valor)
    {
        return valor > 0.001f ? Mathf.Log10(valor) * 20f : -80f;
    }
}