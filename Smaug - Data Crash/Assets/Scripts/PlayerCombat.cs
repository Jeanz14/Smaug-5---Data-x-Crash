using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Hitbox")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BoxCollider2D hitboxCollider;
    [SerializeField] private HitboxController hitbox;

    [Header("Especializacao do golpe")]
    [SerializeField] private float especialPorGolpe = 0.2f;
    [SerializeField] private int dano = 1;
    [SerializeField] private float tempoHitStun = 0.1f;

    [Header("Animator")]
    [SerializeField] private Animator anim;

    [Header("Restricoes")]
    [SerializeField] private bool podeAtacarLeve = true;
    [SerializeField] private bool podeAtacarPesado = true;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Atacar();
        if (Input.GetKeyDown(KeyCode.H)) UsarEspecial();
    }
    private void Atacar()
    {
        //Ataque leve
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (hitbox == null)
            {
                Debug.LogError("Hitbox n�o atribu�da no PlayerCombat!");
                return;
            }
            if(podeAtacarLeve)
            {
                podeAtacarPesado = false;
                anim.SetInteger("PlayerState", 4);
                anim.SetBool("ContinuarCombo", true);
            }
            

            
            Debug.Log("Ataque Leve executado!");
        }
        //Ataque Pesado
        if (Input.GetKeyDown(KeyCode.U))
        {
            if(podeAtacarPesado)
            {
                if(GameManager.Instance.UsarGolpePesado()){
                    podeAtacarLeve = false;
                    anim.SetInteger("PlayerState", 5);
                }
            }
        }

    }

    private void UsarEspecial()
    {
        if (GameManager.Instance.UsarEspecial())
            Debug.Log("ESPECIAL ATIVADO!");
        else
            Debug.Log("Sem barra especial suficiente.");
    }
    private void Fim()
    {
        anim.SetInteger("PlayerState", 0);
        anim.SetBool("ContinuarCombo", false);
    }
    private void Resetar()
    {
        if(anim.GetBool("ContinuarCombo") && !anim.GetCurrentAnimatorStateInfo(0).IsName("AtaqueLeveP3")) return;
        podeAtacarLeve = true;
        podeAtacarPesado = true;
        anim.SetInteger("PlayerState", 0);
    }
    private void Danificar(int golpe)
    {
        switch (golpe)
        {
            case 1: 
                hitboxCollider.offset = new Vector2(2.9f * playerController.GetDirecao(), 2f);
                hitboxCollider.size = new Vector2(4.5f, 4.4f);
                break;
            case 2: 
                hitboxCollider.offset = new Vector2(2.4f * playerController.GetDirecao(), -2.5f);
                hitboxCollider.size = new Vector2(3.5f, 4.4f);
                break;
            case 3: 
                hitboxCollider.offset = new Vector2(3f * playerController.GetDirecao(), 0f);
                hitboxCollider.size = new Vector2(4.8f, 6f);
                break;
            default:
                break;
        }
        hitbox.AplicarDano(especialPorGolpe, dano, tempoHitStun);
    }

}