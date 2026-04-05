using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Hitbox")]
    [SerializeField] private HitboxController hitbox;

    [Header("Especializacao do golpe")]
    [SerializeField] private float especialPorGolpe = 0.2f;
    [SerializeField] private int dano = 1;
    [SerializeField] private float tempoHitStun = 0.1f;

    void Update()
    {
        Atacar();
        if (Input.GetKeyDown(KeyCode.H)) UsarEspecial();
    }
    private void Atacar()
    {
        //Ataque leve
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (hitbox == null)
            {
                Debug.LogError("Hitbox n�o atribu�da no PlayerCombat!");
                return;
            }

            hitbox.AplicarDano(especialPorGolpe, dano, tempoHitStun);
            Debug.Log("Ataque Leve executado!");
        }
        //Ataque Pesado
        if (Input.GetKeyDown(KeyCode.U))
        {
            //animação a ser feita
            if(GameManager.Instance.UsarGolpePesado()){
                hitbox.AplicarDano(especialPorGolpe*2, dano*2, tempoHitStun*2);
            }
        }

    }

    private void UsarEspecial()
    {
        if (GameManager.Instance.UsarEspecial())
            Debug.Log("ESPECIAL ATIVADO!");
        else
            Debug.Log("Sem barra especial suficiente.");
    }
}