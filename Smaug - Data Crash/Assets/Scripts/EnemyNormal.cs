using UnityEngine;

public class EnemyNormal : MonoBehaviour
{
    public enum tipoInimigo : int
    {
        Fraco,
        Elite
    }
    [SerializeField] private tipoInimigo tipo;
    private Animator anim;
    public TravarCamera spawner = null; 
    [SerializeField] private int pontosAoDerrotar = 100;
    private int vidaAtual = 5;
    public GameObject recompensa = null;
    private SpriteRenderer sr;
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sortingOrder = Mathf.RoundToInt(transform.position.y * SceneDatabase.divisorDeCamada);
        anim = GetComponent<Animator>();
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
        if(spawner != null)
        {
            if (tipo == tipoInimigo.Fraco)
            {
                spawner.inimigosFracoMortos++;
            }
            else if (tipo == tipoInimigo.Elite)
            {
                spawner.inimigosEliteMortos++;
            }
        }
        Destroy(gameObject);
    }
}