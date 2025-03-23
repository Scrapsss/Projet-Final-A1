using UnityEngine;
using UnityEngine.U2D;

public class LineCollider : MonoBehaviour
{
    private EdgeCollider2D collider;
    private LineRenderer line;

    private Vector3[] Vect3List;
    private Vector2[] Vect2List;

    public GameObject Player;
    private Transform PlayerTransform;

    private bool isTriggered;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TryGetComponent(out collider);
        TryGetComponent(out line);

        Vect3List = new Vector3[2];
        Vect2List = new Vector2[2];

        PlayerTransform = Player.GetComponent<Transform>();

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = PlayerTransform.position;
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
