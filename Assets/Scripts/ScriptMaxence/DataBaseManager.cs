using UnityEngine;

public class DataBaseManager : MonoBehaviour
{

    private static DataBaseManager _instance;

    public static DataBaseManager Instance => _instance;

    [SerializeField] private EnemyDataBase enemyDataBase;

    private void Awake()
    {
        if ( _instance == null )
            _instance = this;
        else
            Destroy( gameObject );

        DontDestroyOnLoad( gameObject );
    }

    public EnemyData GetData(int id) => enemyDataBase.GetData(id);
}
