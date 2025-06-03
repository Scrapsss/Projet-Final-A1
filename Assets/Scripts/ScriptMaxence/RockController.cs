using System.Collections;
using UnityEngine;

public class RockController : MonoBehaviour
{

    private CircleCollider2D _circleCollider;
    private Rigidbody2D _rb;
    public PlayerPower _playerPower;
    public LayerMask enemyMask;

    private bool _canMakeSound = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _circleCollider = GetComponent<CircleCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (Mathf.Abs(_rb.linearVelocityX) > 10)
            {
                _rb.linearVelocityX /= Mathf.Abs(_rb.linearVelocityX);
                _rb.linearVelocityX *= 10;
            }

            if (_canMakeSound)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _circleCollider.radius, enemyMask);

                foreach (Collider2D hit in hits)
                {
                    EnemyController enemy = hit.GetComponent<EnemyController>();
                    if (enemy != null)
                    {
                        enemy._state = EnemyController.STATE.SOUNDHEARD;
                        enemy.SoundTarget = transform.position;
                    }
                }
                StartCoroutine(SelfDestruct());
                _playerPower.ChangeCameraLens(20);
            }
            _canMakeSound = false;
            
        }
    }
}
