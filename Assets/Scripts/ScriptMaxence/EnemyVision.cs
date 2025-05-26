using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    private PolygonCollider2D _collider;
    public EnemyController _enemyController;

    private Coroutine _returnToIdle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _collider = GetComponent<PolygonCollider2D>();
        _enemyController = GetComponentInParent<EnemyController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _enemyController._state = EnemyController.STATE.CHASE;
            _enemyController.Target = collision.gameObject.transform;

            if (_returnToIdle != null)
                StopCoroutine(_returnToIdle);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _returnToIdle = StartCoroutine(_enemyController.ReturnToIdle());
        }
    }
}
