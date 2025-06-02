using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UIElements;

// Exemple d'opération ternaire
// public Vector2 direction => DirectionMode = MOVE_DIR HORINZONTAL ? Vector2.right : Vector2.up;
//         On déclare                     La question                    Si oui        Si non


public class EnemyController : MonoBehaviour
{

    private EnemyData data;

    public int id;

    private Rigidbody2D _rigidBody;
    private SpriteRenderer _spriteRenderer;
    private CapsuleCollider2D _collider;
    private Animator _animator;

    [SerializeField] private GameObject _executionCollider;

    private float direction;

    //Les limites de patrouilles
    public float LimitePatrouilleDroite;
    public float LimitePatrouilleGauche;
    private Vector3 LimiteDroite;
    private Vector3 LimiteGauche;

    private Vector3 _scale;

    [SerializeField] private GameObject _executionIcon;

    //Gestion de la vision
    public Transform Target;
    public Vector3 SoundTarget;

    //Les booléens pour les animations et les détéctions de notre personnage (Pareil quand dans notre personnage mais seulement ceux utiles)
    private bool _isGrounded;
    public bool IsGrounded
    {
        get
        { return _isGrounded; }
        set
        { _isGrounded = value; }
    }
    private bool _isMoving;
    public bool IsMoving
    {
        get
        { return _isMoving; }
        set
        { _isMoving = value; }
    }
    private bool _isRunning;
    public bool IsRunning
    {
        get { return _isRunning; }
        set { _isRunning = value; }
    }

    //InShadow pourrait être utile à un moment donc je le garde
    private bool _inShadow = true;
    public bool InShadow
    {
        get
        {
            return _inShadow;
        }
        set
        {
            _inShadow = value;
        }
    }

    private bool _isExecutable;
    public bool IsExecutable
    {
        get { return _isExecutable; }
        set { _isExecutable = value; }
    }

    private enum StartPatrolSide
    {
        Left,
        Right
    }

    [SerializeField] private StartPatrolSide _StartPatrolSide;

    public enum STATE
    {
        NONE,
        INIT,
        IDLE,
        MOVE,
        CHASE,
        SOUNDHEARD,
        SOUNDCHECK,
        FIRE,
        DEATH
    }

    public STATE _state;

    private float _countdown;
    private float _SoundCountdown;

    [SerializeField] private Transform _VisualStateGO;
    [SerializeField] private GameObject QuestionState;
    [SerializeField] private GameObject SeenState;
    [SerializeField] private GameObject ChaseState;

    [HideInInspector] public bool ChaseCoroutineDone;

    private void Awake()
    {
        TryGetComponent(out _rigidBody);
        TryGetComponent(out _spriteRenderer);
        TryGetComponent(out _collider);
        TryGetComponent(out _animator);

        _scale = transform.localScale;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        data = DataBaseManager.Instance.GetData(id);
        Init();

        LimiteDroite = new Vector3(transform.position.x + LimitePatrouilleDroite, transform.position.y, transform.position.z);
        LimiteGauche = new Vector3(transform.position.x - LimitePatrouilleGauche, transform.position.y, transform.position.z);

        if (_StartPatrolSide == StartPatrolSide.Left)
            direction = -1;
        else 
            direction = 1;
    }

    private void Init()
    {
        _state = STATE.INIT;
        _spriteRenderer.sprite = data.sprite;
        _spriteRenderer.color = data.color;
    }

    private void Update()
    {
        StateManager();
        CharacterFacing();
        AnimationCheck();
        ExecuteIcon();
        IsInShadow();
    }

    private void IsInShadow()
    {
        if (InShadow)
            _executionCollider.SetActive(true);
        else if (!InShadow)
            _executionCollider.SetActive(false);
    }

    private void ExecuteIcon()
    {
        if (_isExecutable)
            _executionIcon.SetActive(true);
        else
            _executionIcon.SetActive(false);
    }

    private void ChangePatrolVisual(int index)
    {
        QuestionState.SetActive(false);
        SeenState.SetActive(false);
        ChaseState.SetActive(false);

        switch(index)
        {
            case 1:
                QuestionState.SetActive(true);
                break;
            case 2: 
                SeenState.SetActive(true);
                break;
            case 3:
                ChaseState.SetActive(true);
                break;
            default:
                break;
        }
    }

    //Gestion de l'état de l'ennemi
    private void StateManager()
    {
        if (_state < STATE.INIT)
            return;

        switch (_state)
        {
            case STATE.NONE:
                IsMoving = false;
                break;
            case STATE.INIT:
                _state = STATE.IDLE;

                break;
            case STATE.IDLE:
                ChangePatrolVisual(0); // On active rien du tout pour simplement tout désactiver
                ChaseCoroutineDone = false; // On reste la coroutine de chasse

                if (_countdown > data.durationIDLE)
                {
                    _state = STATE.MOVE;
                    _countdown = 0;
                }

                _rigidBody.linearVelocityX = 0;
                _countdown += Time.deltaTime;
                _isMoving = false;
                break;
            case STATE.MOVE:

                //ici on fait le code la patrouille, ça c'est le déplacement
                _rigidBody.linearVelocityX = data.stats.speed * direction;
                _isMoving = true;

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
                IsMoving = true;
                if (!ChaseCoroutineDone)
                    StartCoroutine(PlayerDetected()); // Activer le visuel correspondant
                ChaseCoroutineDone = true; // Il la lancera qu'une seule fois

                Vector2 ChaseDirection = ((Vector2)Target.position - (Vector2)transform.position).normalized;
                _rigidBody.linearVelocityX = ChaseDirection.x * data.stats.speed;
                break;
            case STATE.SOUNDHEARD:
                ChangePatrolVisual(1);

                if (_SoundCountdown > data.durationSOUNDHEARD)
                {
                    _state = STATE.SOUNDCHECK;
                    _SoundCountdown = 0;
                }

                _rigidBody.linearVelocityX = 0;
                _SoundCountdown += Time.deltaTime;
                _isMoving = false;
                break;
            case STATE.SOUNDCHECK:
                IsMoving = true;

                //On mesure la différence entre les deux points
                float diff = SoundTarget.x - transform.position.x;
                //On détermine si la différence est négative ou positive
                direction = Mathf.Sign(diff);

                if (Mathf.Abs(diff) < 0.1f) // On propose un environ autour du point enregistré
                {
                    _state = STATE.IDLE;
                    _countdown = 0;
                }

                _rigidBody.linearVelocityX = direction * data.stats.speed;
                break;
            case STATE.FIRE:
                break;
            case STATE.DEATH:
                _isMoving = false;
                break;
        }
    }

    private IEnumerator PlayerDetected()
    {
        ChangePatrolVisual(2);
        yield return new WaitForSeconds(1);
        ChangePatrolVisual(3);
    }

    //Les mêmes code que le personnages puisque ce sont des contrôles de bases
    private void CharacterFacing()
    {
        if (_rigidBody.linearVelocityX < 0)
        {
            _scale.x = -1;
            transform.localScale = _scale;
            _VisualStateGO.localScale = _scale; // On inverse le scale du canvas aussi pour le garder dans le même sens constamment
        }
        else if (_rigidBody.linearVelocityX > 0)
        {
            _scale.x = 1;
            transform.localScale = _scale;
            _VisualStateGO.localScale = _scale;
        }
    }

    private void AnimationCheck()
    {
        _animator.SetBool("isMoving", _isMoving);
        _animator.SetBool("isGrounded", _isGrounded);
        _animator.SetFloat("Velocity_Y", _rigidBody.linearVelocityY);
    }

    public IEnumerator ReturnToIdle()
    {
        yield return new WaitForSeconds(2);
        _state = STATE.IDLE;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Light"))
        {
            _inShadow = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _inShadow = true;
    }

    //Gestion des collisions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().Die(); // On lance la séquence de mort du joueur si on le touche
            _state = STATE.NONE;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        //On vérifie si l'objet qu'on touche fait partie des murs du jeu
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            //Si on touche le haut de l'objet c'est un sol
            if (collision.contacts[0].normal == Vector2.up)
            {
                _isGrounded = true;
            }
        }
    }

    //On reset tout les états de collisions
    private void OnCollisionExit2D(Collision2D collision)
    {
        _isGrounded = false;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(LimiteDroite, LimiteGauche);
    }
}
