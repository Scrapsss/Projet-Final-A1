using UnityEngine;

public class ExecCollision : MonoBehaviour
{
    private EnemyController enemyController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyController = GetComponentInParent<EnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            enemyController.IsExecutable = true;
            collision.gameObject.GetComponent<PlayerPower>().CanExecute(enemyController.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        enemyController.IsExecutable = false;
    }
}
