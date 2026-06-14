using UnityEngine;

public class ComportamentoHostil : MonoBehaviour
{
    public enum tipoInimigo : int
    {
        Fraco,
        Elite
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
    [SerializeField] private float velocidade = 5f;
    [SerializeField] private float direcaoAtual = 1f;
    [SerializeField] private Vector2 alvo;
    [SerializeField] private int dano = 10;
    [SerializeField] private bool podeAtacar = false;
    [SerializeField] private Collider2D hitbox;
    private Animator anim;
    [SerializeField] private Transform posPlayer;
    private SpriteRenderer sr;
    void Awake()
    {
        anim = GetComponent<Animator>();
        posPlayer = GameObject.FindGameObjectWithTag("Player").transform;
        sr = GetComponent<SpriteRenderer>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
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
        if(hitbox.IsTouching(GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>()))
        {
            anim.SetInteger("IFState", (int)InimigoState.Atacando);
        }
    }
    public void AtaqueCompleto()
    {
        if (podeAtacar)
        {
            Debug.Log("Inimigo atacou o player");
            GameManager.Instance.PlayerApanhou(dano);
        }
    }
    void Update()
    {
        Andar();
    }
    void Andar()
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
                hitbox.offset = new Vector2(-4.5f*direcaoAtual, -4.5f);//pessima pratica, corrigir depois
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
                hitbox.offset = new Vector2(-4.5f*direcaoAtual, -4.5f);//pessima pratica, corrigir depois
            }
            sr.flipX = true;
        }
        transform.position += (Vector3)(alvo * velocidade * Time.deltaTime);
        //Se tiver som de andar, tocar aqui
    }
    
}
