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
        anim = GetComponent<Animator>();
        if (spawnarInimigo == null && recompensa == null) 
        {
            spawnarInimigo = GameObject.FindAnyObjectByType<SpawnarInimigo>();
        }
    }
    public void ReceberGolpe(int dano, float tempoHitStun)
    {
        vidaAtual-=dano;
        Debug.Log("Inimigo normal: " + vidaAtual + " vidas restantes");

        if (vidaAtual <= 0)
        {
            if (anim == null) { 
                Nocautear(); 
                return;
            }
            anim.SetTrigger("Morte");
            return;
        }
        if (anim == null) return;
        if (tempoHitStun < 0.2f)
        {
            anim.SetTrigger("HitStun");
            anim.SetInteger("IFState", 3);
        }
        else if (tempoHitStun >= 0.2f)
        {
            anim.SetTrigger("HitStun");
            anim.SetInteger("IFState", 4);
        }

    }

    private void Nocautear()
    {
        GameManager.Instance.AdicionarPontos(pontosAoDerrotar);
        Debug.Log("Inimigo nocauteado! +" + pontosAoDerrotar + " pontos");
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