using UnityEngine;

public class ComportamentoHostil : MonoBehaviour
{
    public enum InimigoState : int
    {
        Idle = 0, 
        Andando = 1, 
        Atacando = 2
    }
    [SerializeField] private float velocidade = 3f;
    [SerializeField] private int dano = 10;
    [SerializeField] private bool podeAtacar = true;
    [SerializeField] private Collider2D hitbox;
    private Animator anim;
    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;
        if (hitbox.IsTouching(other))
        {
            Debug.Log("Player agrou o inimigo");
            podeAtacar = true;
            anim.SetInteger("IFState", (int)InimigoState.Atacando);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;
        if (!hitbox.IsTouching(other))
        {
            Debug.Log("Player desagrou o inimigo");
            podeAtacar = false;
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

}
