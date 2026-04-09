using UnityEngine;

public class Coletavel : MonoBehaviour
{
    public int cura = 10;
    private bool pegavel = false;
    void OntriggerEnter2D(Collider2D other)
    {
        Debug.Log("Player entrou em range");
        if (other.CompareTag("Player"))
        {
            pegavel = true;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player saiu do range");
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
