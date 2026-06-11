using UnityEngine;
using Cinemachine;

public class TravarCamera : MonoBehaviour
{
    [SerializeField] private GameObject[] inimigosFraco;
    public int inimigosFracoMortos = 0;
    [SerializeField] private GameObject[] inimigosElite;
    public int inimigosEliteMortos = 0;
    [SerializeField] private Collider2D[] barreiras;
    [SerializeField] private Collider2D limiteDaCameraTemp;
    public Collider2D LimiteDaCameraPadrao;
    public CinemachineConfiner confiner;
    private bool ativado = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))return;
        if (ativado) return;
        ativado = true;
        confiner.m_BoundingShape2D = limiteDaCameraTemp;
        foreach (Collider2D barreira in barreiras)
        {
            barreira.enabled = true;
        }
        foreach (GameObject inimigoFraco in inimigosFraco)
        {
            inimigoFraco.SetActive(true);
            inimigoFraco.GetComponent<EnemyNormal>().spawner = this;
        }
        foreach (GameObject inimigoElite in inimigosElite)
        {
            inimigoElite.SetActive(true);
            inimigoElite.GetComponent<EnemyNormal>().spawner = this;
        }
        //tocar alguma musica/som de Combate iniciado, se tiver
    }
    void Update()
    {
        if (inimigosFraco.Length <= inimigosFracoMortos && inimigosElite.Length <= inimigosEliteMortos)
        {FimDoCombate();}
    }
    public void FimDoCombate()
    {
        confiner.m_BoundingShape2D = LimiteDaCameraPadrao;
        foreach (Collider2D barreira in barreiras)
        {
            barreira.enabled = false;
        }
        //parar de tocar Som ou Musica de Combate
        Destroy(gameObject);
    }
}
