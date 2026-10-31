using UnityEngine;

public class ComportamentoHostil : MonoBehaviour
{
    public enum tipoInimigo : int
    {
        Fraco,
        Elite,
        Bombeiro,
        Boss
    }
    [SerializeField] private tipoInimigo tipo;
    public enum InimigoState : int
    {
        Idle = 0, 
        Andando = 1,
        Atacando = 2,
        ApanhandoUm = 3,
        ApanhandoDois = 4,
        Morreu = 5
    }
    [SerializeField] private float timer = 0f;
    [SerializeField] private float timerEsperaAndar = 2f;
    [SerializeField] private float timerEsperaAtaque = 5f;
    [SerializeField] private bool cooldown = false;
    [SerializeField] private GameObject projetil;
    [SerializeField] private float velocidade = 5f;
    [SerializeField] private float direcaoAtual = 1f;
    [SerializeField] private Vector2 alvo;
    [SerializeField] private int dano = 10;
    [SerializeField] private bool podeAtacar = false;
    [SerializeField] private Collider2D hitbox;
    private Animator anim;
    [SerializeField] private Transform posPlayer;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    void Awake()
    {
        if (tipo == tipoInimigo.Bombeiro) return;
        anim = GetComponent<Animator>();
        posPlayer = GameObject.FindGameObjectWithTag("Player").transform;
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (tipo == tipoInimigo.Bombeiro) return;
        if (cooldown) return;
        if(!other.CompareTag("Player")) return;
        if (hitbox.IsTouching(other))
        {
            Debug.Log("Player agrou o inimigo");
            podeAtacar = true;
            anim.SetInteger("IFState", (int)InimigoState.Atacando);
            alvo = transform.position;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (tipo == tipoInimigo.Bombeiro) return;
        if(!other.CompareTag("Player")) return;
        if (!hitbox.IsTouching(other))
        {
            Debug.Log("Player desagrou o inimigo");
            podeAtacar = false;
            anim.SetInteger("IFState", (int)InimigoState.Idle);
        }
    }
    public void ChecarRange()
    {
        if (cooldown) return;
        if(!hitbox.IsTouching(GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>())) return;
        if(tipo == tipoInimigo.Bombeiro) 
        {
            if(transform.position.x-500f >= posPlayer.position.x || transform.position.x+500f <= posPlayer.position.x)
            {
                podeAtacar = false;
                return;
            }
            else
            {
                podeAtacar = true;
            }
        }
        anim.SetInteger("IFState", (int)InimigoState.Atacando);
    }
    public bool Timer()
    {
        if (cooldown)
        {
            timer += Time.deltaTime;
            if (timer >= timerEsperaAtaque)
            {
                ResetCooldown();
                return true;
            }
            if (timer < timerEsperaAndar) 
            {
                anim.SetInteger("IFState", (int)InimigoState.Idle);
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return true;
        }
    }
    public void SetCooldown()
    {
        timer = 0f;
        cooldown = true;
    }
    public void ResetCooldown()
    {
        timer = 0f;
        cooldown = false;
    }
    public void AtaqueCompleto()
    {
        if (podeAtacar)
        {
            Debug.Log("Inimigo atacou o player");
            GameManager.Instance.PlayerApanhou(dano, (int)tipo, -direcaoAtual);
        }
    }
    public void SpawnarProjeril()
    {
        if (projetil == null) return;
        GameObject proj = Instantiate(projetil, transform.position, Quaternion.identity);
        proj.GetComponent<Projetil>().dano = dano;
    }

    void Update()
    {
        ChecarRange();
    }
    void FixedUpdate()
    {
        if(Timer()) Andar();
    }
    void Andar()
    {
        if (tipo == tipoInimigo.Fraco || tipo == tipoInimigo.Elite)
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if ((!stateInfo.IsTag("Movimento"))|| podeAtacar)
            {
                return;
            }
            anim.SetInteger("IFState", (int)InimigoState.Andando);
            alvo = (posPlayer.position - transform.position).normalized;
            if (alvo.x > 0)
            {
                direcaoAtual = -1f;
                if (tipo == tipoInimigo.Fraco)
                {
                    hitbox.offset = new Vector2(-3f*direcaoAtual, -4.5f);//pessima pratica, corrigir depois
                }
                else if (tipo == tipoInimigo.Elite)
                {
                    hitbox.offset = new Vector2(-4.5f*direcaoAtual, -4f);//pessima pratica, corrigir depois
                }
                sr.flipX = false;
            }
            else if (alvo.x < 0)
            {
                direcaoAtual = 1f;
                if (tipo == tipoInimigo.Fraco)
                {
                    hitbox.offset = new Vector2(-3f*direcaoAtual, -4.5f);//pessima pratica, corrigir depois
                }
                else if (tipo == tipoInimigo.Elite)
                {
                    hitbox.offset = new Vector2(-4.5f*direcaoAtual, -4f);//pessima pratica, corrigir depois
                }
                sr.flipX = true;
            }
            rb.MovePosition(rb.position+(Vector2)(alvo * velocidade * Time.deltaTime));
            //Se tiver som de andar, tocar aqui
        }
        else if (tipo == tipoInimigo.Bombeiro)
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if ((!stateInfo.IsTag("Movimento")) || podeAtacar)
            {
                return;
            }
            anim.SetInteger("IFState", (int)InimigoState.Andando);
            float distancia = Mathf.Abs(rb.position.x - posPlayer.position.x);
            if(distancia >= 0)
            {
                direcaoAtual = 1f;
                sr.flipX = true;
            }
            else
            {
                direcaoAtual = -1f;
                sr.flipX = false;
            }
            // Graus de pertinência fuzzy
            float afastar = Mathf.Clamp01((200f - distancia) / 100f);
            float aproximar = Mathf.Clamp01((distancia - 300f) / 100f);

            Vector2 direcaoAfastar = new Vector2(Mathf.Sign(rb.position.x - posPlayer.position.x), 0f);

            Vector2 direcaoAproximar = new Vector2(Mathf.Sign(posPlayer.position.x - rb.position.x), 0f);

            // Combinação dos comportamentos fuzzy
            Vector2 movimento = direcaoAfastar * afastar + direcaoAproximar * aproximar;

            if (movimento.sqrMagnitude > 1f) movimento.Normalize();

            rb.MovePosition(rb.position + movimento * velocidade * Time.fixedDeltaTime);
        }
    }
    
}
