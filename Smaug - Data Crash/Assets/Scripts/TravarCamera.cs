using UnityEngine;
using Cinemachine;

public class TravarCamera : MonoBehaviour
{
    [SerializeField] private GameObject[] inimigosFraco;
    [SerializeField] private GameObject[] inimigosElite;
    [SerializeField] private Collider2D[] barreiras;
    [SerializeField] private Collider2D limiteDaCameraTemp;
    public Collider2D LimiteDaCameraPadrao;
    public CinemachineConfiner confiner;
    private bool ativado = false;

    void OnTriggerEnter2D(Collider2D other)
    {
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
        }
        foreach (GameObject inimigoElite in inimigosElite)
        {
            inimigoElite.SetActive(true);
        }
        //tocar algum efeito/som de Combate iniciado, se tiver
    }
    void Update()
    {
        if (inimigosFraco.Length == 0 && inimigosElite.Length == 0) FimDoCombate();
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
