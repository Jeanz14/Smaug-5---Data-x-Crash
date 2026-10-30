using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Área (trigger) de fim de fase. Quando o Player entra, inicia a TransicaoDeFase.
/// Uso: crie um objeto vazio, adicione um BoxCollider2D com "Is Trigger" marcado e este script.
/// </summary>
public class GatilhoFimDeFase : MonoBehaviour
{
    [Header("Destino")]
    [Tooltip("Nome exato da cena. Ela precisa estar em File > Build Settings.")]
    [SerializeField] private string nomeDaCena;

    [Tooltip("Se marcado, ignora o nome acima e vai para a próxima cena da lista do Build Settings.")]
    [SerializeField] private bool irParaProximaCenaDoBuild = false;

    [Header("Mensagem")]
    [Tooltip("Deixe vazio para não mostrar texto.")]
    [SerializeField] private string mensagem = "FASE CONCLUÍDA";
    [SerializeField] private Color corMensagem = new Color(0.18f, 1f, 0.31f, 1f); // verde
    [SerializeField] private float tamanhoFonte = 120f;
    [Tooltip("Opcional. Vazio = fonte padrão do TextMeshPro.")]
    [SerializeField] private TMP_FontAsset fonte;

    [Header("Tempos (segundos)")]
    [SerializeField] private float duracaoFadeOut = 1f;
    [SerializeField] private float duracaoFadeMensagem = 0.5f;
    [SerializeField] private float tempoMensagemVisivel = 1.5f;
    [SerializeField] private float duracaoFadeIn = 1f;

    [Header("Player")]
    [SerializeField] private string tagDoAlvo = "Player";
    [Tooltip("Desliga os scripts do Player e congela a animação durante a transição.")]
    [SerializeField] private bool travarPlayer = true;

    private bool ativado = false;

    void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
            Debug.LogError("GatilhoFimDeFase em '" + name + "': falta um Collider2D (ex.: BoxCollider2D) com Is Trigger marcado.", this);
        else if (!col.isTrigger)
            Debug.LogWarning("GatilhoFimDeFase em '" + name + "': marque Is Trigger no collider, senão ele bloqueia o Player em vez de detectar.", this);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (ativado) return;
        if (!other.CompareTag(tagDoAlvo)) return;

        string destino = ResolverDestino();
        if (string.IsNullOrEmpty(destino))
        {
            Debug.LogError("GatilhoFimDeFase em '" + name + "': nenhuma cena de destino definida.", this);
            return;
        }
        if (!Application.CanStreamedLevelBeLoaded(destino))
        {
            Debug.LogError("GatilhoFimDeFase em '" + name + "': a cena '" + destino + "' não existe ou não está no Build Settings.", this);
            return;
        }

        ativado = true;

        if (travarPlayer)
            TravarPlayer(other.gameObject);

        TransicaoDeFase.Config config = new TransicaoDeFase.Config
        {
            mensagem = mensagem,
            corMensagem = corMensagem,
            tamanhoFonte = tamanhoFonte,
            fonte = fonte,
            duracaoFadeOut = duracaoFadeOut,
            duracaoFadeMensagem = duracaoFadeMensagem,
            tempoMensagemVisivel = tempoMensagemVisivel,
            duracaoFadeIn = duracaoFadeIn
        };

        TransicaoDeFase.Iniciar(destino, config);
    }

    private string ResolverDestino()
    {
        if (!irParaProximaCenaDoBuild)
            return nomeDaCena;

        int atual = SceneManager.GetActiveScene().buildIndex;
        if (atual < 0)
        {
            Debug.LogError("GatilhoFimDeFase: a cena atual não está no Build Settings.", this);
            return null;
        }

        int proximo = atual + 1;
        if (proximo >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError("GatilhoFimDeFase: esta já é a última cena do Build Settings.", this);
            return null;
        }

        string caminho = SceneUtility.GetScenePathByBuildIndex(proximo);
        return System.IO.Path.GetFileNameWithoutExtension(caminho);
    }

    // Não depende de nenhum script do Player: desliga todos e congela o Animator
    private void TravarPlayer(GameObject player)
    {
        foreach (MonoBehaviour script in player.GetComponents<MonoBehaviour>())
            script.enabled = false;

        Animator anim = player.GetComponent<Animator>();
        if (anim != null)
            anim.speed = 0f;
    }

    void OnDrawGizmos()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null) return;

        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.25f);
        Gizmos.DrawCube(col.bounds.center, col.bounds.size);
    }
}