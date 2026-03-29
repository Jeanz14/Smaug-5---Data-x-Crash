using UnityEngine;

public class EnemyNormal : MonoBehaviour
{
    [SerializeField] private int pontosAoDerrotar = 100;
    private int vidaAtual = 5;

    public void ReceberGolpe()
    {
        vidaAtual--;
        Debug.Log("Inimigo normal: " + vidaAtual + " vidas restantes");

        if (vidaAtual <= 0)
            Nocautear();
    }

    private void Nocautear()
    {
        GameManager.Instance.AdicionarPontos(pontosAoDerrotar);
        Debug.Log("Inimigo nocauteado! +" + pontosAoDerrotar + " pontos");
        Destroy(gameObject);
    }
}