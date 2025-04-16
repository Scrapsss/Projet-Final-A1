using UnityEngine;

/// <summary>
/// Ce script va gérer la détection de collisions par rapport à la vision de l'ennemi et influencer son état
/// </summary>

public class EnemyVisionTrigger : MonoBehaviour
{

    private EnemyController ScriptParent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ScriptParent = GetComponentInParent<EnemyController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ScriptParent.VisionEnter(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ScriptParent.VisionExit(collision);
    }
}
