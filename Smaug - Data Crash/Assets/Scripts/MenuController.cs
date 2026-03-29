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

        
        sliderMusica.onValueChanged.RemoveAllListeners();
        sliderSons.onValueChanged.RemoveAllListeners();

        sliderMusica.onValueChanged.AddListener(AudioManager.Instance.AlterarVolumeMusica);
        sliderSons.onValueChanged.AddListener(AudioManager.Instance.AlterarVolumeSons);

        
        sliderMusica.value = 1f;
        sliderSons.value = 1f;
    }

    
    public void BotaoJogar()
    {
        StartCoroutine(FadeController.Instance.FadeParaCena("Fase01"));
    }

    
    public void AbrirOpcoes()
    {
        painelOpcoes.SetActive(true);
    }

    public void FecharOpcoes()
    {
        painelOpcoes.SetActive(false);
    }

    
    public void AbrirCreditos()
    {
        painelCreditos.SetActive(true);
    }

    public void VoltarDoCreditos()
    {
        painelCreditos.SetActive(false);
    }

    
    public void BotaoSair()
    {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }   
}