using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeController : MonoBehaviour
{
    public static FadeController Instance;
    [SerializeField] private CanvasGroup canvasGroup;

    void Awake()
    {
        Instance = this;
    }

    public IEnumerator FadeParaCena(string nomeDaCena, float duracao = 1f)
    {
        // Fade in (tela escurece)
        float tempo = 0f;
        canvasGroup.blocksRaycasts = true;
        while (tempo < duracao)
        {
            tempo += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(tempo / duracao);
            yield return null;
        }

        AudioManager.Instance.PararMusica();
        SceneManager.LoadScene(nomeDaCena);
    }
}