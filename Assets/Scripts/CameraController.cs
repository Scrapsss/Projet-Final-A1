using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] private Transform target;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _smoothTime = 0.2f;


    private Vector3 _velocity;

    // Update is called once per frame
    void Update()
    {
        var targetPos = Vector3.SmoothDamp(transform.position, target.position + _offset, ref _velocity, _smoothTime );
        targetPos.z = transform.position.z;

        transform.position = targetPos;
    }

    public void ChangeTarget( Transform Newtarget )
    {
       target = Newtarget;
    }
}
