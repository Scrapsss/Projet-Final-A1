using UnityEngine;
using System;
using System.Collections;

// Exemple d'opération ternaire
// public Vector2 direction => DirectionMode = MOVE_DIR HORINZONTAL ? Vector2.right : Vector2.up;
//         On déclare                     La question                    Si oui        Si non


public class EnemyController : MonoBehaviour
{

    private EnemyData data;

    public int id;

    private Rigidbody2D rb;
    private SpriteRenderer skin;
    private BoxCollider2D monCollider2D;

    private float direction = 1;

    public float LimitePatrouilleDroite;
    public float LimitePatrouilleGauche;
    private Vector3 LimiteDroite;
    private Vector3 LimiteGauche;

    private Vector3 currentScale;

    //Gestion de la vision
    private GameObject Target;

    public enum STATE
    {
        NONE,
        INIT,
        IDLE,
        MOVE,
        CHASE,
        FIRE,
        DEATH
    }

    [SerializeField] private STATE _state;

    private float _countdown;

    private void Awake()
    {
        TryGetComponent(out rb);
        TryGetComponent(out skin);
        TryGetComponent(out monCollider2D);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        data = DataBaseManager.Instance.GetData(id);
        Init();

        LimiteDroite = new Vector3(transform.position.x + LimitePatrouilleDroite, transform.position.y, transform.position.z);
        LimiteGauche = new Vector3(transform.position.x - LimitePatrouilleGauche, transform.position.y, transform.position.z);

        currentScale = transform.localScale;
    }

    private void Init()
    {
        _state = STATE.INIT;
        skin.sprite = data.sprite;
        skin.color = data.color;
    }

    private void Update()
    {

        StateManager();
        FlipCheck();
        
    }

    //Gestion de l'état de l'ennemi
    private void StateManager()
    {
        if (_state < STATE.INIT)
            return;

        switch (_state)
        {
            case STATE.NONE:
                break;
            case STATE.INIT:
                _state = STATE.IDLE;

                break;
            case STATE.IDLE:
                if (_countdown > data.durationIDLE)
                {
                    _state = STATE.MOVE;
                    _countdown = 0;


                }

                rb.linearVelocityX = 0;
                _countdown += Time.deltaTime;
                break;


            case STATE.MOVE:

                //ici on fait le code la patrouille, ça c'est le déplacement
                rb.linearVelocityX = data.stats.speed * direction;

                //Maintenant on va faire le faire patrouiller entre 2 points tout en rajoutant des sécurités au cas où il sortirait de la portée de sa patrouille
                if (direction >= 1)
                {
                    if (transform.position.x >= LimiteDroite.x)
                    {
                        _state = STATE.IDLE;
                        direction *= -1;
                    }
                }
                else if (transform.position.x <= LimiteGauche.x)
                {
                    _state = STATE.IDLE;
                    direction *= -1;
                }

                //Avec cette vérification de direction, même si pour une quelconque raison il dépasse sa portée de patrouille, il va y retourner tout seul pour se remettre dedans




                break;
            case STATE.CHASE:
                Vector2 ChaseDirection = ((Vector2)Target.transform.position - (Vector2)transform.position).normalized;
                rb.linearVelocityX = ChaseDirection.x * data.stats.speed;


                break;
            case STATE.FIRE:
                break;
            case STATE.DEATH:
                break;
        }
    }

    private void FlipCheck()
    {
        // Gestion de l'orientation de l'ennemi
        //Si on va à gauche
        if (rb.linearVelocityX < 0)
        {
            currentScale.x = -Math.Abs(currentScale.x);
            transform.localScale = currentScale;
        }
        //Sinon si on va à droite
        else if (rb.linearVelocityX > 0)
        {
            currentScale.x = Math.Abs(currentScale.x);
            transform.localScale = currentScale;
        }
    }

    private IEnumerator ReturnToIdle()
    {
        yield return new WaitForSeconds(2);
        _state = STATE.IDLE;
    }

    public void VisionEnter(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _state = STATE.CHASE;
            Target = collision.gameObject;

            StopCoroutine(ReturnToIdle());
        }
    }

    public void VisionExit(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            StartCoroutine(ReturnToIdle());
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(LimiteDroite, 0.5f);
        Gizmos.DrawLine(LimiteDroite, LimiteGauche);
        Gizmos.DrawWireSphere(LimiteGauche, 0.5f);
    }
}
