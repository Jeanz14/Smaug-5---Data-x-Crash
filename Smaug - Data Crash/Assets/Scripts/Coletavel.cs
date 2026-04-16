using UnityEngine;

public class Coletavel : MonoBehaviour
{
    public int cura = 10;
    [SerializeField] private bool pegavel = false;
    void OntriggerEnter2D(Collider2D other)
    {
        Debug.Log("Player entrou em range");
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entrou em range de verdade");
            pegavel = true;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Player saiu do range");
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player saiu do range de verdade");
            pegavel = false;
        }
    }
    void Update()
    {
        if (pegavel && Input.GetKeyDown(KeyCode.E))
        {
            GameManager.Instance.PlayerApanhou(-cura);
            Destroy(gameObject);
        }
    }
}
