using UnityEngine;

/// <summary>
/// On va gérer tout ce qui touche au mouvement de notre personnage :
/// Vitesse
/// Force de saut, double saut etc..
/// </summary>

public class PlayerController : MonoBehaviour
{
    //Les composants
    public Rigidbody2D _rigidBody;
    public Collision2D _collision;
    private Animator _animator;

    public float _speed;
    public float _jumpForce;
    public int _maxJumpCount;
    private int _jumpCount;
    

    //Les booléens pour les animations et les détéctions de notre personnage
    private bool _isGrounded;
    private bool _isWallLeft;
    private bool _isWallRight;
    private bool _isRoof;
    private bool _isMoving;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        AnimationCheck();
        Jump();
    }

    private void FixedUpdate()
    {
        Move();
        WallMove();
        CharacterFacing();
    }

    private void WallMove()
    {
        if (_isWallLeft || _isWallRight)
        {
            _rigidBody.gravityScale = 0;
            //On se déplace de haut en bas
            if (Input.GetKey("z"))
            {
                _rigidBody.linearVelocityY = 1 * _speed;
            }
            else if (Input.GetKey("s"))
            {
                _rigidBody.linearVelocityY = -1 * _speed;
            }
        }
    }

    private void Move()
    {
        //Déplacement Horizontal
        if (Input.GetKey("d"))
        {
            _isMoving = true;
            //Le 1 équivaut à la direction : Droite
            _rigidBody.linearVelocityX = 1 * _speed * Time.deltaTime;

        }
        else if (Input.GetKey("a"))
        {
            _isMoving = true;
            //Le -1 équivaut à la direction : Gauche
            _rigidBody.linearVelocityX = -1 * _speed * Time.deltaTime;
        }
        else
        {
            _isMoving = false;
            _rigidBody.linearVelocityX = 0;
        }
    }

    private void CharacterFacing()
    {
        if (_rigidBody.linearVelocityX < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (_rigidBody.linearVelocityX > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown("space") && _jumpCount < _maxJumpCount && _isWallLeft)
        {
            _rigidBody.linearVelocityY = _jumpForce;
            _rigidBody.linearVelocityX = -1 * _speed;
            _jumpCount++;
        }
        else if (Input.GetKeyDown("space") && _jumpCount < _maxJumpCount && _isWallRight)
        {
            _rigidBody.linearVelocityY = _jumpForce;
            _rigidBody.linearVelocityX = 1 * _speed;
            _jumpCount++;
        }
        else
        {
            _rigidBody.linearVelocityY = _jumpForce;
            _jumpCount++;
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
                _jumpCount = 0;
            }
            //La gauche c'est un mur sur notre droite
            else if (collision.contacts[0].normal == Vector2.left)
            {
                _isWallRight = true;
            }
            else if (collision.contacts[0].normal == Vector2.right)
            {
                _isWallLeft = true;
            }
            else if (collision.contacts[0].normal == Vector2.down)
            {
                _isRoof = true;
            }
        }
    }

    //On reset tout les états de collisions
    private void OnCollisionExit2D(Collision2D collision)
    {
        _isGrounded = false;
        _isWallLeft = false;
        _isWallRight = false;
        _isRoof = false;
    }

    //Gestion des infos de l'animation
    private void AnimationCheck()
    {
        _animator.SetBool("isMoving", _isMoving);
        _animator.SetBool("isGrounded", _isGrounded);
        _animator.SetFloat("Velocity_Y", _rigidBody.linearVelocityY);
    }


}
