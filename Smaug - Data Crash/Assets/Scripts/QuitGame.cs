using UnityEngine;

public class QuitGame : MonoBehaviour
{
    /// <summary>
    /// Fecha o jogo. No Editor, para o modo Play.
    /// Em builds, encerra a aplicação.
    /// </summary>
    public void SairDoJogo()
    {
        Debug.Log("Fechando o jogo...");

#if UNITY_EDITOR
            // Para o Play Mode no Editor
            UnityEditor.EditorApplication.isPlaying = false;
#else
        // Encerra o jogo na build final
        Application.Quit();
#endif
    }
}