using UnityEngine;

public class EnemyNormal : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private SpawnarInimigo spawnarInimigo = null; 
    [SerializeField] private int pontosAoDerrotar = 100;
    private int vidaAtual = 5;
    public GameObject recompensa = null;
    void Awake()
    {
        if(recompensa != null) // deletar depois que o artista desenhar a caixa apanhando
        {
            return;
        }
        anim = GetComponent<Animator>();
    }
    public void ReceberGolpe(int dano, float tempoHitStun)
    {
        vidaAtual-=dano;
        Debug.Log("Inimigo normal: " + vidaAtual + " vidas restantes");

        if (vidaAtual <= 0)
        {
            Nocautear();
            return;
        }
        if (tempoHitStun < 0.2f)
        {
            anim.Play("IF_Apanhando1", -1, 0f);
        }
        else if (tempoHitStun >= 0.2f)
        {
            anim.Play("IF_Apanhando2", -1, 0f);
        }

    }

    private void Nocautear()
    {
        GameManager.Instance.AdicionarPontos(pontosAoDerrotar);
        Debug.Log("Inimigo nocauteado! +" + pontosAoDerrotar + " pontos");
        //adicionar firula de animação da morte/destruição
        if(recompensa != null)
        {
            Instantiate(recompensa, transform.position, Quaternion.identity);
        }
        if(spawnarInimigo != null)
        {
            spawnarInimigo.Spawnar();
        }
        Destroy(gameObject);
    }
}