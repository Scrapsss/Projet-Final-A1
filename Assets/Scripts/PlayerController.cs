using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("MOVE")]
    [SerializeField]private int _speed = 2;


    [Header("JUMP")]
    [SerializeField] private float _forceJump = 5;
    [SerializeField] private int _currentJump = 0;
    [SerializeField] private int _limiteJump = 2;

    [Header("WALL")]
    [SerializeField] private float _distanceDW;
    [SerializeField] private LayerMask detectWall;

    private Rigidbody2D rb;
    private SpriteRenderer skin;
    private CapsuleCollider2D monCollider;

    private bool canFlip = true;
    private bool isFacingRight = true;
    private Vector3 currentScale;

    private bool isGrounded;
    private bool isSprinting;

    //WallJump System
    private bool isWallLeft;
    private bool isWallRight;
    private bool WallJumpLock;

    private bool isRoof;

    void Start()
    {
        TryGetComponent(out rb);
        TryGetComponent(out skin);
        TryGetComponent(out monCollider);

        currentScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
    }

    //Gestion des déplacements et de l'orientation du personnage
    void Move()
    {
        Vector3 direction = Input.GetAxisRaw("Horizontal") * Vector2.right;
        var hit = Physics2D.BoxCast(transform.position,Vector2.one, 0, direction, _distanceDW, detectWall);


        if (hit.collider != null)
            return;
        
        //Quand on sort d'un saut mural on ne peut pas bouger pendant un certain moment
        if (!WallJumpLock)
        { 
            //Déplacement horizontaux
            if (Input.GetButton("Horizontal") && isGrounded && !isSprinting)
                {
                    rb.linearVelocityX = Input.GetAxisRaw("Horizontal") * _speed;
                }
            
            else if ( (Input.GetButton("Horizontal") && !isGrounded) || (Input.GetButton("Horizontal") && isSprinting))
            {
                rb.linearVelocityX = Input.GetAxisRaw("Horizontal") * (_speed * 2);
            }
        
            if (Input.GetButtonUp("Horizontal"))
            {
                rb.linearVelocityX = 0;
            }
        }

        //Déplacement verticaux
        if (Input.GetButton("Vertical") && (isWallLeft || isWallRight) )
            {
                rb.linearVelocityY = Input.GetAxisRaw("Vertical") * _speed;
            }

        if (Input.GetButtonUp("Vertical") && ( isWallLeft || isWallRight) )
        {
            rb.linearVelocityY = 0;
        }

        //Déplacement sur plafond
        if (Input.GetAxisRaw("Vertical") == -1 && isRoof)
        {
            rb.gravityScale = 4;
        }

        //Gestion de pression de touche externe pour différents états comme le sprint ou le blocage de rotation
        if (Input.GetButton("Fire1"))
            canFlip = false;

        if (Input.GetButton("Sprint"))
            isSprinting = true;

        else
            isSprinting = false;
            canFlip = true;

        if (canFlip)
        {
            //Si on va à gauche
            if ( (rb.linearVelocityX < 0) && isFacingRight == true)
                {
                    currentScale.x = -currentScale.x;
                    transform.localScale = currentScale;
                    isFacingRight = false;
                }
            //Sinon si on va à droite
            else if ( (rb.linearVelocityX >0) && isFacingRight == false)
                {
                    currentScale.x = -currentScale.x;
                    transform.localScale = currentScale;
                isFacingRight = true;
                }
        }
        
    }

    //Fonction pour sauter
    void Jump()
    {
        //Saut mural
        //Wall jump vers la droite
        if (Input.GetButtonDown("Jump") && isWallLeft)
        {
            rb.linearVelocityY = _forceJump;

            //On force le personnage à s'éloigner du mur
            rb.linearVelocityX = 1 * _speed;

            //Ici on viens bloquer les contrôle de déplacement pendant quelques temps pour laisser le personnage se détacher du mur
            WallJumpLock = true;
            StartCoroutine(JumpMovementCooldown());
        }
        //WallJump vers la gauche
        else if (Input.GetButtonDown("Jump") && isWallRight)
        {
            rb.linearVelocityY = _forceJump;
            rb.linearVelocityX = -1 * _speed;

            WallJumpLock = true;
            StartCoroutine(JumpMovementCooldown());
        }

        //Impossible de sauter si on est contre un plafond (pour éviter de consommer les charges de sauts
        //Saut normal 
        else if (Input.GetButtonDown("Jump") && _currentJump < _limiteJump && !isRoof)
        {
            rb.linearVelocityY = _forceJump;
            _currentJump++;
        }
    }

    //On patiente 0.25 secondes avant de débloquer les contrôle de déplacements
    IEnumerator JumpMovementCooldown()
    {
        yield return new WaitForSeconds(0.25f);
        WallJumpLock = false;
    }

    //Dès qu'on quitte les collision nos marqueurs passent en false
    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
        isWallLeft = false;
        isWallRight = false;
        isRoof = false;

        rb.gravityScale = 4;
    }


    //On check quelle collision on touche pour savoir quoi faire
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            //On viens mesurer quelle collision on touche en premier
            Vector3 normal = collision.GetContact(0).normal;

            //Si c'est le haut de la collision alors on peut marcher dessus
            if (normal == Vector3.up)
            {
                isGrounded = true;
                _currentJump = 0;
            }

            //Si c'est la gauche ou la droite alors on grimpe sur le mur et on désactive la gravité pour ne pas tomber
            if ( normal == Vector3.left )
            {
                isWallRight = true;
                rb.linearVelocityY = 0;
                rb.gravityScale = 0;
            }

            if ( normal == Vector3.right )
            {
                isWallLeft = true;
                rb.linearVelocityY = 0;
                rb.gravityScale = 0;
            }

            if ( normal == Vector3.down )
            {
                isRoof = true;
                rb.linearVelocityX = 0;
                rb.gravityScale = 0;
            }
        }
    }

    //Fonction de récolte de collectibles
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.SetActive(false);
    }
}
