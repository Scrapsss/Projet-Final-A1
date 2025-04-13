using System;
using UnityEngine;


[Serializable]
public class EnemyData
{

    public string Label;

    [Header("STATS")]
    public int pv;
    public float speed;
    public int damage;

    [Header("SETUP")]
    public Sprite sprite;
    public Color color;
    public float scaleCoef;

    [Header("STATE")]
    public float durationIDLE;
    public float durationWALK;
}
