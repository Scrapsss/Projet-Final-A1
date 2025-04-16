using System;
using UnityEngine;


[Serializable]
public struct EnemyStats
{
    public int pv;
    public float speed;
    public int damage;

    public EnemyStats( int pv, float speed, int damage )
    {
        this.pv = pv;
        this.speed = speed;
        this.damage = damage;
    }
}


[Serializable]
public class EnemyData
{

    public string Label;

    public EnemyStats stats;

    [Header("SETUP")]
    public Sprite sprite;
    public Color color;
    public float scaleCoef;

    [Header("STATE")]
    public float durationIDLE;
    public float durationWALK;
}
