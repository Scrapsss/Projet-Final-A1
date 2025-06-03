using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private TutorialAnimation TutorialPanel;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            TutorialPanel.Activate = true;
            TutorialPanel.DeActivate = false;
            TutorialPanel.ResetState();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            TutorialPanel.DeActivate = true;
            TutorialPanel.Activate = false;
            TutorialPanel.ResetState();
        }
    }
}
