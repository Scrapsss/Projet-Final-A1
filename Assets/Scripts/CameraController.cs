using TMPro.Examples;
using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private CinemachineCamera _camera;

    private Vector3 _smoothedMousePosition;
    private Vector3 _mouseVelocity;
    private float _smoothTime = 1f;
    private float _shadowLerpT = 0f;
    private float _shadowLerpSpeed = 1f;

    private Vector3 _velocity;
    public Transform _playerTransform;
    public Transform _TrackingTarget;

    public PlayerPower _playerPower;
    public PlayerController _playerController;

    private void Start()
    {
        _camera = GetComponent<CinemachineCamera>();
        _playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        _TrackingTarget = GameObject.Find("CameraTrackingTarget").GetComponent<Transform>();
        _playerPower = _playerTransform.gameObject.GetComponent<PlayerPower>();
        _playerController = _playerTransform.gameObject.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!_playerPower.ShadowTP_Stance && !_playerController.InObservPoint)
            _TrackingTarget.position = _playerTransform.position;
    }

    public void ChangeTarget(Transform Newtarget)
    {
        _camera.Target.TrackingTarget = Newtarget;
    }

    public void ShadowCamera()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Vector3 mouseOffset = Input.mousePosition - screenCenter;

        // On limite l'offset pour ne pas aller trop loin
        float maxOffsetPixels = 350f; // distance max en pixels
        mouseOffset = Vector3.ClampMagnitude(mouseOffset, maxOffsetPixels);

        // La on convertis nos données écran en position monde
        Vector3 worldOffset = Camera.main.ScreenToWorldPoint(screenCenter + mouseOffset);
        Vector3 playerScreen = Camera.main.WorldToScreenPoint(_playerTransform.position);
        Vector3 targetWorldPos = Camera.main.ScreenToWorldPoint(playerScreen + mouseOffset);

        targetWorldPos.z = 0;
        _TrackingTarget.position = targetWorldPos;
    }

    public void ObservPointCamera(ObservPointManager ObservPoint)
    {
        _TrackingTarget.position = ObservPoint.Destination.position;
    }
}
