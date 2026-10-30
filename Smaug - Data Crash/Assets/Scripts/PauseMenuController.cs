using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public static PauseMenuController Instance;

    [Header("Paineis")]
    [SerializeField] private GameObject painelPausa;
    [SerializeField] private GameObject painelOpcoes;
    [SerializeField] private GameObject painelControles;

    [Header("Sliders de volume")]
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
        sliderMusica.onValueChanged.RemoveAllListeners();
        sliderSFX.onValueChanged.RemoveAllListeners();

        sliderMusica.navigation = new Navigation() { mode = Navigation.Mode.None };
        sliderSFX.navigation = new Navigation() { mode = Navigation.Mode.None };

        // Primeiro posiciona o slider com o volume salvo, depois liga o listener
        sliderMusica.value = AudioManager.Instance.VolumeMusica;
        sliderSFX.value = AudioManager.Instance.VolumeSFX;

        sliderMusica.onValueChanged.AddListener(AlterarVolumeMusica);
        sliderSFX.onValueChanged.AddListener(AlterarVolumeSFX);
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

    // ── Volume (repassa para o AudioManager) ──────────
    public void AlterarVolumeMusica(float valor) => AudioManager.Instance.AlterarVolumeMusica(valor);
    public void AlterarVolumeSFX(float valor) => AudioManager.Instance.AlterarVolumeSFX(valor);
}