using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class DeathMenuController : MonoBehaviour
{
    public static DeathMenuController Instance;

    [Header("Painel")]
    [SerializeField] private GameObject painelMorte;
    [SerializeField] private Image imgFade;

    [Header("Audio")]
    [SerializeField] private AudioSource musicaFase;
    [SerializeField] private float pitchLento = 0.5f;
    [SerializeField] private float duracaoEfeito = 0.6f;

    [Header("Configuracao do Fade")]
    [SerializeField] private float alphaFinal = 0.85f;

    private bool morteAtivada = false;

    void Awake()
    {
        Instance = this;
        painelMorte.SetActive(false);

        if (imgFade != null)
            imgFade.color = new Color(0f, 0f, 0f, 0f);
    }

    public void AtivarMenuMorte()
    {
        Debug.Log("AtivarMenuMorte chamado! morteAtivada=" + morteAtivada);
        if (morteAtivada) return;
        morteAtivada = true;
        StartCoroutine(SequenciaMorte());
    }

    private IEnumerator SequenciaMorte()
    {
        StartCoroutine(DesacelerarMusica());
        yield return StartCoroutine(FadeEscurecer());
        Time.timeScale = 0f;
        painelMorte.SetActive(true);
    }

    private IEnumerator FadeEscurecer()
    {
        if (imgFade == null)
        {
            Debug.LogError("ImgFade não atribuído no DeathMenuController!");
            yield break;
        }

        float tempo = 0f;

        while (tempo < duracaoEfeito)
        {
            tempo += Time.unscaledDeltaTime;
            float alpha = Mathf.Clamp01(tempo / duracaoEfeito) * alphaFinal;
            imgFade.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        imgFade.color = new Color(0f, 0f, 0f, alphaFinal);
    }

    private IEnumerator DesacelerarMusica()
    {
        if (musicaFase == null) yield break;

        float pitchOriginal = musicaFase.pitch;
        float tempo = 0f;

        while (tempo < duracaoEfeito)
        {
            tempo += Time.deltaTime;
            musicaFase.pitch = Mathf.Lerp(pitchOriginal, pitchLento, tempo / duracaoEfeito);
            yield return null;
        }
    }

    public void TentarNovamente()
    {
        morteAtivada = false;
        Time.timeScale = 1f;

        if (musicaFase != null)
            musicaFase.pitch = 1f;

        if (imgFade != null)
            imgFade.color = new Color(0f, 0f, 0f, 0f);

        painelMorte.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void VoltarAoMenu()
    {
        morteAtivada = false;
        Time.timeScale = 1f;

        if (musicaFase != null)
            musicaFase.pitch = 1f;

        SceneManager.LoadScene("MenuPrincipal");
    }
}