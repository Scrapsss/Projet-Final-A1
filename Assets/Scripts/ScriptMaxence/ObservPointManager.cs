using UnityEngine;

public class ObservPointManager : MonoBehaviour
{
    [SerializeField] public Transform Destination;

    private void Start()
    {
        Destination = GetComponentsInChildren<Transform>()[1];
    }
}
