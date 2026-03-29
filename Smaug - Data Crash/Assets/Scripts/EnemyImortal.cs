using UnityEngine;

public class EnemyImortal : MonoBehaviour
{
    public void ReceberGolpe()
    {
        // Nunca morre — apenas dá feedback visual futuro
        Debug.Log("Inimigo imortal recebeu golpe!");
    }
}