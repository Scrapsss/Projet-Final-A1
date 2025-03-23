using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;


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
    private BoxCollider2D monCollider;

    private bool canFlip;
    private bool isFacingRight = true;
    private Vector3 currentScale;

    private bool isGrounded;
    private bool isSprinting;
    private bool isCrouch;
    private bool tryUncrouch;

    //WallJump System
    private bool isWallLeft;
    private bool isWallRight;
    private bool MovementLock;

    private bool isRoof;
    private bool ShadowTP_Stance;

    //ShadowSystem
    private bool inShadow;
    public GameObject line;
    private LineCollider lineScript;
    public LayerMask ignoreLayer;

    void Start()
    {
        TryGetComponent(out rb);
        TryGetComponent(out skin);
        TryGetComponent(out monCollider);

        lineScript = line.GetComponent<LineCollider>();

        currentScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
        ShadowTeleport();
    }

    //Gestion des déplacements et de l'orientation du personnage
    void Move()
    {
        Vector3 direction = Input.GetAxisRaw("Horizontal") * Vector2.right;
        var hit = Physics2D.BoxCast(transform.position,Vector2.one / 2, 0, direction, _distanceDW, detectWall);


        if ( (hit.collider != null) && !(isWallLeft || isWallRight) )
        {
            return;
        }
            
        
        //Quand on sort d'un saut mural on ne peut pas bouger pendant un certain moment
        if (!MovementLock)
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
        if (Input.GetButton("Vertical") && (isWallLeft || isWallRight) && !isGrounded )
            {
                rb.linearVelocityY = Input.GetAxisRaw("Vertical") * _speed;
            }

        if (Input.GetButtonUp("Vertical") && (isWallLeft || isWallRight) && !isGrounded)
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
        
        else
            canFlip = true;


        if (Input.GetButton("Sprint"))
            isSprinting = true;

        else
            isSprinting = false;


        if (Input.GetKeyDown(KeyCode.C) && !isCrouch)
        {
            isCrouch = true;
            currentScale.y /= 2;
            transform.localScale = currentScale;
            _speed /= 2;
            tryUncrouch = false;
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            tryUncrouch = true;
        }

        if (tryUncrouch)
        {
            if (!Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y + currentScale.y), currentScale.y, detectWall))
            {
                isCrouch = false;
                currentScale.y *= 2;
                transform.localScale = currentScale;
                _speed *= 2;
                tryUncrouch = false;
            }
        }




        if (canFlip)
        {
            //Si on va à gauche
            if ( ( Math.Round(rb.linearVelocityX) < 0) && isFacingRight == true)
                {
                    currentScale.x = -currentScale.x;
                    transform.localScale = currentScale;
                    isFacingRight = false;
                }
            //Sinon si on va à droite
            else if ( ( Math.Round(rb.linearVelocityX) > 0) && isFacingRight == false)
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
        if (!MovementLock)
        {
            //Saut mural
            //Wall jump vers la droite
            if (Input.GetButtonDown("Jump") && isWallLeft && !isGrounded)
            {
                rb.linearVelocityY = _forceJump;

                //On force le personnage à s'éloigner du mur
                rb.linearVelocityX = 1 * (_speed * 2);

                //Ici on viens bloquer les contrôle de déplacement pendant quelques temps pour laisser le personnage se détacher du mur
                MovementLock = true;
                StartCoroutine(JumpMovementCooldown());
            }
            //WallJump vers la gauche
            else if (Input.GetButtonDown("Jump") && isWallRight && !isGrounded)
            {
                rb.linearVelocityY = _forceJump;
                rb.linearVelocityX = -1 * (_speed * 2);

                MovementLock = true;
                StartCoroutine(JumpMovementCooldown());
            }

            //Impossible de sauter si on est contre un plafond (pour éviter de consommer les charges de sauts )
            //Saut normal 
            else if (Input.GetButtonDown("Jump") && _currentJump < _limiteJump && !isRoof)
            {
                rb.linearVelocityY = _forceJump;
                _currentJump++;
            }
        }
        
    }

    //On patiente 0.25 secondes avant de débloquer les contrôle de déplacements
    IEnumerator JumpMovementCooldown()
    {
        yield return new WaitForSeconds(0.25f);
        MovementLock = false;
    }

    //------- Shadow Teleport Power -------

    private void ShadowTeleport()
    {

        if (inShadow)
        {
            //C'est la touche E
            if (Input.GetButtonDown("Fire2") && !ShadowTP_Stance && isGrounded)
            {
                ShadowTP_Stance = true;
            }
            else if (Input.GetButtonDown("Fire2") && ShadowTP_Stance)
            {
                ShadowTP_Stance = false;
                MovementLock = false;

                lineScript.StopDrawLine();
                
            }

            //Gestion de la TP
            if (ShadowTP_Stance)
            {
                MovementLock = true;
                rb.linearVelocityX = 0;

                //Si jamais la ligne croise un mur on ne peut pas se tp                
                if ( !lineScript.DrawLine() )
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        //Ici on viens chercher sur quel objet on clic
                        Ray clicray = Camera.main.ScreenPointToRay(Input.mousePosition);

                        RaycastHit2D hit = Physics2D.Raycast(clicray.origin, clicray.direction, Mathf.Infinity, ~ignoreLayer);

                        if (hit.collider != null)
                        {
                            //Si c'est une zone d'ombre alors on peut se tp
                            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Shadow"))
                            {
                                //Ici on vient chercher la position de notre souris par rapport au monde
                                Vector3 screenPoint = Input.mousePosition;

                                //On précise la distance entre la caméra et le monde qui est de 10 unités
                                screenPoint.z = 10;
                                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenPoint);

                                transform.position = worldPoint;
                            }
                        }
                    }
                }

                
            }
        }
        
    }




    //------- Gestion des Collisions ---------

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

    private void OnCollisionStay2D(Collision2D collision)
    {
        Vector3 normal = collision.GetContact(0).normal;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            //Si c'est le haut de la collision alors on peut marcher dessus
            if (normal == Vector3.up)
            {
                isGrounded = true;
                _currentJump = 0;
            }
        }
            
    }

    //Fonction de récolte de collectibles
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Collectible"))
            collision.gameObject.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Ici on check si on est dans une ombre
        if (collision.gameObject.layer == LayerMask.NameToLayer("Shadow"))
        {
            inShadow = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        inShadow = false;
    }
}
