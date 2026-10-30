using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Transição de cena 100% independente.
/// Cria sozinha o Canvas, o fundo preto e o texto, faz o fade, mostra a mensagem,
/// troca de cena, clareia a tela na cena nova e se destrói.
/// Não usa nenhum outro script do projeto e não precisa de nada na Hierarchy.
/// </summary>
public class TransicaoDeFase : MonoBehaviour
{
    public class Config
    {
        public string mensagem = "FASE CONCLUÍDA";
        public Color corMensagem = new Color(0.18f, 1f, 0.31f, 1f); // verde
        public float tamanhoFonte = 120f;
        public TMP_FontAsset fonte = null;
        public float duracaoFadeOut = 1f;
        public float duracaoFadeMensagem = 0.5f;
        public float tempoMensagemVisivel = 1.5f;
        public float duracaoFadeIn = 1f;
    }

    private static TransicaoDeFase ativa;

    private Config cfg;
    private Image fundo;
    private TextMeshProUGUI texto;

    public static void Iniciar(string nomeDaCena, Config config)
    {
        if (ativa != null) return; // já existe uma transição em andamento

        if (!Application.CanStreamedLevelBeLoaded(nomeDaCena))
        {
            Debug.LogError("TransicaoDeFase: a cena '" + nomeDaCena + "' não existe ou não está no Build Settings.");
            return;
        }

        GameObject go = new GameObject("TransicaoDeFase", typeof(RectTransform));
        DontDestroyOnLoad(go); // sobrevive à troca de cena

        TransicaoDeFase t = go.AddComponent<TransicaoDeFase>();
        t.cfg = config;
        ativa = t;
        t.StartCoroutine(t.Rotina(nomeDaCena));
    }

    void OnDestroy()
    {
        if (ativa == this) ativa = null;
    }

    private IEnumerator Rotina(string nomeDaCena)
    {
        ConstruirUI();

        // 1. Escurece a tela
        yield return StartCoroutine(FadeFundo(0f, 1f, cfg.duracaoFadeOut));

        // 2. Mostra a mensagem
        if (!string.IsNullOrEmpty(cfg.mensagem))
        {
            yield return StartCoroutine(FadeTexto(0f, 1f, cfg.duracaoFadeMensagem));
            yield return StartCoroutine(Esperar(cfg.tempoMensagemVisivel));
            yield return StartCoroutine(FadeTexto(1f, 0f, cfg.duracaoFadeMensagem));
        }

        // 3. Troca de cena (garante que a cena nova não comece pausada)
        Time.timeScale = 1f;
        AsyncOperation carregamento = SceneManager.LoadSceneAsync(nomeDaCena);
        while (!carregamento.isDone)
            yield return null;
        yield return null; // um frame para a cena nova iniciar

        // 4. Clareia a tela na cena nova
        fundo.raycastTarget = false; // libera os cliques da cena nova
        yield return StartCoroutine(FadeFundo(1f, 0f, cfg.duracaoFadeIn));

        Destroy(gameObject);
    }

    // ── Construção da interface por código ───────────────────────────
    private void ConstruirUI()
    {
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32000; // por cima de qualquer outro Canvas

        CanvasScaler escala = gameObject.AddComponent<CanvasScaler>();
        escala.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        escala.referenceResolution = new Vector2(1920f, 1080f);
        escala.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        escala.matchWidthOrHeight = 0.5f;

        gameObject.AddComponent<GraphicRaycaster>(); // faz o fundo bloquear cliques durante a transição

        // Fundo preto, tela cheia
        GameObject goFundo = new GameObject("Fundo", typeof(RectTransform));
        goFundo.transform.SetParent(transform, false);
        fundo = goFundo.AddComponent<Image>();
        fundo.color = new Color(0f, 0f, 0f, 0f);

        RectTransform rtFundo = fundo.rectTransform;
        rtFundo.anchorMin = Vector2.zero;
        rtFundo.anchorMax = Vector2.one;
        rtFundo.offsetMin = Vector2.zero;
        rtFundo.offsetMax = Vector2.zero;

        // Texto no centro
        GameObject goTexto = new GameObject("Mensagem", typeof(RectTransform));
        goTexto.transform.SetParent(transform, false);
        texto = goTexto.AddComponent<TextMeshProUGUI>();

        if (cfg.fonte != null)
            texto.font = cfg.fonte;

        texto.text = cfg.mensagem;
        texto.fontSize = cfg.tamanhoFonte;
        texto.fontStyle = FontStyles.Bold;
        texto.alignment = TextAlignmentOptions.Center;
        texto.raycastTarget = false;
        DefinirAlphaTexto(0f);

        RectTransform rtTexto = texto.rectTransform;
        rtTexto.anchorMin = new Vector2(0.5f, 0.5f);
        rtTexto.anchorMax = new Vector2(0.5f, 0.5f);
        rtTexto.pivot = new Vector2(0.5f, 0.5f);
        rtTexto.anchoredPosition = Vector2.zero;
        rtTexto.sizeDelta = new Vector2(1700f, 300f);
    }

    // ── Efeitos (tempo real: não travam se o jogo estiver pausado) ───
    private IEnumerator FadeFundo(float de, float para, float duracao)
    {
        duracao = Mathf.Max(0.01f, duracao);
        float tempo = 0f;

        while (tempo < duracao)
        {
            tempo += Time.unscaledDeltaTime;
            DefinirAlphaFundo(Mathf.Lerp(de, para, Mathf.Clamp01(tempo / duracao)));
            yield return null;
        }
        DefinirAlphaFundo(para);
    }

    private IEnumerator FadeTexto(float de, float para, float duracao)
    {
        duracao = Mathf.Max(0.01f, duracao);
        float tempo = 0f;

        while (tempo < duracao)
        {
            tempo += Time.unscaledDeltaTime;
            DefinirAlphaTexto(Mathf.Lerp(de, para, Mathf.Clamp01(tempo / duracao)));
            yield return null;
        }
        DefinirAlphaTexto(para);
    }

    private IEnumerator Esperar(float segundos)
    {
        float tempo = 0f;
        while (tempo < segundos)
        {
            tempo += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    private void DefinirAlphaFundo(float alpha)
    {
        fundo.color = new Color(0f, 0f, 0f, alpha);
    }

    private void DefinirAlphaTexto(float alpha)
    {
        Color c = cfg.corMensagem;
        texto.color = new Color(c.r, c.g, c.b, alpha);
    }
}