using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Gestion des compétences de notre personnage
/// </summary>
public class PlayerPower : MonoBehaviour
{

    public Rigidbody2D rb;
    public CinemachineCamera mainCamera;
    public PlayerController playerController;
    private CameraController cameraController;
    private LineController lineController;

    
    public LayerMask ignoreLayer;

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

    public GameObject RockPrefab;
    public float LancerMax; //Puissance max du lancer
    public float ForceDistance;

    [Header("Camera Settings")]
    [HideInInspector] private float animTime;
    public float animDuration;
    private float StartLens;
    private bool LensAnimate = false;

    private float CameraGoToLens;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
        lineController = GameObject.Find("TeleportLine").GetComponent<LineController>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraController = GameObject.Find("CinemachineCamera").GetComponent<CameraController>();
        mainCamera = cameraController.gameObject.GetComponent<CinemachineCamera>();

        StartLens = mainCamera.Lens.OrthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        ShadowTeleport();
        ThrowRock();

        if (LensAnimate)
        {
            AnimateCameraLens(CameraGoToLens);
        }
    }

    //Pouvoir de téléportation des ombres
    private void ShadowTeleport()
    {

        if (playerController.InShadow)
        {
            //C'est la touche E
            if (Input.GetKeyDown(KeyCode.Q) && !ShadowTP_Stance && playerController.IsGrounded)
            {
                ShadowTP_Stance = true;

            }
            else if (Input.GetKeyDown(KeyCode.Q) && ShadowTP_Stance)
            {
                ShadowTP_Stance = false;
            }

            //Gestion de la TP
            if (ShadowTP_Stance)
            {
                playerController.MovementLock = true;
                playerController.IsMoving = false;
                rb.linearVelocityX = 0;
                cameraController.ShadowCamera();

                //Si jamais la ligne croise un mur on ne peut pas se tp                
                if (!lineController.DrawLine())
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        //Ici on viens chercher sur quel objet on clic
                        Ray clicray = Camera.main.ScreenPointToRay(Input.mousePosition);

                        RaycastHit2D hit = Physics2D.Raycast(clicray.origin, clicray.direction, Mathf.Infinity, ~ignoreLayer);

                        if (hit.collider != null)
                        {
                            
                            //Si c'est une zone d'ombre alors on peut se tp
                            if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Light"))
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
                lineController.StopDrawLine();
            }
        }

    }

    private void ThrowRock()
    {
        if (Input.GetMouseButton(1))
        {
            cameraController.ShadowCamera();
            ChangeCameraLens(25);
            
        }

        if (Input.GetMouseButtonUp(1))
        {

            Rigidbody2D rock = Instantiate(RockPrefab, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
            rock.gameObject.GetComponent<RockController>()._playerPower = this; // On viens lui donner la référence à ce script

            Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //On récupère la position de la souris dans le monde
            cursorPosition.z = 0; // on règle le Z sinon l'alignement sera pas bon vu que la caméra est à -10

            Vector2 direction = (cursorPosition - transform.position).normalized; // La direction de la souris par rapprot au joueur
            float distance = Vector2.Distance(cursorPosition, transform.position); // On mesure la distance entre les deux

            distance = Mathf.Min(distance, LancerMax); // on cap la puissance du lancer
            float puissance = Mathf.Lerp(0, LancerMax, distance / ForceDistance);

            rock.AddForce(direction * puissance, ForceMode2D.Impulse);
        }
    }

    public void ChangeCameraLens(float distance)
    {
        LensAnimate = true;
        CameraGoToLens = distance;
    }

    private void AnimateCameraLens(float distance)
    {
        float ratio = animTime / animDuration;

        mainCamera.Lens.OrthographicSize = Mathf.Lerp(StartLens, distance , ratio);

        animTime += Time.deltaTime;
        if (animTime > animDuration)
        {
            animTime = 0;
            LensAnimate = false;
            StartLens = distance;
        }
    }

    IEnumerator ExecuteWaiting(GameObject enemy)
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(enemy);
        playerController.MovementLock = false;
    }

    public void CanExecute(GameObject enemy)
    {
        if (Input.GetKey("e"))
        {
            ExecuteEnemy(enemy);
        }
    }

    public void CanDeactivateLightSource(DeactivateLightSource LightScript)
    {
        if (Input.GetKey("e"))
        {
            LightScript.Deactivate = true;
        }
    }

    private void ExecuteEnemy(GameObject enemy)
    {
        //Animation de kill 
        playerController.MovementLock = true;
        playerController.IsMoving = false;
        playerController._rigidBody.linearVelocityX = 0;
        enemy.GetComponent<EnemyController>()._state = EnemyController.STATE.DEATH;
        StartCoroutine(ExecuteWaiting(enemy));
    }
}
