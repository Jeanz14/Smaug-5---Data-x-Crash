using UnityEngine;

public class Projetil : MonoBehaviour
{
    [SerializeField] private float timer = 0f;
    [SerializeField] private float duracao = 5f;
    [SerializeField] private Vector2 direcao;
    public float velocidade = 5f;
    public int dano = 50;
    public Vector2 alvo;


    void Awake()
    {
        alvo = GameObject.FindGameObjectWithTag("Player").transform.position;
        direcao = (alvo - new Vector2(transform.position.x, transform.position.y)).normalized;
    }

    void Update()
    {
        if (timer > duracao)
        {
            Destroy(gameObject);
            return;
        }
        timer += Time.deltaTime;

    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;
        GameManager.Instance.PlayerApanhou(dano);
        Destroy(gameObject);
    }
}
