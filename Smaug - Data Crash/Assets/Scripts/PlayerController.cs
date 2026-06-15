using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public enum Acao : int
    {
        Idle = 0,
        Andar = 1,
        Pular = 2,
        Correr = 3,
        AtacarLeve = 4,
        AtacarForte = 5,
        AtaqueEspecial = 6,
        TomandoDano = 7
    }

    [Header("Movimento")]
    [SerializeField] private float velocidade = 5f;
    [SerializeField] private Rigidbody2D rb;

    [Header("Pulo")]
    [SerializeField] private float alturaPulo = 2f;
    [SerializeField] private float duracaoPulo = 0.4f;

    [Header("Limites W/S")]
    [SerializeField] private float limiteYcima = 1f;
    [SerializeField] private float limiteYbaixo = -2f;

    [Header("Animator")]
    [SerializeField] private Animator anim;

    [Header("Colisao")]
    [SerializeField] private BoxCollider2D corpoCollider;
    [SerializeField] private LayerMask layerObstaculos;

    private SpriteRenderer sr;
    private bool pulando = false;
    private float groundY;
    private float direcaoAtual = 1f;

    void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (sr == null)
            sr = GetComponentInChildren<SpriteRenderer>();

        corpoCollider = GetComponent<BoxCollider2D>();
        groundY = transform.position.y;
    }

    void FixedUpdate()
    {
        Mover();

        if (Input.GetKeyDown(KeyCode.Space) && !pulando)
            StartCoroutine(Pular());
    }

    private void Mover()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsTag("Movimento")) return;

        float h = 0f;
        float v = 0f;

        if (Input.GetKey(KeyCode.A)) h = -1f;
        if (Input.GetKey(KeyCode.D)) h = 1f;
        if (Input.GetKey(KeyCode.W)) v = 1f;
        if (Input.GetKey(KeyCode.S)) v = -1f;

        if (h > 0) direcaoAtual = 1f;
        if (h < 0) direcaoAtual = -1f;

        if (h != 0 || v != 0) anim.SetInteger("PlayerState", (int)Acao.Andar);
        else anim.SetInteger("PlayerState", (int)Acao.Idle);

        if (sr != null)
            sr.flipX = direcaoAtual < 0;

        // Movimento horizontal com checagem de colisão
        if (h != 0)
        {
            Vector3 novaPosX = new Vector3(
                transform.position.x + h * velocidade * Time.deltaTime,
                transform.position.y,
                transform.position.z
            );

            if (!ChecarColisao(novaPosX))
                transform.position = novaPosX;
        }

        // Movimento vertical (W/S) com checagem de colisão
        if (!pulando && v != 0)
        {
            float novoY = groundY + v * velocidade * 0.4f * Time.deltaTime;
            novoY = Mathf.Clamp(novoY, limiteYbaixo, limiteYcima);

            Vector3 novaPosY = new Vector3(
                transform.position.x,
                novoY,
                transform.position.z
            );

            if (!ChecarColisao(novaPosY))
            {
                groundY = novoY;
                transform.position = novaPosY;
            }
        }
    }

    private IEnumerator Pular()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsTag("Movimento"))
            yield break;

        pulando = true;
        float tempo = 0f;

        while (tempo < duracaoPulo)
        {
            tempo += Time.deltaTime;
            float progresso = tempo / duracaoPulo;
            float offsetY = Mathf.Sin(progresso * Mathf.PI) * alturaPulo;
            transform.position = new Vector3(
                transform.position.x,
                groundY + offsetY,
                transform.position.z
            );
            yield return null;
        }

        transform.position = new Vector3(transform.position.x, groundY, transform.position.z);
        pulando = false;
    }

    private bool ChecarColisao(Vector3 novaPosicao)
    {
        if (corpoCollider == null) return false;

        Collider2D hit = Physics2D.OverlapBox(
            novaPosicao + (Vector3)corpoCollider.offset,
            corpoCollider.size * 0.9f,
            0f,
            layerObstaculos
        );

        return hit != null;
    }

    public float GetDirecao() => direcaoAtual;
}