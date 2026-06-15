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
    [SerializeField] private TMP_Text txtVidas;

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

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerAnim = player.GetComponent<Animator>();
        }
        else
        {
            Debug.LogError("GameManager: nenhum objeto com a tag 'Player' foi encontrado na cena.");
        }
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
        if (playerAnim.GetCurrentAnimatorStateInfo(0).IsTag("Ataque") || playerAnim.GetCurrentAnimatorStateInfo(0).IsTag("HitStun"))
        {
            return false;
        }
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

    private void AtualizarTxtVidas()
    {
        if (txtVidas != null)
            txtVidas.text = vidas.ToString();
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
        AtualizarBarrasEspecial();
        AtualizarBarraVida();
        AtualizarTxtVidas();
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
            Renascer();
            return;
        }

        AtualizarHUD();
        //Corrigir depois o hitstun
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
        //Tocar algum efeito/som de ataque acertado no prota
    }

    public void Morrer()
    {
        //e mudar de acordo a logica do gameover jean
        DeathMenuController.Instance.AtivarMenuMorte();
        //Tocar algum efeito/som de morte do prota se tiver
    }

    private void Renascer()
    {
        if (vidas > 0)
        {
            vidas--;
            life = maxLife;
            AtualizarHUD();
        }
        else
        {
            AnimatorStateInfo stateInfo = playerAnim.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsTag("Morte")) return;
            playerAnim.SetTrigger("Morte");
        }
    }
}