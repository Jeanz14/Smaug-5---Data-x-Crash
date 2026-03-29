using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float velocidade = 5f;

    [Header("Pulo")]
    [SerializeField] private float alturaPulo = 2f;
    [SerializeField] private float duracaoPulo = 0.4f;

    [Header("Limites W/S")]
    [SerializeField] private float limiteYcima = 1f;
    [SerializeField] private float limiteYbaixo = -2f;

    [Header("Hitbox")]
    [SerializeField] private Transform hitboxTransform;

    private SpriteRenderer sr;
    private bool pulando = false;
    private float groundY;
    private float direcaoAtual = 1f; // 1 = direita, -1 = esquerda

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
            sr = GetComponentInChildren<SpriteRenderer>();
        if (sr == null)
            Debug.LogError("SpriteRenderer não encontrado!");

        if (hitboxTransform == null)
            Debug.LogError("HitboxTransform não atribuído no Inspector!");

        groundY = transform.position.y;
    }

    void Update()
    {
        Mover();

        if (Input.GetKeyDown(KeyCode.Space) && !pulando)
            StartCoroutine(Pular());
    }

    private void Mover()
    {
        float h = 0f;
        float v = 0f;

        if (Input.GetKey(KeyCode.A)) h = -1f;
        if (Input.GetKey(KeyCode.D)) h = 1f;
        if (Input.GetKey(KeyCode.W)) v = 1f;
        if (Input.GetKey(KeyCode.S)) v = -1f;

        // Atualiza direção só quando há input horizontal
        if (h > 0) direcaoAtual = 1f;
        if (h < 0) direcaoAtual = -1f;

        // Flip do sprite
        if (sr != null)
        {
            sr.flipX = direcaoAtual < 0;
        }

        // Hitbox sempre segue a direção, mesmo parado
        if (hitboxTransform != null)
        {
            hitboxTransform.localPosition = new Vector3(0.6f * direcaoAtual, 0f, 0f);
        }

        // Movimento horizontal
        float novoX = transform.position.x + h * velocidade * Time.deltaTime;
        transform.position = new Vector3(novoX, transform.position.y, transform.position.z);

        // Movimento W/S
        if (!pulando && v != 0)
        {
            float novoY = groundY + v * velocidade * 0.4f * Time.deltaTime;
            groundY = Mathf.Clamp(novoY, limiteYbaixo, limiteYcima);
            transform.position = new Vector3(transform.position.x, groundY, transform.position.z);
        }
    }

    private IEnumerator Pular()
    {
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

    // Expõe a direção para outros scripts (PlayerCombat etc)
    public float GetDirecao() => direcaoAtual;
}
