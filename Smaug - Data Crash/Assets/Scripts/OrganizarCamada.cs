using UnityEngine;

public class OrganizarCamada : MonoBehaviour
{
    private SpriteRenderer[] renderers;

    void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>(true);
    }

    void LateUpdate() // ← mudou de Update para LateUpdate
    {
        if (renderers.Length == 0) return;

        int ordem = Mathf.RoundToInt(transform.position.y * -1f);

        foreach (SpriteRenderer sr in renderers)
            if (sr != null) sr.sortingOrder = ordem;
    }
}