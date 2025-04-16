using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] private Transform target;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _smoothTime = 0.2f;

    private Vector3 _smoothedMousePosition;
    private Vector3 _mouseVelocity;
    private float _shadowLerpT = 0f;
    private float _shadowLerpSpeed = 1f;

    private Vector3 _velocity;

    // Update is called once per frame
    void FixedUpdate()
    {
        var targetPos = Vector3.SmoothDamp(transform.position, target.position + _offset, ref _velocity, _smoothTime );
        targetPos.z = transform.position.z;

        transform.position = targetPos;
    }

    public void ChangeTarget( Transform Newtarget )
    {
       target = Newtarget;
    }

    public void ShadowCamera()
    {
        // Ici on vient chercher la position de notre souris par rapport au monde
        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = 10;
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenPoint);

        // Smooth la position du curseur
        _smoothedMousePosition = Vector3.SmoothDamp(_smoothedMousePosition, worldPoint, ref _mouseVelocity, 0.1f);

        // Incrémente _shadowLerpT jusqu'à 0.5f
        _shadowLerpT = Mathf.MoveTowards(_shadowLerpT, 0.5f, Time.deltaTime * _shadowLerpSpeed);

        // Calcule la position intermédiaire avec interpolation dynamique
        Vector3 midPoint = Vector3.Lerp(target.transform.position, _smoothedMousePosition, _shadowLerpT);
        midPoint.z = -10;

        transform.position = midPoint;
    }
}
