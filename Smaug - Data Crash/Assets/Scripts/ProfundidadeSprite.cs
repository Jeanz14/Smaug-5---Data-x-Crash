using UnityEngine;

/// <summary>
/// Ordena os sprites pela PROFUNDIDADE (Y dos pés), como num beat 'em up:
/// quem está mais embaixo na tela (mais perto da câmera) é desenhado na frente.
///
/// Independente: não depende de nenhum outro script do projeto.
/// Use em: Player, Inimigos, Obstáculos, Props, Coletáveis, Projéteis.
/// </summary>
[DisallowMultipleComponent]
public class ProfundidadeSprite : MonoBehaviour
{
    [Header("Pés")]
    [Tooltip("Distância em Y do pivô do objeto até o ponto onde ele toca o chão. Negativo se os pés estão abaixo do pivô. A linha vermelha na Scene View mostra esse ponto.")]
    [SerializeField] private float offsetDoPeY = 0f;

    [Header("Sorting Layer")]
    [Tooltip("Todos os objetos ordenados por profundidade precisam estar na MESMA Sorting Layer. Deixe vazio para não alterar.")]
    [SerializeField] private string sortingLayer = "Personagens";

    [Header("Comportamento")]
    [Tooltip("Marque em objetos que não se movem (caixas, postes, coletáveis). Ordena uma vez só.")]
    [SerializeField] private bool estatico = false;

    [Tooltip("Níveis de ordem por unidade de Y. Maior = mais preciso. Com 20 funciona até cerca de ±160 de Y.")]
    [SerializeField] private int precisao = 20;

    /// <summary>
    /// Altura atual de um pulo/voo (0 = no chão). Quem faz o pulo deve atualizar este valor,
    /// assim o objeto não muda de camada enquanto está no ar.
    /// </summary>
    public float AlturaDoSalto { get; set; } = 0f;

    // Cada nível de profundidade reserva 10 vagas para ajustes finos entre os sprites do mesmo objeto
    // (ex.: sombra = -1, corpo = 0, efeito = +1 no "Order in Layer" do Inspector).
    private const int ESPACO_POR_NIVEL = 10;

    private Renderer[] renderers;
    private int[] ordemLocal;
    private int ultimaBase = int.MinValue;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        ordemLocal = new int[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
            ordemLocal[i] = renderers[i].sortingOrder;

        AplicarSortingLayer();
        Atualizar();

        if (estatico)
            enabled = false; // não precisa rodar o LateUpdate
    }

    // LateUpdate: roda depois de todos os movimentos do frame
    void LateUpdate()
    {
        Atualizar();
    }

    private void AplicarSortingLayer()
    {
        if (string.IsNullOrEmpty(sortingLayer)) return;

        int id = SortingLayer.NameToID(sortingLayer);
        if (!SortingLayer.IsValid(id))
        {
            Debug.LogWarning("ProfundidadeSprite em '" + name + "': a Sorting Layer '" + sortingLayer + "' não existe. Crie em Project Settings > Tags and Layers.", this);
            return;
        }

        foreach (Renderer r in renderers)
            if (r != null) r.sortingLayerID = id;
    }

    private void Atualizar()
    {
        float yDosPes = transform.position.y - AlturaDoSalto + offsetDoPeY;
        int baseOrdem = Mathf.RoundToInt(-yDosPes * precisao) * ESPACO_POR_NIVEL;

        if (baseOrdem == ultimaBase) return; // nada mudou
        ultimaBase = baseOrdem;

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null) continue;
            renderers[i].sortingOrder = Mathf.Clamp(baseOrdem + ordemLocal[i], short.MinValue, short.MaxValue);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 pe = transform.position + new Vector3(0f, offsetDoPeY, 0f);
        Gizmos.DrawLine(pe + Vector3.left * 1.5f, pe + Vector3.right * 1.5f);
        Gizmos.DrawWireSphere(pe, 0.12f);
    }
}