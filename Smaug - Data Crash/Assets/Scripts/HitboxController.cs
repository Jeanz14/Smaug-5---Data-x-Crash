using UnityEngine;
using System.Collections.Generic;

public class HitboxController : MonoBehaviour
{
    private List<Collider2D> inimigosNaArea = new List<Collider2D>();

    public void AplicarDano(float especialPorGolpe)
    {
        // Cria uma cópia da lista para iterar com segurança
        List<Collider2D> copia = new List<Collider2D>(inimigosNaArea);

        foreach (Collider2D col in copia)
        {
            // Remove entradas nulas (inimigos destruídos)
            if (col == null)
            {
                inimigosNaArea.Remove(col);
                continue;
            }

            EnemyImortal imortal = col.GetComponent<EnemyImortal>();
            if (imortal != null)
            {
                imortal.ReceberGolpe();
                GameManager.Instance.AdicionarEspecial(especialPorGolpe);
                continue;
            }

            EnemyNormal normal = col.GetComponent<EnemyNormal>();
            if (normal != null)
            {
                normal.ReceberGolpe();
                GameManager.Instance.AdicionarEspecial(especialPorGolpe);
            }
        }

        // Limpa entradas nulas da lista original após o loop
        inimigosNaArea.RemoveAll(col => col == null);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Inimigo") && !inimigosNaArea.Contains(other))
            inimigosNaArea.Add(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (inimigosNaArea.Contains(other))
            inimigosNaArea.Remove(other);
    }
}
