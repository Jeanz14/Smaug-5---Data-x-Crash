using UnityEngine;
using System.Collections.Generic;

public class HitboxController : MonoBehaviour
{
    private List<Collider2D> inimigosNaArea = new List<Collider2D>();

    public void AplicarDano(float especialPorGolpe, int dano, float tempoHitStun)
    {
        
        List<Collider2D> copia = new List<Collider2D>(inimigosNaArea);

        foreach (Collider2D col in copia)
        {
            
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
                normal.ReceberGolpe(dano, tempoHitStun);
                GameManager.Instance.AdicionarEspecial(especialPorGolpe);
                GameManager.Instance.AdicionarCombo();
            }
        }

        
        inimigosNaArea.RemoveAll(col => col == null);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Inimigo")||other.CompareTag("quebravel") && !inimigosNaArea.Contains(other))
        {
            inimigosNaArea.Add(other);
        }
            
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (inimigosNaArea.Contains(other))
            inimigosNaArea.Remove(other);
    }
}
