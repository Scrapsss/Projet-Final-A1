using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] private Transform target;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _smoothTime = 0.3f;


    private Vector3 _velocity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var targetPos = Vector3.SmoothDamp(transform.position, target.position + _offset, ref _velocity, _smoothTime );
        targetPos.z = transform.position.z;

        transform.position = targetPos;
    }
}
