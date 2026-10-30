using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BotaoSom : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Tocar);
    }

    private void Tocar()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.TocarSomBotao();
    }
}