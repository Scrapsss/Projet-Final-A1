using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EnemyDataBase", menuName = "Datas/Enemy" , order = 1)]
public class EnemyDataBase : ScriptableObject
{
    public List<EnemyData> datas = new();

    public EnemyData GetData(int id)
    {
        id = Mathf.Clamp(id, 0, datas.Count);
        return datas[id];
    }
}
