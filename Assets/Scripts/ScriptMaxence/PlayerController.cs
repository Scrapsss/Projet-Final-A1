using System.Collections;
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
    private SpriteRenderer _spriteRenderer;

    public float _speed;
    public float _jumpForce;
    public int _maxJumpCount;
    private int _jumpCount;
    

    //Les booléens pour les animations et les détéctions de notre personnage
    private bool _isGrounded;
    public bool IsGrounded
    {
        get
        { return _isGrounded; }
        set
        { _isGrounded = value; }
    }
    private bool _isWallLeft;
    public bool IsWallLeft
    {
        get
        { return _isWallLeft; }
        set
        { _isWallLeft = value; }
    }
    private bool _isWallRight;
    public bool IsWallRight
    {
        get
        { return _isWallRight; }
        set
        { _isWallRight = value; }
    }
    private bool _isRoof;
    public bool IsRoof
    {
        get
        { return _isRoof; }
        set
        { _isRoof = value; }
    }
    private bool _isMoving;
    public bool IsMoving
    {
        get
        { return _isMoving; }
        set
        { _isMoving = value; }
    }
    private bool _MovementLock;
    public bool MovementLock
    {
        get
        { return _MovementLock; }
        set
        { _MovementLock = value; }
    }
    private bool _inShadow;
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

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimationCheck();
        Jump();
        RoofMove();
    }

    private void FixedUpdate()
    {
        Move();
        WallMove();
        CharacterFacing();
    }

    //Gestion des déplacement muraux
    private void WallMove()
    {
        if (!_MovementLock)
        {
            if (_isWallLeft || _isWallRight)
            {
                _rigidBody.gravityScale = 0;
                //On se déplace de haut en bas (Faut mettre les touches en qwerty pour que ça les captes bien
                if (Input.GetKey("w"))
                {
                    _rigidBody.linearVelocityY = 1 * _speed;
                }
                else if (Input.GetKey("s"))
                {
                    _rigidBody.linearVelocityY = -1 * _speed;
                }
                else
                {
                    _rigidBody.linearVelocityY = 0;
                }
            }
        }

        //Reset de la gravité quand on touche pas les murs ou le plafond
        if (!_isWallLeft && !_isWallRight && !_isRoof)
        {
            _rigidBody.gravityScale = 6;
        }
    }

    //Déplacement au plafond (on enlève juste la gravité, Move() fera le reste)
    private void RoofMove()
    {
        if (_isRoof)
        {
            _rigidBody.gravityScale = 0;

            if (Input.GetKey("s"))
            {
                _rigidBody.gravityScale = 6;
            }
        }
        else if (!_isRoof)
        {
            _rigidBody.gravityScale = 6;
        }
    }

    //Déplacement gauche droite de base
    private void Move()
    {
        if (!_MovementLock)
        {
            //Déplacement Horizontal
            if (Input.GetKey("d"))
            {
                _isMoving = true;
                //Le 1 équivaut à la direction : Droite
                _rigidBody.linearVelocityX = 1 * _speed;

            }
            else if (Input.GetKey("a"))
            {
                _isMoving = true;
                //Le -1 équivaut à la direction : Gauche
                _rigidBody.linearVelocityX = -1 * _speed;
            }
            else
            {
                _isMoving = false;
                _rigidBody.linearVelocityX = 0;
            }
        } 
    }

    //Gestion de l'orientation du personnage en fonction de sa vélocité
    private void CharacterFacing()
    {
        if (_rigidBody.linearVelocityX < 0)
        {
            _spriteRenderer.flipX = true;
        }
        else if (_rigidBody.linearVelocityX > 0)
        {
            _spriteRenderer.flipX = false;
        }
    }

    //Gestion du saut et des sauts muraux
    private void Jump()
    {
        if (Input.GetKeyDown("space") && _jumpCount < _maxJumpCount && _isWallLeft)
        {
            _rigidBody.linearVelocityY = _jumpForce;
            _rigidBody.linearVelocityX = 1 * _speed;
            _MovementLock = true;
            StartCoroutine(MovementLockCooldown());
        }
        else if (Input.GetKeyDown("space") && _jumpCount < _maxJumpCount && _isWallRight)
        {
            _rigidBody.linearVelocityY = _jumpForce;
            _rigidBody.linearVelocityX = -1 * _speed;
            _MovementLock = true;
            StartCoroutine(MovementLockCooldown());
        }
        else if (Input.GetKeyDown("space") && _jumpCount < _maxJumpCount)
        {
            _rigidBody.linearVelocityY = _jumpForce;
            _jumpCount++;
        }
    }

    //Cooldown du bloquage des contrôles suite au saut mural
    IEnumerator MovementLockCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        _MovementLock = false;
    }

    //Gestion des infos de l'animation
    private void AnimationCheck()
    {
        _animator.SetBool("isMoving", _isMoving);
        _animator.SetBool("isGrounded", _isGrounded);
        _animator.SetFloat("Velocity_Y", _rigidBody.linearVelocityY);
    }

    //Gestion des collisions
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
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Si on entre dans une ombre
        if (collision.gameObject.layer == LayerMask.NameToLayer("Shadow"))
        {
            _inShadow = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            //La gauche c'est un mur sur notre droite
            if (collision.contacts[0].normal == Vector2.left)
            {
                _isWallRight = true;
                _jumpCount = 0;
            }
            else if (collision.contacts[0].normal == Vector2.right)
            {
                _isWallLeft = true;
                _jumpCount = 0;
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        _inShadow = false;
    }





}
