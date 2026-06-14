using UnityEngine;

public class PlayerCombat : MonoBehaviour
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
    [Header("Animacao da Ult")]
    [SerializeField] private Vector3 posAtual;
    [SerializeField] private float duracaoUlt = 0.5f;
    [SerializeField] private float alturaPuloUlt = 100f;
    [SerializeField] private float distanciaPuloUlt = 500f;
    public bool puloDaUlt = false;
    private float timerDaUlt;

    

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Atacar();
        UpdateDaUlt();
    }
    private void Atacar()
    {
        //Ataque leve
        if (Input.GetKeyDown(KeyCode.J))
        {
            anim.SetInteger("PlayerState", (int)Acao.AtacarLeve);
            anim.SetBool("ContinuarCombo", true);
        }
        //Ataque Pesado
        if (Input.GetKeyDown(KeyCode.U))
        {
            anim.SetInteger("PlayerState", (int)Acao.AtacarForte);
            anim.SetBool("ContinuarComboPesado", true);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            Especial();
        }
    }

    private void Especial()
    {
        if (GameManager.Instance.UsarEspecial())
        {
            anim.SetInteger("PlayerState", (int)Acao.AtaqueEspecial);
        }
    }
    private void Morte()
    {
        GameManager.Instance.Morrer();
    }
    private void Fim()
    {
        anim.SetInteger("PlayerState", (int)Acao.Idle);
        anim.SetBool("ContinuarCombo", false);
        anim.SetBool("ContinuarComboPesado", false);
    }
    private void Resetar()
    {
        anim.SetInteger("PlayerState", (int)Acao.Idle);
    }
    private void Danificar(int golpe)
    {
        dano = 1;
        tempoHitStun = 0.1f;
        especialPorGolpe = 0.2f;
        switch (golpe)
        {
            case 0:
                hitboxCollider.offset = new Vector2(5f * playerController.GetDirecao(), 0f);
                hitboxCollider.size = new Vector2(3f, 1f);
                break;
            case 1: 
                hitboxCollider.offset = new Vector2(2.9f * playerController.GetDirecao(), 0f);
                hitboxCollider.size = new Vector2(4.5f, 1f);
                break;
            case 2: 
                hitboxCollider.offset = new Vector2(2.4f * playerController.GetDirecao(), 0f);
                hitboxCollider.size = new Vector2(3.5f, 1f);
                break;
            case 3: 
                hitboxCollider.offset = new Vector2(3f * playerController.GetDirecao(), 0f);
                hitboxCollider.size = new Vector2(4.8f, 1f);
                break;
            case 4:
                especialPorGolpe = 0f;
                tempoHitStun = 2f;
                hitboxCollider.offset = new Vector2(0f * playerController.GetDirecao(), 0f);
                hitboxCollider.size = new Vector2(9f, 5f);
                break;
            case 5:
                dano = 5;
                tempoHitStun = playerController.GetDirecao();
                especialPorGolpe = 0f;
                hitboxCollider.offset = new Vector2(0f * playerController.GetDirecao(), 0f);
                hitboxCollider.size = new Vector2(9f, 5f);
                break;
            default:
                break;
        }
        hitbox.AplicarDano(especialPorGolpe, dano, tempoHitStun);
    }
    private void InicioDaUlt()
    {
        posAtual = transform.position;
        puloDaUlt = true;
        timerDaUlt = 0f;
    }
    private void UpdateDaUlt()
    {
        if (puloDaUlt)
        {
            timerDaUlt += Time.deltaTime;
            float progresso = timerDaUlt / duracaoUlt;
            if (progresso > 1f) progresso = 1f;
            float x = distanciaPuloUlt * progresso;
            float y = 4 * alturaPuloUlt * progresso * (1 - progresso); //formula para criar um movimento parabólico matematica não é inutil
            transform.position = posAtual + new Vector3(x * playerController.GetDirecao(), y, 0f);
        }
    }
    private void FimDaUlt()
    {
        puloDaUlt = false;
        timerDaUlt = 0f;
    }
}