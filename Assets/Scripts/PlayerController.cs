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
    [SerializeField] public float _distanceDW;
    [SerializeField] public LayerMask detectWall;

    private Rigidbody2D rb;
    private Transform transform;
    private SpriteRenderer skin;
    

    private bool canFlip = true;

    void Start()
    {
        TryGetComponent(out rb);
        TryGetComponent(out transform);
        TryGetComponent(out skin);
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
        
        Debug.DrawRay(transform.position, direction * _distanceDW);


        if (hit.collider != null)
            return;



        if (Input.GetButton("Horizontal"))
            {
                transform.position += Vector3.right * Input.GetAxisRaw("Horizontal") * _speed * Time.deltaTime;
            }


        if (Input.GetButton("Fire1"))
            canFlip = false;
        
        else
            canFlip = true;

        if (canFlip)
        {
            if (Input.GetAxisRaw("Horizontal") == -1)
                {
                    transform.localScale = new Vector3(-1,1,1);
                }
            else if (Input.GetAxisRaw("Horizontal") == 1)
                {
                    transform.localScale = new Vector3(1,1,1);
                }
        }
        
    }

    //Fonction pour sauter
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && _currentJump < _limiteJump)
        {
            rb.linearVelocity = Vector2.up * _forceJump;
            _currentJump++;
        }
    }

    //On check les collision avec le sol pour pouvoir reset notre nombre de saut à 0
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            _currentJump = 0;
        }
    }

    //Fonction de récolte de collectibles
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.SetActive(false);
    }
}
