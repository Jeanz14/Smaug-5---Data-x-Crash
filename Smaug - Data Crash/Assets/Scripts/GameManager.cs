using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("HUD")]
    [SerializeField] private Image barra1;
    [SerializeField] private Image barra2;
    [SerializeField] private TMP_Text txtPlacar;
    [SerializeField] private TMP_Text txtVidas;
    [SerializeField] private TMP_Text txtLife;

    [Header("Dados")]
    private int placar = 0;
    private int maxLife = 100;
    private int life = 100; //não confundir com vidas (desculpa eu n tenho criatividade e achei engraçado, te dou todo direito de trocar o termo)
    private int ghostLife = 0;
    private int vidas = 2;
    private const int MAX_VIDAS = 3;

    // Especial: valor entre 0 e 2 (cada unidade = 1 barra cheia)
    private float especial = 0f;
    private const float MAX_ESPECIAL = 2f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        AtualizarHUD();
    }

    // ── Especial ──────────────────────────────────────────
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

    // ── Placar ────────────────────────────────────────────
    public void AdicionarPontos(int pontos)
    {
        placar += pontos;
        txtPlacar.text = placar.ToString("D6"); // ex: 001500
    }

    // ── Vidas ─────────────────────────────────────────────
    public void AdicionarVida()
    {
        vidas = Mathf.Min(vidas + 1, MAX_VIDAS);
        AtualizarHUD();
    }

    private void AtualizarHUD()
    {
        txtVidas.text = "Vidas: " + vidas;
        txtPlacar.text = placar.ToString("D6");
        txtLife.text = "Saúde: " + life;
        AtualizarBarrasEspecial();
    }
    //── life ───────────────────────────────────────────────
    public bool UsarGolpePesado()
    {
        if (ghostLife > life-10)
        {
            Debug.Log("Vai dar não chefe");
            return false;
        }
        ghostLife+=5;
        return true;
    }
    public void PlayerApanhou(int dano)
    {
        if (dano < 0)
        {
            life = Mathf.Min(life - dano, maxLife);
            AtualizarHUD();
            return;
        }
        //a ser adicionado mecanica de hitstun e invulnerabilidade
        life -= dano+ghostLife;
        if(life <= 0){
            if (vidas>0)
            {
                vidas--;
                Renascer();//a ser criado
            }
            else
            {
                Morrer();//a ser criado
            }
        }
        AtualizarHUD();
    }
    private void Morrer()
    {
        return;
    }
    private void Renascer()
    {
        return;
    }
}