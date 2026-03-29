using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Hitbox")]
    [SerializeField] private HitboxController hitbox;

    [Header("Especial por golpe")]
    [SerializeField] private float especialPorGolpe = 0.2f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) Atacar();
        if (Input.GetKeyDown(KeyCode.H)) UsarEspecial();
    }

    private void Atacar()
    {
        if (hitbox == null)
        {
            Debug.LogError("Hitbox não atribuída no PlayerCombat!");
            return;
        }

        hitbox.AplicarDano(especialPorGolpe);
        Debug.Log("Ataque executado!");
    }

    private void UsarEspecial()
    {
        if (GameManager.Instance.UsarEspecial())
            Debug.Log("ESPECIAL ATIVADO!");
        else
            Debug.Log("Sem barra especial suficiente.");
    }
}