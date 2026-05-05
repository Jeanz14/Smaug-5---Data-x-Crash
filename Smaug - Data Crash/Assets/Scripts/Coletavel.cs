using UnityEngine;

public class Coletavel : MonoBehaviour
{
    public int cura = 10;
    [SerializeField] private bool pegavel = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            pegavel = true;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Player saiu do range");
        if (other.CompareTag("Player"))
        {
            pegavel = false;
        }
    }
    void Update()
    {
        if (pegavel && Input.GetKeyDown(KeyCode.E))
        {
            GameManager.Instance.PlayerApanhou(-cura);
            //Tocar algum efeito de musica vai que é sua Jean
            Destroy(gameObject);
        }
    }
}
