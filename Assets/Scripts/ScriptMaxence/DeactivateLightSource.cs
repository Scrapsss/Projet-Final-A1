using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DeactivateLightSource : MonoBehaviour
{
    [SerializeField] private Light2D LightSource;
    [SerializeField] private GameObject VisualFeedBack;
    private Color nativeColor;
    private Color endColor;

    private BoxCollider2D _collider;

    public float a_Duration;
    private float a_Time = 0;

    public bool Deactivate = false;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        nativeColor = LightSource.color;
        endColor = new Color(nativeColor.r, nativeColor.g, nativeColor.b, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Deactivate)
        {
            LightFadeOut();
        }
    }

    void LightFadeOut()
    {
        float ratio = a_Time / a_Duration;

        LightSource.color = Color.Lerp(nativeColor, endColor, ratio);

        a_Time += Time.unscaledDeltaTime;
        if (a_Time > a_Duration)
        {
            a_Time = 0;
            Deactivate = false;
            Destroy(LightSource.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            VisualFeedBack.SetActive(true);
            collision.gameObject.GetComponent<PlayerPower>().CanDeactivateLightSource(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            VisualFeedBack.SetActive(false);
        }
    }
}
