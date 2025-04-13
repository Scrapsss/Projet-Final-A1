using UnityEngine;
using System;

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

    public enum STATE
    {
        NONE,
        INIT,
        IDLE,
        MOVE,
        FOLLOW,
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
    }

    private void Init()
    {
        _state = STATE.INIT;
        skin.sprite = data.sprite;
        skin.color = data.color;
    }

    private void Update()
    {
        if ( _state < STATE.INIT )
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
                    

                _countdown += Time.deltaTime;
                break;
            case STATE.MOVE:

                //ici on fait le code la patrouille, ça c'est le déplacement
                transform.position += Vector3.right * Time.deltaTime * data.stats.speed * direction;

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
            case STATE.FOLLOW:
                break;
            case STATE.FIRE:
                break;
            case STATE.DEATH:
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.TransformPoint(LimiteDroite) ,1f);
        Gizmos.DrawLine(LimiteDroite, LimiteGauche);
        Gizmos.DrawWireSphere(transform.TransformPoint(LimiteGauche),1f);
    }
}
