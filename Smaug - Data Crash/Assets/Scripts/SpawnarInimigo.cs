using UnityEngine;

public class SpawnarInimigo : MonoBehaviour
{
    [SerializeField] private GameObject inimigo;
    
    public void Spawnar()
    {
        Instantiate(inimigo, transform.position, Quaternion.identity);
    }
    
}
