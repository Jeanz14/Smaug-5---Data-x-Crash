using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("HUD")]
    [SerializeField] private Image barra1;
    [SerializeField] private Image barra2;
    [SerializeField] private Image barraVida;
    [SerializeField] private TMP_Text txtPlacar;
    [SerializeField] private TMP_Text txtLife;

    [Header("Corações")]
    [SerializeField] private GameObject coracao1;
    [SerializeField] private GameObject coracao2;
    [SerializeField] private GameObject coracao3;

    [Header("Dados")]
    private int placar = 0;
    private int combo = 0;

    [Header("Player")]
    private int maxLife = 100;
    private int life = 100;
    private int vidas = 2;
    private const int MAX_VIDAS = 3;
    private Animator playerAnim;

    private float especial = 0f;
    private const float MAX_ESPECIAL = 2f;

    void Awake()
    {
        Instance = this;
        playerAnim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
    }

    void Start()
    {
        AtualizarHUD();
    }

    public void AdicionarEspecial(float quantidade)
    {
        especial = Mathf.Clamp(especial + quantidade, 0f, MAX_ESPECIAL);
        AtualizarBarrasEspecial();
    }

    public bool UsarEspecial()
    {
        if (especial < 1f) return false;
        especial -= 1f;
        AtualizarBarrasEspecial();
        return true;
    }

    private void AtualizarBarrasEspecial()
    {
        barra1.fillAmount = Mathf.Clamp01(especial);
        barra2.fillAmount = Mathf.Clamp01(especial - 1f);
    }

    private void AtualizarBarraVida()
    {
        if (barraVida != null)
            barraVida.fillAmount = (float)life / maxLife;
    }

    private void AtualizarCoracoes()
    {
        if (coracao1 != null) coracao1.SetActive(vidas >= 1);
        if (coracao2 != null) coracao2.SetActive(vidas >= 2);
        if (coracao3 != null) coracao3.SetActive(vidas >= 3);
    }

    public void AdicionarPontos(int pontos)
    {
        placar += pontos * Mathf.Max(1, Mathf.Min((combo / 10), 5));
        txtPlacar.text = placar.ToString("D6");
    }

    public void ResetarCombo()
    {
        combo = 0;
        AtualizarHUD();
    }

    public void AdicionarCombo()
    {
        combo++;
        AtualizarHUD();
    }

    public void AdicionarVida()
    {
        vidas = Mathf.Min(vidas + 1, MAX_VIDAS);
        AtualizarHUD();
    }

    private void AtualizarHUD()
    {
        txtPlacar.text = placar.ToString("D6");
        txtLife.text = "Saúde: " + life;
        AtualizarBarrasEspecial();
        AtualizarBarraVida();
        AtualizarCoracoes();
    }

    public void PlayerApanhou(int dano)
    {
        if (dano < 0)
        {
            life = Mathf.Min(life - dano, maxLife);
            AtualizarHUD();
            return;
        }

        life -= dano;
        ResetarCombo();

        if (life <= 0)
        {
            if (vidas > 0)
            {
                vidas--;
                Renascer();
            }
            else
            {
                Morrer();
            }
            return;
        }

        AtualizarHUD();

        if (playerAnim.GetBool("StunImune"))
        {
            playerAnim.SetBool("StunImune", false);
            return;
        }
        else
        {
            playerAnim.SetBool("StunImune", true);
        }

        playerAnim.SetTrigger("HitStun");
    }

    private void Morrer()
    {
        SceneDatabase.cenaAntesDaMorte = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("GameOver");
    }

    private void Renascer()
    {
        life = maxLife;
        AtualizarHUD();
    }
}