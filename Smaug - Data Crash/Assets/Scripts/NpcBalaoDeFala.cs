using UnityEngine;

/// <summary>
/// NPC com balão de fala por proximidade.
/// Quando o Player entra na área (trigger) o sprite do NPC muda (ex.: sprite com balão)
/// e/ou um balão separado aparece. Quando o Player sai, tudo volta ao normal.
///
/// Independente: só usa a tag "Player" do projeto.
/// Uso: no NPC, adicione um Collider2D com "Is Trigger" marcado (a área de alcance) e este script.
/// </summary>
public class NpcBalaoDeFala : MonoBehaviour
{
    [Header("Sprite do NPC")]
    [Tooltip("O Sprite Renderer do NPC (se vazio, usa o do próprio objeto).")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Tooltip("Sprite(s) mostrado(s) enquanto o Player está perto. Com mais de um, eles se alternam (um balão após o outro).")]
    [SerializeField] private Sprite[] spritesFalando;

    [Tooltip("Tempo (segundos) que cada sprite fica na tela quando há mais de um.")]
    [SerializeField] private float tempoPorSprite = 2f;

    [Header("Correção de posição (sprite maior que o normal)")]
    [Tooltip("Mantém o NPC parado ao trocar de sprite, alinhando a BASE-CENTRO dos dois sprites (equivale a usar Pivot Bottom nos dois). " +
             "Desmarque se os pivôs dos sprites já estão alinhados manualmente no Sprite Editor.")]
    [SerializeField] private bool compensarPivo = true;

    [Tooltip("Ajuste extra (em unidades) aplicado enquanto o sprite de fala está ativo. X positivo = direita, Y positivo = cima. " +
             "Vale para o NPC virado para a direita; é espelhado se o sprite estiver invertido (Flip X).")]
    [SerializeField] private Vector2 ajusteFino = Vector2.zero;

    [Header("Balão como objeto separado (opcional)")]
    [Tooltip("Um objeto filho com o balão. Ele é ligado quando o Player chega perto e desligado quando se afasta.")]
    [SerializeField] private GameObject balao;

    [Header("Comportamento")]
    [SerializeField] private string tagDoAlvo = "Player";

    [Tooltip("Espera antes de esconder o balão depois que o Player sai. Evita piscar quando ele fica na borda da área.")]
    [SerializeField] private float atrasoParaSumir = 0.15f;

    private Sprite spriteNormal;
    private Vector3 posLocalOriginal;

    // Se o Sprite Renderer está no MESMO objeto dos colliders, mover o sprite moveria os colliders junto.
    // Para isso não acontecer, os colliders recebem o deslocamento oposto (ficam parados no mundo).
    private bool rendererNoMesmoObjeto;
    private Collider2D[] colliders;
    private Vector2[] offsetsOriginais;

    private int dentro = 0;          // quantos colliders do Player estão na área
    private bool falando = false;
    private float tempoFora = 0f;
    private float tempoNoSprite = 0f;
    private int indiceSprite = 0;

    void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteNormal = spriteRenderer.sprite;                        // guarda o sprite original
            posLocalOriginal = spriteRenderer.transform.localPosition;   // e a posição original
            rendererNoMesmoObjeto = spriteRenderer.transform == transform;
        }
        else if (spritesFalando != null && spritesFalando.Length > 0)
        {
            Debug.LogWarning("NpcBalaoDeFala em '" + name + "': há sprites de fala, mas nenhum Sprite Renderer foi definido.", this);
        }

        // Guarda os offsets originais dos colliders deste objeto
        colliders = GetComponents<Collider2D>();
        offsetsOriginais = new Vector2[colliders.Length];
        for (int i = 0; i < colliders.Length; i++)
            offsetsOriginais[i] = colliders[i].offset;

        if (balao != null)
            balao.SetActive(false);

        if (colliders.Length == 0)
            Debug.LogError("NpcBalaoDeFala em '" + name + "': falta um Collider2D com Is Trigger marcado (a área de alcance).", this);
        else if (!colliders[0].isTrigger)
            Debug.LogWarning("NpcBalaoDeFala em '" + name + "': marque Is Trigger no collider, senão ele bloqueia o Player em vez de detectar.", this);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(tagDoAlvo)) return;

        dentro++;
        tempoFora = 0f;

        if (!falando)
            Mostrar();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(tagDoAlvo)) return;

        dentro = Mathf.Max(0, dentro - 1);
    }

    void Update()
    {
        if (!falando) return;

        if (dentro == 0)
        {
            tempoFora += Time.deltaTime;
            if (tempoFora >= atrasoParaSumir)
                Esconder();
            return;
        }

        AlternarSprites();
    }

    void OnDisable()
    {
        dentro = 0;
        if (falando)
            Esconder();
    }

    private void Mostrar()
    {
        falando = true;
        indiceSprite = 0;
        tempoNoSprite = 0f;

        AplicarSprite(0);

        if (balao != null)
            balao.SetActive(true);
    }

    private void Esconder()
    {
        falando = false;

        if (spriteRenderer != null)
        {
            if (spriteNormal != null)
                spriteRenderer.sprite = spriteNormal;

            spriteRenderer.transform.localPosition = posLocalOriginal; // volta para a posição original
        }

        DeslocarColliders(Vector2.zero); // colliders voltam ao offset original

        if (balao != null)
            balao.SetActive(false);
    }

    private void AplicarSprite(int indice)
    {
        if (spriteRenderer == null) return;
        if (spritesFalando == null || spritesFalando.Length == 0) return;

        Sprite novo = spritesFalando[indice];
        if (novo == null) return;

        spriteRenderer.sprite = novo;
        AplicarPosicao(novo);
    }

    // Move o sprite para que a base do NPC não "ande" quando o sprite é maior
    private void AplicarPosicao(Sprite novo)
    {
        Vector2 deslocamento = ajusteFino;

        if (compensarPivo && spriteNormal != null)
            deslocamento += BaseCentro(spriteNormal) - BaseCentro(novo);

        if (spriteRenderer.flipX)
            deslocamento.x = -deslocamento.x;

        Transform t = spriteRenderer.transform;
        Vector3 mundo = t.TransformVector(new Vector3(deslocamento.x, deslocamento.y, 0f));
        Vector3 local = t.parent != null ? t.parent.InverseTransformVector(mundo) : mundo;

        t.localPosition = posLocalOriginal + local;

        // Se o sprite está no mesmo objeto dos colliders, eles andaram junto: desfaz isso
        if (rendererNoMesmoObjeto)
            DeslocarColliders((Vector2)t.InverseTransformVector(-mundo));
        else
            DeslocarColliders(Vector2.zero);
    }

    // Define o offset de cada collider = offset original + deslocamento (em espaço local)
    private void DeslocarColliders(Vector2 deslocamentoLocal)
    {
        if (colliders == null) return;

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null)
                colliders[i].offset = offsetsOriginais[i] + deslocamentoLocal;
        }
    }

    // Ponto "base no centro" do sprite, relativo ao pivô, em unidades
    private Vector2 BaseCentro(Sprite s)
    {
        return new Vector2(
            (s.rect.width * 0.5f - s.pivot.x) / s.pixelsPerUnit,
            -s.pivot.y / s.pixelsPerUnit
        );
    }

    private void AlternarSprites()
    {
        if (spritesFalando == null || spritesFalando.Length < 2) return;

        tempoNoSprite += Time.deltaTime;
        if (tempoNoSprite < tempoPorSprite) return;

        tempoNoSprite = 0f;
        indiceSprite = (indiceSprite + 1) % spritesFalando.Length;
        AplicarSprite(indiceSprite);
    }

    // Mostra a área de alcance na Scene View (amarelo translúcido)
    void OnDrawGizmos()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null) return;

        Gizmos.color = new Color(1f, 0.85f, 0.2f, 0.25f);

        if (col is CircleCollider2D)
            Gizmos.DrawSphere(col.bounds.center, col.bounds.extents.x);
        else
            Gizmos.DrawCube(col.bounds.center, col.bounds.size);
    }
}