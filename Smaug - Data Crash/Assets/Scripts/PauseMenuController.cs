using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class PauseMenuController : MonoBehaviour
{
    public static PauseMenuController Instance;

    [Header("Paineis")]
    [SerializeField] private GameObject painelPausa;
    [SerializeField] private GameObject painelOpcoes;
    [SerializeField] private GameObject painelControles;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider sliderMusica;
    [SerializeField] private Slider sliderSFX;

    private bool pausado = false;

    void Awake()
    {
        Instance = this;
        painelPausa.SetActive(false);
        painelOpcoes.SetActive(false);
        painelControles.SetActive(false);
    }

    void Start()
    {
        // Configura sliders
        sliderMusica.onValueChanged.RemoveAllListeners();
        sliderSFX.onValueChanged.RemoveAllListeners();

        sliderMusica.onValueChanged.AddListener(AlterarVolumeMusica);
        sliderSFX.onValueChanged.AddListener(AlterarVolumeSFX);

        sliderMusica.navigation = new Navigation() { mode = Navigation.Mode.None };
        sliderSFX.navigation = new Navigation() { mode = Navigation.Mode.None };

        sliderMusica.value = 1f;
        sliderSFX.value = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (pausado) Retomar();
            else Pausar();
        }
    }

    // ── Pausa ─────────────────────────────────────────
    public void Pausar()
    {
        pausado = true;
        Time.timeScale = 0f;
        painelPausa.SetActive(true);
    }

    public void Retomar()
    {
        pausado = false;
        Time.timeScale = 1f;
        painelPausa.SetActive(false);
        painelOpcoes.SetActive(false);
        painelControles.SetActive(false);
    }

    // ── Navegação ─────────────────────────────────────
    public void AbrirOpcoes()
    {
        painelPausa.SetActive(false);
        painelOpcoes.SetActive(true);
    }

    public void AbrirControles()
    {
        painelPausa.SetActive(false);
        painelControles.SetActive(true);
    }

    public void VoltarDasOpcoes()
    {
        painelOpcoes.SetActive(false);
        painelPausa.SetActive(true);
    }

    public void VoltarDosControles()
    {
        painelControles.SetActive(false);
        painelPausa.SetActive(true);
    }

    public void VoltarAoMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
    }

    // ── Volume ────────────────────────────────────────
    public void AlterarVolumeMusica(float valor)
    {
        float db = valor > 0.001f ? Mathf.Log10(valor) * 20f : -80f;
        audioMixer.SetFloat("VolumeMusica", db);
    }

    public void AlterarVolumeSFX(float valor)
    {
        float db = valor > 0.001f ? Mathf.Log10(valor) * 20f : -80f;
        audioMixer.SetFloat("VolumeSFX", db);
    }
}