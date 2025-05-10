using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gestion des compétences de notre personnage
/// </summary>
public class PlayerPower : MonoBehaviour
{

    public Rigidbody2D rb;
    public Camera mainCamera;
    public PlayerController playerController;

    private EdgeCollider2D collider;
    private LineRenderer line;
    public LayerMask ignoreLayer;

    private Vector3[] Vect3List;
    private Vector2[] Vect2List;

    private bool isTriggered;

    

    private bool _shadowTPStance;
    public bool ShadowTP_Stance
    {
        get
        {
            return _shadowTPStance;
        }
        set
        {
            _shadowTPStance = value;
        }
    }


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
        line = GetComponent<LineRenderer>();
        collider = GetComponent<EdgeCollider2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vect3List = new Vector3[2];
        Vect2List = new Vector2[2];
    }

    // Update is called once per frame
    void Update()
    {
        ShadowTeleport();
    }

    private void ShadowTeleport()
    {

        if (playerController.InShadow)
        {
            //C'est la touche E
            if (Input.GetButtonDown("Fire2") && !ShadowTP_Stance && playerController.IsGrounded)
            {
                ShadowTP_Stance = true;

            }
            else if (Input.GetButtonDown("Fire2") && ShadowTP_Stance)
            {
                ShadowTP_Stance = false;
            }

            //Gestion de la TP
            if (ShadowTP_Stance)
            {
                playerController.MovementLock = true;
                playerController.IsMoving = false;
                rb.linearVelocityX = 0;
                //ScriptCamera.ShadowCamera();

                //Si jamais la ligne croise un mur on ne peut pas se tp                
                if (!DrawLine())
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
                                ShadowTP_Stance = false;
                            }
                        }
                    }
                }


            }
            else
            {
                ShadowTP_Stance = false;
                playerController.MovementLock = false;

                StopDrawLine();
                //ScriptCamera.ChangeTarget(transform);
            }
        }

    }

    public bool DrawLine()
    {
        //Ici on vient chercher la position de notre souris par rapport au monde
        Vector3 screenPoint = Input.mousePosition;

        //On précise la distance entre la caméra et le monde qui est de 10 unités
        screenPoint.z = 10;
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenPoint);


        Vect3List[0] = transform.position;
        Vect3List[1] = worldPoint;

        line.SetPositions(Vect3List);

        Vect2List[0] = Vector2.zero;
        Vect2List[1] = worldPoint - transform.position;

        collider.points = Vect2List;

        return isTriggered;
    }
    public void StopDrawLine()
    {
        Vect3List[0] = transform.position;
        Vect3List[1] = transform.position;

        line.SetPositions(Vect3List);

        Vect2List[0] = Vector2.zero;
        Vect2List[1] = Vector2.zero;

        collider.points = Vect2List;
    }

    //Collision de la ligne
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            line.startColor = Color.red;
            line.endColor = Color.red;

            isTriggered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            line.startColor = Color.white;
            line.endColor = Color.white;

            isTriggered = false;
        }
    }
}
