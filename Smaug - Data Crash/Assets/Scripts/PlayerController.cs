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

    [Header("Pulo")]
    [SerializeField] private float alturaPulo = 2f;
    [SerializeField] private float duracaoPulo = 0.4f;

    [Header("Limites W/S")]
    [SerializeField] private float limiteYcima = 1f;
    [SerializeField] private float limiteYbaixo = -2f;

    [Header("Hitbox")]
    [SerializeField] private Transform hitboxTransform;

    [Header("Animator")]
    [SerializeField] private Animator anim;

    private SpriteRenderer sr;
    private bool pulando = false;
    private float groundY;
    private float direcaoAtual = 1f; // 1 = direita, -1 = esquerda

    void Awake()
    {
        anim = GetComponent<Animator>();
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

        
        if (h > 0) direcaoAtual = 1f;
        if (h < 0) direcaoAtual = -1f;

        if(h!=0 || v!=0) {anim.SetInteger("PlayerState", (int)Acao.Andar);}
        else {anim.SetInteger("PlayerState", (int)Acao.Idle);}
        
        if (sr != null)
        {
            sr.flipX = direcaoAtual < 0;
        }

        
        if (hitboxTransform != null)
        {
            hitboxTransform.localPosition = new Vector3(0.6f * direcaoAtual, 0f, 0f);
        }

        if (h != 0)
        {
            float novoX = transform.position.x + h * velocidade * Time.deltaTime;
            transform.position = new Vector3(novoX, transform.position.y, transform.position.z);
        }

        
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

    
    public float GetDirecao() => direcaoAtual;
}
