using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("Paineis")]
    [SerializeField] private GameObject painelOpcoes;
    [SerializeField] private GameObject painelCreditos;

    [Header("Sliders")]
    [SerializeField] private Slider sliderMusica;
    [SerializeField] private Slider sliderSons;

    void Start()
    {
        painelOpcoes.SetActive(false);
        painelCreditos.SetActive(false);

        // Remove qualquer listener existente antes de adicionar os novos
        sliderMusica.onValueChanged.RemoveAllListeners();
        sliderSons.onValueChanged.RemoveAllListeners();

        sliderMusica.onValueChanged.AddListener(AudioManager.Instance.AlterarVolumeMusica);
        sliderSons.onValueChanged.AddListener(AudioManager.Instance.AlterarVolumeSons);

        // Define os valores DEPOIS de conectar os listeners
        sliderMusica.value = 1f;
        sliderSons.value = 1f;
    }

    // Botão JOGAR
    public void BotaoJogar()
    {
        StartCoroutine(FadeController.Instance.FadeParaCena("Fase01"));
    }

    // Botão OPÇÕES
    public void AbrirOpcoes()
    {
        painelOpcoes.SetActive(true);
    }

    public void FecharOpcoes()
    {
        painelOpcoes.SetActive(false);
    }

    // Botão CRÉDITOS
    public void AbrirCreditos()
    {
        painelCreditos.SetActive(true);
    }

    public void VoltarDoCreditos()
    {
        painelCreditos.SetActive(false);
    }

    // Botão SAIR
    public void BotaoSair()
    {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }   
}