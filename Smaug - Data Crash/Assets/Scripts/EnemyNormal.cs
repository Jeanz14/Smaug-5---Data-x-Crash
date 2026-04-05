using UnityEngine;

public class EnemyNormal : MonoBehaviour
{
    [SerializeField] private int pontosAoDerrotar = 100;
    private int vidaAtual = 5;
    public GameObject recompensa = null;

    public void ReceberGolpe(int dano, float tempoHitStun)
    {
        vidaAtual-=dano;
        Debug.Log("Inimigo normal: " + vidaAtual + " vidas restantes");

        if (vidaAtual <= 0)
            Nocautear();
    }

    private void Nocautear()
    {
        GameManager.Instance.AdicionarPontos(pontosAoDerrotar);
        Debug.Log("Inimigo nocauteado! +" + pontosAoDerrotar + " pontos");
        //adicionar firula de animação da morte/destruição
        
        Destroy(gameObject);
    }
    void OnDestroy()
    {
        if(recompensa != null)
        {
            Instantiate(recompensa, transform.position, Quaternion.identity);
        }
    }
}