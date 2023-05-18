using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/EnemyData")]
public class EnemyData : ScriptableObject
{
    public float size;
    public float maxHealth;

    [Range(0, 2)] public float speedDelay; 
    public float baseDamage;

    public EnemyBaseData enemyBase;

    private void Awake()
    {
        Debug.Assert(size != 0, "Size is 0");
        Debug.Assert(maxHealth != 0, "Max health is 0");
        Debug.Assert(baseDamage != 0, "Base damage is 0");

        Debug.Assert(enemyBase != null, "Enemy Base data is null");
    }
}
